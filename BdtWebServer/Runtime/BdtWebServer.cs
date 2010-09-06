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
using System.IO;
using System.Web;

using Bdt.Server.Service;
using Bdt.Server.Runtime;
using Bdt.Shared.Configuration;
using Bdt.Shared.Logs;
using Bdt.Shared.Runtime;
#endregion

namespace Bdt.WebServer.Runtime
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Programme c�t� serveur du tunnel de communication
    /// </summary>
    /// -----------------------------------------------------------------------------
    public class BdtWebServer : Program
    {

        #region " Attributs "
        HttpServerUtility m_server;
        #endregion

        #region " Proprietes "
        public override string ConfigFile
        {
            get
            {
                return m_server.MapPath(string.Format("App_Data" + Path.DirectorySeparatorChar + "{0}Cfg.xml", typeof(BdtServer).Assembly.GetName().Name));
            }
        }
        #endregion
        
        #region " M�thodes "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="server">l'utilitaire serveur (mappage)</param>
        /// -----------------------------------------------------------------------------
        public BdtWebServer(HttpServerUtility server)
            : base()
        {
            m_server = server;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Fixe la culture courante
        /// </summary>
        /// <param name="name">le nom de la culture</param>
        /// -----------------------------------------------------------------------------
        public override void SetCulture(String name)
        {
            base.SetCulture(name);
            if ((name != null) && (name != String.Empty))
            {
                Bdt.Server.Resources.Strings.Culture = new CultureInfo(name);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Chargement des donn�es de configuration
        /// </summary>
        /// <param name="args">Arguments de la ligne de commande</param>
        /// -----------------------------------------------------------------------------
        public override void LoadConfiguration(string[] args)
        {
            m_args = args;

            LoggedObject.GlobalLogger = CreateLoggers();
            Log(Bdt.Shared.Resources.Strings.LOADING_CONFIGURATION, ESeverity.DEBUG);
            SharedConfig cfg = new SharedConfig(m_config);
            // unneeded in IIs web hosting model, see web.config
            // m_protocol = GenericProtocol.GetInstance(cfg);
            SetCulture(cfg.ServiceCulture);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// D�marrage du serveur
        /// </summary>
        /// -----------------------------------------------------------------------------
        public void Start()
        {
            LoadConfiguration(new String[] { });

            Log(string.Format(Bdt.Server.Resources.Strings.SERVER_TITLE, this.GetType().Assembly.GetName().Version.ToString(3)), ESeverity.INFO);
            Log(Program.FrameworkVersion(), ESeverity.INFO);

            Tunnel.Configuration = m_config;
            Tunnel.Logger = LoggedObject.GlobalLogger;

            // unneeded in IIs web hosting model, see web.config
            // server.Protocol.ConfigureServer(typeof(Tunnel));
            Log(Bdt.Server.Resources.Strings.SERVER_STARTED, ESeverity.INFO);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Initialisation des loggers
        /// </summary>
        /// <returns>un MultiLogger li� � une source fichier et console</returns>
        /// -----------------------------------------------------------------------------
        public override BaseLogger CreateLoggers()
        {
            XMLConfig xmlConfig = new XMLConfig(ConfigFile, 1);
            m_config = new ConfigPackage();
            m_config.AddSource(xmlConfig);

            // Map the path to the current Web Application
            String key = CFG_FILE + Bdt.Shared.Configuration.BaseConfig.SOURCE_ITEM_ATTRIBUTE + FileLogger.CONFIG_FILENAME;
            String filename = xmlConfig.Value(key, null);
            if ((filename != null) && (!Path.IsPathRooted(filename))) {
                xmlConfig.SetValue(key, m_server.MapPath("App_Data" + Path.DirectorySeparatorChar + filename)); 
            }

            MultiLogger log = new MultiLogger();
            m_consoleLogger = new ConsoleLogger(CFG_CONSOLE, m_config);
            m_fileLogger = new FileLogger(CFG_FILE, m_config);
            log.AddLogger(m_consoleLogger);
            log.AddLogger(m_fileLogger);

            return log;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Arr�t du serveur
        /// </summary>
        /// -----------------------------------------------------------------------------
        public void Stop()
        {
            Tunnel.DisableChecking();
            Protocol.UnConfigureServer();
            UnLoadConfiguration();
        }
        #endregion

    }
}
