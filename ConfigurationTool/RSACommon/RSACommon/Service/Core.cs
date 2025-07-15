using Diagnostic.Configuration;
using log4net;
using Robot;
using RSACommon.Configuration;
using RSAInterface.Logger;
using RSACommon.Service;
using RSAInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using WebApi;

namespace RSACommon
{
    public class Core : IService
    {
        public WebApiSharedList ApiSharedList { get; set; }
        public List<IService> ServiceList { get; private set; } = new List<IService>() {};
        public List<Type> ServiceTypeList { get; private set; } = new List<Type>() { };
        public readonly string ConfigFile = "config.json";
        public CoreConfigurationsBuilder CoreBuilder;
        public CoreConfigurations CoreConfigurations { get; private set; }
        public string Name { get; private set; } = "FSCore default name";
        public ILog Log { get; private set; }
        public Uri ServiceURI { get => new UriBuilder().Uri; }
        public bool IsActive { get; private set; }
        public Diagnostic.Core.Diagnostic DiagnosticService { get; private set; }
        public static EventHandler CoreIsStarted = null;
        public bool DiagnosticIsActive
        {
            get
            {
                if (DiagnosticService != null && DiagnosticService.IsActive)
                    return true;
                else
                    return false;
            }
        }
        public IRobot<IRobotVariable> Robot { get; private set; }
        public IServiceConfiguration Configuration { get; private set; }

        public Core(string name, List<IService> listOfService = null)
        {
            Name = name;

            ServiceList = listOfService;

            if (ServiceList == null)
                ServiceList = new List<IService>();

            CoreBuilder = new CoreConfigurationsBuilder(ConfigFile, ".\\");

            if(!File.Exists(ConfigFile))
            {
                SaveConfigurationFileDefault();
            }
        }

        public bool TryFindPerType<T>(out List<T> outputService) where T: IService
        {
            var listOfService = ServiceList.FindAll(x => x.GetType() == typeof(T));

            outputService = new List<T>();

            if (listOfService == null || listOfService.Count == 0)
                return false;

            foreach (IService service in listOfService)
            {
                if (service is T toAdd)
                    outputService.Add(toAdd);

            }

            return true;
        }


        public List<IService> FindPerType(Type tp)
        {
            return ServiceList.FindAll(x => x.GetType() == tp);
        }


        private async Task<IService> StartAllService()
        {
            Log?.Info("Starting the service");

            foreach (IService service in ServiceList)
            {
                if (service is Core)
                    continue;

                if (service.IsActive)
                {
                    await service.Start();
                }
            }

            return this;
        }
        public IService StopAllService()
        {
            foreach (IService service in ServiceList)
            {
                if (service is Core)
                    continue;

                service.Stop();
            }

            return this;
        }

        /// <summary>
        /// Add a singleton service, don't need to be initialized
        /// </summary>
        /// <param name="service"></param>
        public void AddSingletonService(IService service)
        {
            if(!ServiceList.Any(x => Type.Equals(x, service)))
            {
                ServiceList.Add(service);
            }
        }

        public void AddScoped(IService service)
        {
            ServiceList.Add(service);
        }

        public void AddScoped<T>()
        {
            ServiceTypeList.Add(typeof(T));
        }

        public bool IsInServiceTypeList<T>()
        {
            if (ServiceType<T>() != null)
                return true;

            return false;
        }

        public Type ServiceType<T>()
        {
            //bool test = false;

            foreach (Type objectType in ServiceTypeList)
            {
                //test = objectType.GetType().GetInterfaces().Any(x =>  x.IsGenericType &&  x.GetGenericTypeDefinition() == typeof(T));
                var generciType = objectType.BaseType;

                if (objectType.IsSubclassOf(typeof(T)) || objectType.IsAssignableFrom(typeof(T)))
                {
                    return objectType;
                }
            }

            return null;
        }

