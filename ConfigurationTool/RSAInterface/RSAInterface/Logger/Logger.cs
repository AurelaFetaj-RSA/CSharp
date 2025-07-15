using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace RSAInterface.Logger
{
    public class LoggerConfiguration
    {
        public string ConversionPattern { get; set; } = "%d{MM-dd HH:mm:ss} [%-5p]  [%logger{1}]  %m%n";
        public string LoggerName { get; set; } = "DefaultLogger";
        public string Folder { get; set; } = "";
        public string LogFileName { get; set; } = "Logger.log";
        public Level MinLevel { get; set; } = Level.Info;
        public bool ShowOnConsole { get; set; } = true;
        public string ServiceName { get; set; } = "DefaultServiceName";
    }

    public class Log
    {

        public string Name { get; set; }
        public ILog Logger => LogManager.GetLogger(Name);
        public LoggerConfiguration LConfig { get; set; }    

        /// <summary>
        /// we need to configure the logger before use
        /// </summary>
        /// <param name="logInfo"></param>
        public Log Configure(LoggerConfiguration logConfiguration)
        {
            LConfig = logConfiguration;

            Name = logConfiguration.LoggerName;
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Root.RemoveAllAppenders(); /*Remove any other appenders*/

            FileAppender fileAppender = new FileAppender();
            fileAppender.File = Path.Combine(logConfiguration.Folder, logConfiguration.LogFileName);
            fileAppender.AppendToFile = true;

            PatternLayout pl = new PatternLayout();
            pl.ConversionPattern = logConfiguration.ConversionPattern;
            pl.ActivateOptions();

            fileAppender.Layout = pl;
            fileAppender.Name = logConfiguration.LoggerName;
            fileAppender.Threshold = logConfiguration.MinLevel;

            fileAppender.ActivateOptions();

            List<IAppender> appenders = new List<IAppender>() { fileAppender };

            if (logConfiguration.ShowOnConsole)
            {
                ConsoleAppender consoleAppender = new ConsoleAppender();
                consoleAppender.Layout = pl;
                consoleAppender.ActivateOptions();
                appenders.Add(consoleAppender);
            }

            BasicConfigurator.Configure(appenders.ToArray());

            return this;
        }
    }
}
