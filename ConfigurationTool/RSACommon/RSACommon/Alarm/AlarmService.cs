using log4net;
using RSACommon.Alarm.Configuration;
using RSACommon.Alarm.Core;
using RSAInterface.Logger;
using RSAInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RSACommon.Alarm
{
    public class AlarmService : IService
    {
        public string Name { get; set; } = "Alarm Service";
        public ILog Log { get; set; } = null;
        public Uri ServiceURI { get; set; } = null;
        public bool IsActive { get; set; } = false;
        public IServiceConfiguration Configuration {get; set;} = null;
        public AlarmConfiguration AlarmConfiguration { get; set; } = null;
        private Alarm.Core.AlarmParser _alarm { get; set; } = null;

        public AlarmService(IServiceConfiguration configuration)
        {
            Name = configuration.ServiceName;
            IsActive = configuration.Active;
            Configuration = configuration;

            if (configuration is AlarmConfiguration config)
            {
                AlarmConfiguration = config;
                _alarm = new Core.AlarmParser(config.Files).Load();
            }


        }

        public List<AlarmStatus> GetStatus(int valueToParse)
        {
            return _alarm.GetAlarms(valueToParse);
        }

        public IService SetLogger(LoggerConfigurator logger)
        {
            Log = logger?.GetLogger(this);

            Log?.Info($"Create the {Name} service");

            return this;
        }

        public async Task<IService> Start()
        {
            Log?.Info($"Starterd the Alarm Service");

            return await Task.Run(async () =>
            {
                await Task.Delay(200);
                return this as IService;
            });
        }

        void IService.Stop()
        {
            Log?.Info($"Stopped the Alarm Service");
        }
    }
}
