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

using Bdt.Shared.Logs;
using Bdt.Client.Resources;
#endregion

namespace Bdt.Client.Socks
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Gestionnaire Socks v4 (sans DNS)
    /// </summary>
    /// -----------------------------------------------------------------------------
    public class Socks4Handler : GenericSocksHandler
    {

        #region " Constantes "
        public const int SOCKS4_REPLY_VERSION = 0;
        public const int SOCKS4_OK = 90; // Requ�te accept�e
        public const int SOCKS4_KO = 91; // Requ�te refus�e
        public const int SOCKS4_CONNECT_COMMAND = 1; // commande CONNECT
        public const int SOCKS4_BIND_COMMAND = 2; // commande BIND (non support�e)
        public const int REPLY_SIZE = 8; // octets de r�ponse
        #endregion

        #region " Attributs "
        protected byte[] m_reply = new byte[REPLY_SIZE];
        #endregion

        #region " Proprietes "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Le handler est-il adapt� � la requ�te?
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override bool IsHandled
        {
            get
            {
                // Pr�paration pessimiste de la r�ponse
                m_reply[0] = SOCKS4_REPLY_VERSION;
                m_reply[1] = SOCKS4_KO;
                Array.Clear(m_reply, 2, 6);

                if (Version != 4)
                {
                    return false;
                }

                // Teste pour basic Socks4 (pas Socks4a)
                if ((Buffer[4] != 0) || (Buffer[5] != 0) || (Buffer[6] != 0))
                {
                    if (Command != SOCKS4_BIND_COMMAND)
                    {
                        RemotePort = 256 * Convert.ToInt32(Buffer[2]) + Convert.ToInt32(Buffer[3]);
                        Address = Buffer[4] + "." + Buffer[5] + "." + Buffer[6] + "." + Buffer[7];
                        // Pr�paration de la r�ponse
                        m_reply[1] = SOCKS4_OK;
                        Array.Copy(Buffer, 2, m_reply, 2, 6);
                        Log(Strings.SOCKS4_REQUEST_HANDLED, ESeverity.DEBUG);
                        return true;
                    }
                    else
                    {
                        // Socks4 BIND
                        Log(Strings.SOCKS_BIND_UNSUPPORTED, ESeverity.WARN);
                    }
                }
                return false;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Les donn�es de r�ponse
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override byte[] Reply
        {
            get
            {
                return m_reply;
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
        public Socks4Handler(byte[] buffer)
            : base(buffer)
        {
            Version = buffer[0];
            Command = buffer[1];
        }
        #endregion

    }
}

