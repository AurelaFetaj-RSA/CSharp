using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using RSACommon.WebApiDefinitions;
using RSAInterface.Logger;
using log4net;
using RSACommon;
using RSAInterface;
using RSAInterface.Helper;

namespace WebApi
{
    public class WebApiCoreConfiguration : IServiceConfigurationRemote
    {
        public string ServiceName { get ; set ; } = "Web Api Future";
        public int Port { get; set; }
        public string Host { get; set; }
        public string Scheme { get; set; }
        public bool Active { get; set; } = false;

        public string FolderForRecipe { get; set; } = string.Empty;
        //public Type ServiceType => _serviceType;

        //private Type _serviceType = typeof(WebApiCoreConfiguration);
    }


    public class WebApiCore : IService
    {
        public int Port { get => ServiceURI.Port;}
        public string Host { get => ServiceURI.Host; }
        public string Name {get; set;} = "Web Api Future";
        public string Scheme { get; set; } = "Web Api Future";
        public IServiceConfiguration Configuration { get; private set;}
        public Uri ServiceURI { get; set; } = new UriBuilder().Uri;
        public static WebApiSharedList SharedList;
        public bool IsActive { get; private set; }
        public ILog Log { get; private set; }


        private IDisposable webApp;

        public WebApiCore(IServiceConfiguration config, WebApiSharedList sHlist): this(sHlist, config.ServiceName)
        {
            if(config is WebApiCoreConfiguration webApiConf)
            {
                Configuration = webApiConf;

                ServiceURI = Helper.BuildUri(webApiConf);
                IsActive = config.Active;
            }

        }

        /// <summary>
        /// example web api
        /// </summary>
        public WebApiCore(WebApiSharedList sHlist, string name)
        {
            Name = name;
            SharedList = sHlist != null ? sHlist : new WebApiSharedList();
        }

        public async Task<IService> Start()
        {
            await Task.Run(() =>
            {
                webApp = WebApp.Start<WebApiConfigurator>(url: ServiceURI.AbsoluteUri);

                Log?.Info($"Started the {Name} at {ServiceURI.AbsoluteUri} ");
            });

            return this;
        }

        public void Stop()
        {
            Log?.Info($"Stopped the {Name}");
            webApp.Dispose();
        }

        public virtual IService SetLogger(LoggerConfigurator loggerConfig)
        {
            Log = loggerConfig?.GetLogger(this);

            Log?.Info($"Create the {Name} at {ServiceURI.AbsoluteUri}");

            return this;
        }
    }
}
