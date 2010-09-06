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
using System.Globalization;
using Bdt.Shared.Resources;
using Bdt.Shared.Logs;
using Bdt.Shared.Configuration;
using Bdt.Shared.Protocol;
#endregion

namespace Bdt.Shared.Runtime
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Une �bauche de programme
    /// </summary>
    /// -----------------------------------------------------------------------------
    public abstract class Program : Bdt.Shared.Logs.LoggedObject
    {

        #region " Constantes "
        protected const string CFG_LOG = SharedConfig.WORD_LOGS + SharedConfig.TAG_ELEMENT;
        protected const string CFG_CONSOLE = CFG_LOG + SharedConfig.WORD_CONSOLE;
        protected const string CFG_FILE = CFG_LOG + SharedConfig.WORD_FILE;
        #endregion

        #region " Attributs "
        protected ConfigPackage m_config;
        protected GenericProtocol m_protocol;
        protected BaseLogger m_consoleLogger;
        protected FileLogger m_fileLogger;
        protected string[] m_args;
        #endregion

        #region " Proprietes "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Le protocole de communication
        /// </summary>
        /// -----------------------------------------------------------------------------
        public virtual GenericProtocol Protocol
        {
            get
            {
                return m_protocol;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Le protocole de communication
        /// </summary>
        /// -----------------------------------------------------------------------------
        public virtual ConfigPackage Configuration
        {
            get
            {
                return m_config;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Le fichier de configuration
        /// </summary>
        /// -----------------------------------------------------------------------------
        public virtual string ConfigFile
        {
            get
            {
                return string.Format("{0}Cfg.xml", this.GetType().Assembly.GetName().Name);
            }
        }
        #endregion

        #region " M�thodes "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Chargement des donn�es de configuration
        /// </summary>
        /// -----------------------------------------------------------------------------
        public void LoadConfiguration()
        {
            LoadConfiguration(m_args);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Initialisation des loggers
        /// </summary>
        /// <returns>un MultiLogger li� � une source fichier et console</returns>
        /// -----------------------------------------------------------------------------
        public virtual BaseLogger CreateLoggers ()
        {
            StringConfig ldcConfig = new StringConfig(m_args, 0);
            XMLConfig xmlConfig = new XMLConfig(ConfigFile, 1);
            m_config = new ConfigPackage();
            m_config.AddSource(ldcConfig);
            m_config.AddSource(xmlConfig);

            MultiLogger log = new MultiLogger();
            m_consoleLogger = new ConsoleLogger(CFG_CONSOLE, m_config);
            m_fileLogger = new Bdt.Shared.Logs.FileLogger(CFG_FILE, m_config);
            log.AddLogger(m_consoleLogger);
            log.AddLogger(m_fileLogger);

            return log;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Chargement des donn�es de configuration
        /// </summary>
        /// <param name="args">Arguments de la ligne de commande</param>
        /// -----------------------------------------------------------------------------
        public virtual void LoadConfiguration(string[] args)
        {
            m_args = args;

            LoggedObject.GlobalLogger = CreateLoggers();
            Log(Strings.LOADING_CONFIGURATION, ESeverity.DEBUG);
            SharedConfig cfg = new SharedConfig(m_config);
            m_protocol = GenericProtocol.GetInstance(cfg);
            SetCulture(cfg.ServiceCulture);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Fixe la culture courante
        /// </summary>
        /// <param name="name">le nom de la culture</param>
        /// -----------------------------------------------------------------------------
        public virtual void SetCulture(String name)
        {
            if ((name != null) && (name != String.Empty))
            {
                Bdt.Shared.Resources.Strings.Culture = new CultureInfo(name);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// D�chargement des donn�es de configuration
        /// </summary>
        /// -----------------------------------------------------------------------------
        public virtual void UnLoadConfiguration()
        {
            Log(Strings.UNLOADING_CONFIGURATION, ESeverity.DEBUG);

            if (m_consoleLogger != null)
            {
                m_consoleLogger.Close();
                m_consoleLogger = null;
            }

            if (m_fileLogger != null)
            {
                m_fileLogger.Close();
                m_fileLogger = null;
            }

            LoggedObject.GlobalLogger = null;
            m_config = null;
            m_protocol = null;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Affiche le nom et la version du framework utilis�
        /// </summary>
        /// -----------------------------------------------------------------------------
        public static string FrameworkVersion()
        {
            string plateform = (Type.GetType("Mono.Runtime", false) == null) ? ".NET" : "Mono";
            return string.Format(Strings.POWERED_BY, plateform, System.Environment.Version);
        }

        /*
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Encodeur simple par Xor pour un tableau d'octets
        /// </summary>
        /// <param name="bytes">Le tableau � encoder/d�coder (xor r�versible)</param>
        /// <param name="seed">La racine d'initialisation du g�n�rateur al�atoire</param>
        /// -----------------------------------------------------------------------------
        public static void RandomXorEncoder (ref byte[] bytes, int seed)
        {
            Random rnd = new Random(seed);
            if (bytes!=null)
            {
                for (int i = 0; i <= bytes.Length - 1; i++)
                {
                    bytes[i] = (byte) (bytes[i] ^ Convert.ToByte(Math.Abs(rnd.Next() % 256)));
                }
            }
        }
        */

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Encodeur simple par Xor pour un tableau d'octets
        /// </summary>
        /// <param name="bytes">Le tableau � encoder/d�coder (xor r�versible)</param>
        /// <param name="key">La clef de codage</param>
        /// -----------------------------------------------------------------------------
        public static void StaticXorEncoder(ref byte[] bytes, int key)
        {
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = (byte)(bytes[i] ^ Convert.ToByte(key % 256));
                }
            }
        }
        #endregion

    }

}

