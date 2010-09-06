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
    /// Gestionnaire Socks v4A (avec DNS)
    /// </summary>
    /// -----------------------------------------------------------------------------
    public class Socks4AHandler : Socks4Handler
    {

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

                // Test du Socks4a
                if ((Buffer[4] == 0) && (Buffer[5] == 0) && (Buffer[6] == 0))
                {
                    if (Command != SOCKS4_BIND_COMMAND)
                    {
                        RemotePort = 256 * Convert.ToInt32(Buffer[2]) + Convert.ToInt32(Buffer[3]);
                        int position = -1;
                        for (int i = 8; i <= Buffer.Length - 1; i++)
                        {
                            if (Buffer[i] == 0)
                            {
                                position = i;
                                break;
                            }
                        }
                        if (position >= 0)
                        {
                            Address = new string(System.Text.Encoding.ASCII.GetChars(Buffer), position + 1, Buffer.Length - position - 2);
                        }
                        else
                        {
                            Address = string.Empty;
                        }

                        // Pr�paration de la r�ponse
                        m_reply[1] = SOCKS4_OK;
                        Array.Copy(Buffer, 2, m_reply, 2, 2);
                        Array.Clear(m_reply, 4, 3);
                        m_reply[7] = Buffer[7];
                        Log(Strings.SOCKS4A_REQUEST_HANDLED, ESeverity.DEBUG);
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
        #endregion

        #region " M�thodes "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="buffer"></param>
        /// -----------------------------------------------------------------------------
        public Socks4AHandler(byte[] buffer)
            : base(buffer)
        {
        }
        #endregion

    }
}
