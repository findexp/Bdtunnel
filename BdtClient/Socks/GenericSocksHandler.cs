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
using System.Net.Sockets;

using Bdt.Shared.Logs;
using Bdt.Client.Resources;
#endregion

namespace Bdt.Client.Socks
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Gestionnaire g�n�rique Socks
    /// </summary>
    /// -----------------------------------------------------------------------------
    public abstract class GenericSocksHandler : LoggedObject
    {

        #region " Constantes "
        // La taille du buffer d'IO
        public const int BUFFER_SIZE = 32768;
        #endregion

        #region " Attributs "
        private int m_version;
        private int m_command;
        private string m_address;
        private int m_remoteport;
        private byte[] m_buffer;
        #endregion

        #region " Proprietes "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Le handler est-il adapt� � la requ�te?
        /// </summary>
        /// -----------------------------------------------------------------------------
        public abstract bool IsHandled
        {
            get;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Les donn�es de r�ponse
        /// </summary>
        /// -----------------------------------------------------------------------------
        public abstract byte[] Reply
        {
            get;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// La version de la requ�te socks
        /// </summary>
        /// -----------------------------------------------------------------------------
        protected int Version
        {
            get
            {
                return m_version;
            }
            set
            {
                m_version = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// La commande de la requ�te socks
        /// </summary>
        /// -----------------------------------------------------------------------------
        protected int Command
        {
            get
            {
                return m_command;
            }
            set
            {
                m_command = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Le port distant
        /// </summary>
        /// -----------------------------------------------------------------------------
        public int RemotePort
        {
            get
            {
                return m_remoteport;
            }
            protected set
            {
                m_remoteport = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// L'adresse distante
        /// </summary>
        /// -----------------------------------------------------------------------------
        public string Address
        {
            get
            {
                return m_address;
            }
            protected set
            {
                m_address = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Le buffer de la requ�te
        /// </summary>
        /// -----------------------------------------------------------------------------
        protected byte[] Buffer
        {
            get
            {
                return m_buffer;
            }
        }

        #endregion

        #region " M�thodes "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="buffer"></param>
        /// -----------------------------------------------------------------------------
        protected GenericSocksHandler(byte[] buffer)
        {
            m_buffer = buffer;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Retourne un gestionnaire adapt� � la requ�te
        /// </summary>
        /// <param name="client">le client TCP</param>
        /// <returns>un gestionnaire adapt�</returns>
        /// -----------------------------------------------------------------------------
        public static GenericSocksHandler GetInstance(TcpClient client)
        {
            byte[] buffer = new byte[BUFFER_SIZE];

            NetworkStream stream = client.GetStream();
            int size = stream.Read(buffer, 0, BUFFER_SIZE);
            Array.Resize(ref buffer, size);

            if (size < 3)
            {
                throw (new ArgumentException(Strings.INVALID_SOCKS_HANDSHAKE));
            }

            GenericSocksHandler result;
            result = new Socks4Handler(buffer);
            if (!result.IsHandled)
            {
                result = new Socks4AHandler(buffer);
                if (!result.IsHandled)
                {
                    result = new Socks5Handler(client, buffer);
                    if (!result.IsHandled)
                    {
                        throw (new ArgumentException(Strings.NO_VALID_SOCKS_HANDLER));
                    }
                }
            }

            byte[] reply = result.Reply;
            client.GetStream().Write(reply, 0, reply.Length);

            return result;
        }
        #endregion

    }

}


