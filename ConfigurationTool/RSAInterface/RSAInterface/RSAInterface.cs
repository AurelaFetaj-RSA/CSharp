using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSAInterface
{
    public class CoreConfigurations
    {
        public List<IServiceConfiguration> ServiceConfigurations = new List<IServiceConfiguration>();
    }

    public interface IServiceConfiguration
    {
        string ServiceName { get; set; }
        string Scheme { get; set; }
        bool Active { get; set; }
    }

    public interface IServiceConfigurationRemote : IServiceConfiguration
    {
        int Port { get; set; }
        string Host { get; set; }
    }
    
    public interface IService
    {
        string Name { get; }
        Task<IService> Start();
        void Stop();
        ILog Log { get; }
        IService SetLogger(RSAInterface.Logger.LoggerConfigurator logger);
        Uri ServiceURI { get; }
        bool IsActive { get; }
        IServiceConfiguration Configuration { get; }
    }



}
