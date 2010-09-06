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

using Bdt.Shared.Configuration;
#endregion

namespace Bdt.Shared.Logs
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// G�n�ration des logs dans un fichier
    /// </summary>
    /// -----------------------------------------------------------------------------
    public class FileLogger : BaseLogger
    {

        #region " Constantes "
        public const string CONFIG_APPEND = "append";
        public const string CONFIG_FILENAME = "filename";
        #endregion

        #region " Attributs "
        protected string m_filename = null;
        protected bool m_append = false;
        #endregion

        #region " Propri�t�s "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Retourne le nom du fichier utilis� pour l'�criture des logs
        /// </summary>
        /// <returns>le nom du fichier utilis� pour l'�criture des logs</returns>
        /// -----------------------------------------------------------------------------
        public string Filename
        {
            get
            {
                return m_filename;
            }
            protected set
            {
                m_filename = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Retourne l'�tat indiquant si les donn�es doivent �tre ajout�es au fichier
        /// </summary>
        /// <returns>l'�tat indiquant si les donn�es doivent �tre ajout�es au fichier</returns>
        /// -----------------------------------------------------------------------------
        public bool Append
        {
            get
            {
                return m_append;
            }
            protected set
            {
                m_append = value;
            }
        }
        #endregion

        #region " M�thodes "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Constructeur pour un log vierge
        /// </summary>
        /// -----------------------------------------------------------------------------
        protected FileLogger ()
        {
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Constructeur pour un log � partir des donn�es fournies dans une configuration
        /// </summary>
        /// <param name="prefix">le prefixe dans la configuration ex: application/log</param>
        /// <param name="config">la configuration pour la lecture des parametres</param>
        /// -----------------------------------------------------------------------------
        public FileLogger(string prefix, ConfigPackage config)
            : base(null, prefix, config)
        {
            m_filename = config.Value(prefix + Bdt.Shared.Configuration.BaseConfig.SOURCE_ITEM_ATTRIBUTE + CONFIG_FILENAME, m_filename);
            m_append = config.ValueBool(prefix + Bdt.Shared.Configuration.BaseConfig.SOURCE_ITEM_ATTRIBUTE + CONFIG_APPEND, m_append);
            if (Enabled)
            {
                m_writer = new StreamWriter(m_filename, m_append, System.Text.Encoding.Default);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Constructeur pour un log
        /// </summary>
        /// <param name="filename">le nom du fichier dans lequel �crire</param>
        /// <param name="append">si false la fichier sera �cras�</param>
        /// <param name="dateFormat">le format des dates de timestamp</param>
        /// <param name="filter">le niveau de filtrage pour la sortie des logs</param>
        /// -----------------------------------------------------------------------------
        public FileLogger(string filename, bool append, string dateFormat, ESeverity filter)
            : base(new StreamWriter(filename, append, System.Text.Encoding.Default), dateFormat, filter)
        {
            m_filename = filename;
            m_append = append;
        }
        #endregion

    }

}