        public IService CreateServiceFromServiceTypeList<T>(IServiceConfiguration confg)
        {
            Type serviceType = ServiceType<T>();

            if (serviceType == null)
                return null;

            IService serviceCreated = (IService)Activator.CreateInstance(serviceType, confg);
            return serviceCreated;
        }

        public List<IService> CreateServiceList(CoreConfigurations config, LoggerConfigurator loggerCon)
        {
            Log?.Info("configure core");
            ServiceList = new List<IService>() { SetLogger(loggerCon) };

            //I will add the logger at the Core class instance
            IService serv;

            if (config == null)
                return null;


            foreach (IServiceConfiguration serviceConf in config.ServiceConfigurations)
            {
                if (serviceConf is WebApiCoreConfiguration webApiConf && IsInServiceTypeList<WebApiCore>())
                {

                    Log?.Info("configure webapi");

                    ServiceList.Add(new WebApiCore(webApiConf, ApiSharedList)
                    {
                        //ServiceURI = new UriBuilder(webApiConf.Scheme, webApiConf.Host, webApiConf.Port).Uri,
                    }.SetLogger(loggerCon));
                }
                else if (serviceConf is OpcServerConfiguration opcServerC && IsInServiceTypeList<OpcServerService>())
                {
                    Log?.Info("configure opc server");

                    if((serv = CreateServiceFromServiceTypeList<OpcServerService>(opcServerC)) != null)
                    {
                        ServiceList.Add(serv.SetLogger(loggerCon));
                    }
                }
                else if (serviceConf is Alarm.Configuration.AlarmConfiguration alarmConf && IsInServiceTypeList<Alarm.AlarmService>())
                {
                    Log?.Info("configure Alarm server");

                    if ((serv = CreateServiceFromServiceTypeList<Alarm.AlarmService>(alarmConf)) != null)
                    {
                        ServiceList.Add(serv.SetLogger(loggerCon));
                    }
                }
                else if (serviceConf is RobotConfiguration robotConf && IsInServiceTypeList<Kawasaki>())
                {
                    Log?.Info("configure robot");

                    if ((serv = CreateServiceFromServiceTypeList<Kawasaki>(robotConf)) != null)
                    {
                        Robot = (IRobot<IRobotVariable>)serv;
                        ServiceList.Add(serv.SetLogger(loggerCon));
                    }
                    //try
                    //{
                    //    Robot = (IRobot<IRobotVariable>)Activator.CreateInstance(robotConf.RobotType, robotConf);

                    //    //IRobot newRobot = new Kawasaki(robotConf, null);
                    //    ServiceList.Add(Robot.SetLogger(loggerCon));
                    //}
                    //catch //robot 
                    //{
                    //    Log?.Warn("error on configure robot");
                    //    continue;
                    //}
                }
                else if(serviceConf is DiagnosticConfiguration diagnosticCOnf && IsInServiceTypeList<Diagnostic.Core.Diagnostic>())
                {
                    Log?.Info("configure diagnostic");

                    if ((serv = CreateServiceFromServiceTypeList<Diagnostic.Core.Diagnostic>(diagnosticCOnf)) != null)
                    {
                        DiagnosticService = (Diagnostic.Core.Diagnostic)serv;
                        ServiceList.Add(serv.SetLogger(loggerCon));
                    }
                }
                else if (serviceConf is OpcClientConfiguration clientConfig && IsInServiceTypeList<OpcClientService>())
                {
                    Log?.Info("configure Opc Client");

                    if ((serv = CreateServiceFromServiceTypeList<OpcClientService>(clientConfig)) != null)
                    {
                        ServiceList.Add(serv.SetLogger(loggerCon));
                    }
                }
                else if (serviceConf is ReadProgramsConfiguration progConfig && IsInServiceTypeList<ReadProgramsService>())
                {
                    Log?.Info("configure Programs Reading");

                    if ((serv = CreateServiceFromServiceTypeList<ReadProgramsService>(progConfig)) != null)
                    {
                        ServiceList.Add(serv.SetLogger(loggerCon));
                    }
                }
                else if (serviceConf is MySqlConfiguration mysqlConfig && IsInServiceTypeList<MySQLService>())
                {
                    Log?.Info("configure MySQL");

                    if ((serv = CreateServiceFromServiceTypeList<MySQLService>(mysqlConfig)) != null)
                    {
                        ServiceList.Add(serv.SetLogger(loggerCon));
                    }
                }
                else if (serviceConf is CoreConfiguration coreConf)
                {
                    Configuration = coreConf;
                }
            }

            return ServiceList;
        }


