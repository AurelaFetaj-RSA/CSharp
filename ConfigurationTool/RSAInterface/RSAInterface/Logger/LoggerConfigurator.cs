using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Core;

namespace RSAInterface.Logger
{
    public class LoggerConfigurator
    {
        public Dictionary<IService, Log> Logs = new Dictionary<IService, Log>();

        /// <summary>
        /// in this dictionary, we need to have, service name as string ( key ) and the class as the value
        /// </summary>
        public Dictionary<string, LoggerConfiguration> Configurations = new Dictionary<string, LoggerConfiguration>();
        public string FileName { get; set; }

        public LoggerConfigurator(string configurationFileName) 
        {
            FileName = configurationFileName;
        }

        public LoggerConfigurator()
        {
        }

        public LoggerConfigurator SetAllLogMinimumLevelFileName(Level level)
        {
            foreach (var logConfig in Configurations)
            {
                logConfig.Value.MinLevel = level;
            }

            return this;
        }

        public LoggerConfigurator SetAllLogFileName(string fileLogName)
        {
            foreach (var logConfig in Configurations)
            {
                logConfig.Value.LogFileName = fileLogName;
            }

            return this;
        }

        public LoggerConfigurator Save()
        {
            Helper.Helper.Save(FileName, Configurations);
            return this;
        }

        /// <summary>
        /// This function will create the logger configurator for every IService item.
        /// </summary>
        /// <param name="serviceList"></param>
        /// <returns></returns>
        public LoggerConfigurator CreateDummyLogConfigurations(List<IServiceConfiguration> serviceList, Level level)
        {
            try
            {
                foreach (IServiceConfiguration service in serviceList)
                {
                    if (service.ServiceName == String.Empty)
                        continue;

                    LoggerConfiguration newLoggerConfig = new LoggerConfiguration()
                    {
                        LoggerName = service.ServiceName,
                        ServiceName = service.ServiceName,
                        MinLevel = level,
                    };

                    Configurations[service.ServiceName] = newLoggerConfig;
                }
            }
            catch
            {

            }


            if (FileName == null) 
                throw new ArgumentNullException("This parameter should be different from null on saving logger configurations");

            Helper.Helper.Save(FileName, Configurations);

            return this;
        }

        public LoggerConfigurator CreateDummyLogConfigurations(List<IServiceConfiguration> serviceList)
        {
            return CreateDummyLogConfigurations(serviceList, Level.Info);
        }

        public LoggerConfigurator Load()
        {
            return Load(FileName);
        }

        public LoggerConfigurator Load(string filename)
        {        
            Dictionary<string, LoggerConfiguration> dictionaryToFix = Helper.Helper.Load<Dictionary<string, LoggerConfiguration>>(filename);

            FixErrorOnChangeLog(dictionaryToFix);

            return this;
        }

        private void FixErrorOnChangeLog(Dictionary<string, LoggerConfiguration> dictionaryToFix)
        {
            if (dictionaryToFix == null)
                return;

            foreach(var logConfig in dictionaryToFix)
            {
                Configurations[logConfig.Value.ServiceName] = logConfig.Value;
            }
        }


        public Log CreateLogger(LoggerConfiguration config)
        {
            return new Log().Configure(config);
        }

        public ILog GetLogger(IService serviceToSet)
        {
            foreach(KeyValuePair<string, LoggerConfiguration> configDict in Configurations)
            {
                if(configDict.Value.ServiceName.ToLower().Contains(serviceToSet.Name.ToLower()))
                {
                    return CreateLogger(configDict.Value).Logger;
                }
            }

            return null;
        }


    }
}
