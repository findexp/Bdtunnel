/* BoutDuTunnel Copyright (c) 2007-2010 Sebastien LEBRETON

Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. */

#region " Inclusions "
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

using Bdt.Server.Resources;
using Bdt.Shared.Logs;
using Bdt.Shared.Request;
using Bdt.Shared.Response;
#endregion

namespace Bdt.Server.Service
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Une session utilisateur au sein du tunnel
    /// </summary>
    /// -----------------------------------------------------------------------------
    public class TunnelSession : TimeoutObject 
    {
        #region " Constantes "
        // Le test de la connexion effective
        public const int SOCKET_TEST_POLLING_TIME = 100; // msec
        #endregion

        #region " Attributs "
        protected string m_username;
        protected bool m_admin; 
        protected DateTime m_logon;
        protected int m_connectiontimeoutdelay;
        private Dictionary<int, TunnelConnection> m_connections = new Dictionary<int, TunnelConnection>();
        #endregion

        #region " Proprietes "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Les connexions
        /// </summary>
        /// -----------------------------------------------------------------------------
        internal virtual Dictionary<int, TunnelConnection> Connections
        {
            get
            {
                return m_connections;
            }
            set
            {
                m_connections = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Le nom associ�
        /// </summary>
        /// -----------------------------------------------------------------------------
        public string Username
        {
            get
            {
                return m_username;
            }
            set
            {
                m_username = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Utilisateur en mode admin
        /// </summary>
        /// -----------------------------------------------------------------------------
        public bool Admin
        {
            get
            {
                return m_admin;
            }
            set
            {
                m_admin = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// La date de login
        /// </summary>
        /// -----------------------------------------------------------------------------
        public DateTime Logon
        {
            get
            {
                return m_logon;
            }
            set
            {
                m_logon = value;
            }
        }
        #endregion

        #region " M�thodes "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="timeoutdelay">valeur du timeout de session</param>
        /// <param name="connectiontimeoutdelay">valeur du timeout de connexion</param>
        /// -----------------------------------------------------------------------------
        public TunnelSession(int timeoutdelay, int connectiontimeoutdelay)
            : base(timeoutdelay)
        {
            m_connectiontimeoutdelay = connectiontimeoutdelay;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Timeout de l'objet
        /// </summary>
        /// -----------------------------------------------------------------------------
        protected override void Timeout(ILogger logger)
        {
            logger.Log(this, String.Format(Strings.SESSION_TIMEOUT, Username), ESeverity.INFO);
            DisconnectAndRemoveAllConnections();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// V�rification de timeout de l'objet
        /// </summary>
        /// <returns>true en cas de timeout</returns>
        /// -----------------------------------------------------------------------------
        protected override bool CheckTimeout(ILogger logger)
        {
            CheckTimeout(logger, Connections);
            return base.CheckTimeout(logger);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// V�rification de la connexion associ�e � une requ�te
        /// </summary>
        /// <param name="request">la requ�te</param>
        /// <param name="response">la r�ponse � pr�parer</param>
        /// <returns>La connexion si la connexion est valide</returns>
        /// -----------------------------------------------------------------------------
        internal TunnelConnection CheckConnection<I, O>(ref I request, ref O response)
            where I : IConnectionContextRequest
            where O : IConnectionContextResponse
        {
            TunnelConnection connection;
            if (!Connections.TryGetValue(request.Cid, out connection))
            {
                response.Success = false;
                response.Message = Strings.SERVER_SIDE + Strings.CID_NOT_FOUND;
                return null;
            }
            else
            {
                connection.LastAccess = DateTime.Now;
                try
                {
                    response.Connected = (!(connection.TcpClient.Client.Poll(SOCKET_TEST_POLLING_TIME, System.Net.Sockets.SelectMode.SelectRead) && connection.TcpClient.Client.Available == 0));
                    response.DataAvailable = connection.TcpClient.Client.Available > 0;
                }
                catch (Exception)
                {
                    response.Connected = false;
                    response.DataAvailable = false;
                }
                response.Success = true;
                response.Message = string.Empty;
                return connection;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Generation d'un identifiant de connexion unique
        /// </summary>
        /// <returns>un entier unique</returns>
        /// -----------------------------------------------------------------------------
        protected int GetNewCid()
        {
            return Tunnel.GetNewId(Connections);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Ajoute une nouvelle connexion � la table des connexions
        /// </summary>
        /// <param name="connection">la connexion � ajouter</param>
        /// <returns>le jeton de connexion</returns>
        /// -----------------------------------------------------------------------------
        internal int AddConnection(TunnelConnection connection)
        {
            int cid = GetNewCid();
            Connections.Add(cid, connection);
            return cid;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Cr�ation d'une nouvelle connexion
        /// </summary>
        /// <returns>la connexion</returns>
        /// -----------------------------------------------------------------------------
        internal TunnelConnection CreateConnection()
        {
            return new TunnelConnection(m_connectiontimeoutdelay);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Supprime une connexion de table des connexions
        /// </summary>
        /// <param name="cid">le jeton de connexion � supprimer</param>
        /// -----------------------------------------------------------------------------
        public void RemoveConnection(int cid)
        {
            Connections.Remove(cid);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Deconnexion de toutes les connexions sous cette session
        /// </summary>
        /// -----------------------------------------------------------------------------
        public void DisconnectAndRemoveAllConnections()
        {
            foreach (int cid in new ArrayList(Connections.Keys))
            {
                TunnelConnection connection = Connections[cid];
                connection.SafeDisconnect();
                RemoveConnection(cid);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Retourne toutes les connexions sous forme "structure" pour l'export par ex
        /// </summary>
        /// <returns></returns>
        /// -----------------------------------------------------------------------------
        public Connection[] GetConnectionsStruct()
        {
            List<Connection> result = new List<Connection>();

            foreach (int cid in Connections.Keys)
            {
                TunnelConnection connection = Connections[cid];
                Connection export = new Connection();
                export.Cid = cid.ToString("x");
                export.Address = connection.Address;
                export.Host = connection.Host;
                export.Port = connection.Port;
                export.ReadCount = connection.ReadCount;
                export.WriteCount = connection.WriteCount;
                export.LastAccess = connection.LastAccess;
                result.Add(export);
            }

            return result.ToArray();
        }
        #endregion
    }

}
