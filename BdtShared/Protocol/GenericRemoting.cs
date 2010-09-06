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
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;

using Bdt.Shared.Resources;
using Bdt.Shared.Service;
using Bdt.Shared.Logs;
using System.Collections;
#endregion

namespace Bdt.Shared.Protocol
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Protocole de communication bas� sur le remoting .NET
    /// </summary>
    /// -----------------------------------------------------------------------------
    public abstract class GenericRemoting<T> : GenericProtocol where T: IChannel
    {

        #region " Constantes "
        public const string CFG_NAME = "name";
        public const string CFG_PORT_NAME = "portName";
        public const string CFG_PORT = "port";
        #endregion

        #region " Attributs "
        protected T m_clientchannel;
        protected T m_serverchannel;
        #endregion

        #region " Proprietes "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Le canal de communication c�t� serveur
        /// </summary>
        /// -----------------------------------------------------------------------------
        protected abstract T ServerChannel
        {
            get;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Le canal de communication c�t� client
        /// </summary>
        /// -----------------------------------------------------------------------------
        public abstract T ClientChannel
        {
            get;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// L'URL n�cessaire pour se connecter au serveur
        /// </summary>
        /// -----------------------------------------------------------------------------
        protected abstract string ServerURL
        {
            get;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Le service est-il s�curis�
        /// </summary>
        /// -----------------------------------------------------------------------------
        protected virtual bool IsSecured
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region " M�thodes "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Configuration c�t� client
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override void ConfigureClient()
        {
            Log(string.Format(Strings.CONFIGURING_CLIENT, this.GetType().Name, ServerURL), ESeverity.DEBUG);
            ChannelServices.RegisterChannel(ClientChannel, IsSecured);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// D�-configuration c�t� client
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override void UnConfigureClient()
        {
            Log(string.Format(Strings.UNCONFIGURING_CLIENT, this.GetType().Name), ESeverity.DEBUG);
            ChannelServices.UnregisterChannel(ClientChannel);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Configuration c�t� serveur
        /// </summary>
        /// <param name="type">le type d'objet � rendre distant</param>
        /// -----------------------------------------------------------------------------
        public override void ConfigureServer(Type type)
        {
            Log(string.Format(Strings.CONFIGURING_SERVER, this.GetType().Name, Port), ESeverity.INFO);
            ChannelServices.RegisterChannel(ServerChannel, IsSecured);
            WellKnownServiceTypeEntry wks = new WellKnownServiceTypeEntry(type, Name, WellKnownObjectMode.Singleton);
            RemotingConfiguration.RegisterWellKnownServiceType(wks);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// D�configuration c�t� serveur
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override void UnConfigureServer()
        {
            Log(string.Format(Strings.UNCONFIGURING_SERVER, this.GetType().Name, Port), ESeverity.INFO);
            ChannelServices.UnregisterChannel(ServerChannel);
            if (ServerChannel is IChannelReceiver)
            {
                ((IChannelReceiver)ServerChannel).StopListening(null);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Retourne une instance de tunnel
        /// </summary>
        /// <returns>une instance de tunnel</returns>
        /// -----------------------------------------------------------------------------
        public override Service.ITunnel GetTunnel()
        {
            return ((ITunnel)Activator.GetObject(typeof(ITunnel), ServerURL));
        }

        public virtual Hashtable CreateClientChannelProperties()
        {
            Hashtable properties = new Hashtable();
            properties.Add(CFG_NAME, string.Format("{0}.Client", Name));
            properties.Add(CFG_PORT_NAME, properties[CFG_NAME]);
            return properties;
        }

        public virtual Hashtable CreateServerChannelProperties()
        {
            Hashtable properties = new Hashtable();
            properties.Add(CFG_NAME, Name);
            properties.Add(CFG_PORT, Port.ToString());
            properties.Add(CFG_PORT_NAME, properties[CFG_NAME]);
            return properties;
        }

        #endregion

    }

}

