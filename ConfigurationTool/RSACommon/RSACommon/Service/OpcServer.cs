using Opc.UaFx;
using Opc.UaFx.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RSACommon;
using System.Threading;
using WebApi;
using log4net;
using Microsoft.AspNetCore.Hosting.Server;
using Opc.Ua;
using FutureServerCore.Core.OpcUA.Protocol;
using RSACommon.Configuration;
using RSACommon.Event;
using OpcCustom.OPCLicense;
using RSAInterface.Logger;
using RSAInterface;
using RSAInterface.Helper;

namespace RSACommon.Service
{
    public class OpcServerService : IService
    {
        protected readonly OpcServer _server;
        public string CommunicationString { get; set; } = null;
        public string Name { get; private set; } = "Op Server Default Name";
        public Uri ServiceURI { get; set; } = new UriBuilder().Uri;
        public ILog Log {get; private set;}
        public IServiceConfiguration Configuration { get; private set; }
        public OpcServerConfiguration OpcConfiguration { get; private set; }
        public bool IsActive { get; private set; }
        protected OpcUserNameAcl UserAcl { get; set; }
        protected ILicenseInfo _opcLicenseInfo { get; set; } = null;
        public OpcServerService(IServiceConfiguration config): this(config.ServiceName)
        {
            if (config is OpcServerConfiguration opcServConfig)
            {
                Configuration = opcServConfig;
                ServiceURI = Helper.BuildUri(opcServConfig);
                OpcConfiguration = opcServConfig;
            }

            Name = Configuration.ServiceName;
          
            IsActive = config.Active;

            Opc.UaFx.Server.Licenser.LicenseKey = OpcServerKey.Key;
            _opcLicenseInfo = Opc.UaFx.Server.Licenser.LicenseInfo;

            _server = new OpcServer(ServiceURI);
            UserAcl = _server.Security.UserNameAcl;

            //_server.
            //SetUserAcl();
            SetCallback();
        }

        public OpcServerService AddNodeManager(OpcNodeManager manager)
        {
            _server.NodeManagers.Add(manager);
            return this;
        }


        public OpcServerService(string name)
        {
            Name = name;
        }

        /// <summary>
        /// This function need to be called in constructor, at the beginning, the Logger is not setted
        /// it will not log everything, you can reset after manually.
        /// </summary>
        /// <returns></returns>
        public virtual OpcServerService SetUserAcl()
        {
            /*
             * Commentata perchè prevedeva delle dinamiche custom, ora è stata ereditata e overridata
             * 
            */

            /*
            if(Configurator == null || Configurator.UserList == null || Configurator.UserList.Count == 0)
            {
                Log?.Info($"Problem on configuration file");
                return this;
            }

            foreach(OpcUser user in Configurator.UserList)
            {
                string generatedPwd = PasswordSecurity.DecryptString(user.Password);

                var temp = new SystemIdentity(user.UserName, generatedPwd);

                //Escludo delle variabili per l'utente con privilegi utente
                if (user.UserP == UserPrivilegiesType.User)
                {
                    foreach(var nodeToDeny in M2FNodeManager.DenyUserVariableToWriteList)
                    {
                        temp.Deny(nodeToDeny);
                    }
                }


                UserAcl.AddEntry(temp);
                Log?.Info($"User configured: {user.UserName}{user.UserP}");
            }

            UserAcl.IsEnabled = true;

            // 4. Publish ACL to node manager.
            //M2FNodeManager.AccessControl = UserAcl;

            */

            return this;
        }

        public void Stop()
        {
            Log?.Info($"Server {Name} URI: {ServiceURI.AbsoluteUri} stopped");

            _server.Stop();
            _server.Dispose();
        }

        public async Task<IService> Start()
        {
            await Task.Run(() =>
            {
                Log?.Info($"{ServiceURI.AbsoluteUri} started");
                _server.Start();
            });

            return this;
        }



        public virtual void SetCallback()
        {
            var autoSessions = _server.GetSessions();
            _server.Started += _server_Started;
        }

        private void _server_Started(object sender, EventArgs e)
        {
            Log?.Info($"OPC Server is started {DateTime.Now}");
        }

        private void Server_SessionActivated(object sender, OpcSessionEventArgs e)
        {
            Log?.Info($"{DateTime.Now} {e.Session.Name}");
        }

        protected virtual void Server_Started(object sender, EventArgs e)
        {
            string toAdd = "";

            if (_opcLicenseInfo.ExpiryDate != null)
            {
                toAdd = (_opcLicenseInfo.ExpiryDate != null) ? $", OPCServerLicense Expire on {_opcLicenseInfo.ExpiryDate}" : "";
            }             

            Log?.Info($"{DateTime.Now} started{toAdd}");
        }


        public virtual IService SetLogger(LoggerConfigurator loggerConfig)
        {
            Log = loggerConfig?.GetLogger(this);

            Log?.Info($"Create the {Name} at {ServiceURI.AbsoluteUri}");

            return this;
        }

    }
}