        public void LoadConfiguration(string fileName)
        {
            try
            {
                CoreConfigurations = CoreBuilder.LoadConfiguration(fileName);
            }
            catch
            {
                CoreConfigurations = null;
            }
        }

        public void SaveConfiguration(string filename)
        {
            CoreBuilder.Save(CoreConfigurations, filename);
        }


        public void SaveConfigurationFileDefault()
        {
            CreateConfigurationFileDefault(ConfigFile);
        }

        public void CreateConfigurationFileDefault(string file)
        {
            CoreConfigurationsBuilder coreBuilder = new CoreConfigurationsBuilder(file, ".\\");

            CoreConfigurations newConfiguration = new CoreConfigurations();
            //WebApiCoreConfiguration webApiCoreConf = new WebApiCoreConfiguration()
            //{
            //    Host = "localhost",
            //    Scheme = "http",
            //    Port = 5150,
            //    ServiceName = "Rest API RSWare",
            //    Active = false
            //};

            //OpcServerConfiguration opcConfig = new OpcServerConfiguration()
            //{
            //    Host = "localhost",
            //    Scheme = "opc.tcp",
            //    ServiceName = "OPC UA Server",
            //    Port = 6000,
            //    DefaultKeepAliveFutureValueExpected = 0,
            //    DefaultKeepAliveFutureValueToSet = 1,
            //    Active = false,
            //    UserList = new List<OpcUser>
            //    {
            //        new OpcUser() {UserP = UserPrivilegiesType.Admin, UserName = "Admin", Password = "robots"},
            //        new OpcUser() {UserP = UserPrivilegiesType.User, UserName = "MES", Password = "MesUser"},
            //    }           
            //};

            //RobotConfiguration robotConfigurator = new RobotConfiguration()
            //{
            //    Host = "localhost",
            //    Port = 9105,
            //    RobotType = typeof(Kawasaki),
            //    ServiceName = "Kawasaki",
            //    Active = false
            //};

            //newConfiguration.ServiceConfigurations.Add(webApiCoreConf);
            //newConfiguration.ServiceConfigurations.Add(opcConfig);
            //newConfiguration.ServiceConfigurations.Add(robotConfigurator);

            CoreConfiguration coreConf = new CoreConfiguration()
            {
                Active = true,
            };

            newConfiguration.ServiceConfigurations.Add(coreConf);
            coreBuilder.Save(newConfiguration);
        }

        public async Task<IService> Start()
        {
            var taskStarted = await StartAllService();

            if (taskStarted != null)
            {
                if (CoreIsStarted != null)
                {
                    CoreIsStarted(this, new EventArgs());
                }

                return taskStarted;
            }

            return null;
        }

        public void Stop()
        {
            StopAllService();
        }

        public IService SetLogger(LoggerConfigurator logger)
        {
            Log = logger?.GetLogger(this);

            Log?.Info($"Create the {Name} service");

            return this;
        }

        #region ----DIAGNOSTIC------


        public void CreateDiagnosticTool(string fileToLoad, ILog logger)
        {

            DiagnosticService = new Diagnostic.Core.Diagnostic(fileToLoad, logger);
            DiagnosticService.Load();
        }

        #endregion
    }



}
