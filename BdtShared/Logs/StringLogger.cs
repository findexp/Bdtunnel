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
using System.IO;
using System.Text;

using Bdt.Shared.Configuration;
#endregion

namespace Bdt.Shared.Logs
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// G�n�ration des logs dans une chaine
    /// </summary>
    /// -----------------------------------------------------------------------------
    public sealed class StringLogger : BaseLogger
    {

        #region " Attributs "
        private string m_lastline = string.Empty;
        #endregion

        #region " Proprietes "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Retourne le texte complet logg� dans cet objet
        /// </summary>
        /// -----------------------------------------------------------------------------
        public string Text
        {
            get
            {
                return ((StringWriter)m_writer).GetStringBuilder().ToString();
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Retourne la derni�re ligne de log
        /// </summary>
        /// -----------------------------------------------------------------------------
        public string LastLine
        {
            get
            {
                return m_lastline;
            }
        }
        #endregion

        #region " M�thodes "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Constructeur pour un log � partir des donn�es fournies dans une configuration
        /// </summary>
        /// <param name="prefix">le prefixe dans la configuration ex: application/log</param>
        /// <param name="config">la configuration pour la lecture des parametres</param>
        /// -----------------------------------------------------------------------------
        public StringLogger(string prefix, Bdt.Shared.Configuration.ConfigPackage config)
            : base(new StringWriter(), prefix, config)
        {
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Constructeur pour un log
        /// </summary>
        /// <param name="dateFormat">le format des dates de timestamp</param>
        /// <param name="filter">le niveau de filtrage pour la sortie des logs</param>
        /// -----------------------------------------------------------------------------
        public StringLogger(string dateFormat, ESeverity filter)
            : base(new StringWriter(), dateFormat, filter)
        {
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Ecriture d'une entr�e de log. Ne sera pas prise en compte si le log est inactif
        /// ou si le filtre l'impose
        /// </summary>
        /// <param name="sender">l'emetteur</param>
        /// <param name="message">le message � logger</param>
        /// <param name="severity">la s�v�rit�</param>
        /// -----------------------------------------------------------------------------
        public override void Log(object sender, string message, ESeverity severity)
        {
            StringBuilder sb = ((StringWriter)m_writer).GetStringBuilder();
            int index = sb.Length;
            base.Log(sender, message, severity);
            if ((m_enabled) && (severity >= m_filter))
            {
                m_lastline = sb.ToString(index, sb.Length - index);
            }
        }
        #endregion

    }

}
