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
#endregion

namespace Bdt.Shared.Response
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Une r�ponse de connexion
    /// </summary>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public struct ConnectResponse : IConnectionContextResponse 
    {

        #region " Attributs "
        private bool m_success;
        private string m_message;
        private bool m_dataAvailable;
        private bool m_connected;
        private int m_cid;
        #endregion

        #region " Proprietes "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Des donn�es sont-elles disponibles?
        /// </summary>
        /// -----------------------------------------------------------------------------
        public bool DataAvailable
        {
            get
            {
                return m_dataAvailable;
            }
            set
            {
                m_dataAvailable = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// La connexion est-elle effective?
        /// </summary>
        /// -----------------------------------------------------------------------------
        public bool Connected
        {
            get
            {
                return m_connected;
            }
            set
            {
                m_connected = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// La requ�te a aboutie/�chou� ?
        /// </summary>
        /// -----------------------------------------------------------------------------
        public bool Success
        {
            get
            {
                return m_success;
            }
            set
            {
                m_success = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Le message d'information
        /// </summary>
        /// -----------------------------------------------------------------------------
        public string Message
        {
            get
            {
                return m_message;
            }
            set
            {
                m_message = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Le jeton de connexion affect�
        /// </summary>
        /// -----------------------------------------------------------------------------
        public int Cid
        {
            get
            {
                return m_cid;
            }
            set
            {
                m_cid = value;
            }
        }
        #endregion

        #region " Methodes "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="success">La connexion a aboutie/�chou�</param>
        /// <param name="message">Le message d'information</param>
        /// <param name="cid">Le jeton de connexion affect�</param>
        /// -----------------------------------------------------------------------------
        public ConnectResponse(bool success, string message, int cid)
        {
            this.m_connected = false;
            this.m_dataAvailable = false;
            this.m_success = success;
            this.m_message = message;
            this.m_cid = cid;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="success">La requ�te a aboutie/�chou�</param>
        /// <param name="cid">Le jeton de connexion affect�</param>
        /// -----------------------------------------------------------------------------
        public ConnectResponse(bool success, int cid)
        {
            this.m_connected = false;
            this.m_dataAvailable = false;
            this.m_success = success;
            this.m_message = string.Empty;
            this.m_cid = cid;
        }
        #endregion

    }

}


