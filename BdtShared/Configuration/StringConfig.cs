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

namespace Bdt.Shared.Configuration
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Repr�sente une source de configuration bas�e sur une ligne de commande
    /// </summary>
    /// -----------------------------------------------------------------------------
    public sealed class StringConfig : BaseConfig
    {

        #region " Attributs "
        private string[] m_args; //Les arguments de la ligne de commande
        #endregion

        #region " Propri�t�s "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Retourne/Fixe les arguments de la ligne de commande
        /// </summary>
        /// <returns>les arguments de la ligne de commande</returns>
        /// -----------------------------------------------------------------------------
        public string[] Args
        {
            get
            {
                return m_args;
            }
            set
            {
                m_args = value;
            }
        }
        #endregion

        #region " M�thodes "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Cr�ation d'une source de donn�e bas�e sur la ligne de commande
        /// </summary>
        /// <param name="args">les arguments de la ligne de commande</param>
        /// <param name="priority">la priorit� de cette source (la plus basse=prioritaire)</param>
        /// -----------------------------------------------------------------------------
        public StringConfig(string[] args, int priority)
            : base(priority)
        {
            this.Args = args;
            Rehash();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Force le rechargement de la source de donn�e
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override void Rehash()
        {
            foreach (string arg in Args)
            {
                int equalIndex = arg.IndexOf(SOURCE_ITEM_EQUALS);
                if ((equalIndex >= 0) && equalIndex + 1 < arg.Length)
                {
                    this.SetValue(arg.Substring(0, equalIndex), arg.Substring(equalIndex + 1));
                }
            }
        }
        #endregion

    }

}


