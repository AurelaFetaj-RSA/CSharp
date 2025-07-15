using log4net;
using RSAInterface.Logger;
using RSACommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using RSACommon.Event;
using RSAFile;
using RSAFile.Json;
using RSAInterface;

namespace Robot
{
    public abstract class BaseRobot<V> : IRobot<V> where V : IRobotVariable
    {


        protected JsonConfiguration memorySaverConfig;

        #region --- interface implementation ---
        public string Name { get; protected set; }
        public abstract bool IsConnected { get; }
        public Uri ServiceURI { get; set; }
        public string CommunicationString { get => _commString; }
        protected string _commString;
        public string Host { get => _host; }
        protected string _host;
        public ILog Log { get; protected set; }
        private readonly int MAX_DEACTIVATING_TRY = 10;
        private int _reconnectionTryRemain { get; set; } = 0;
        public int Port { get; }
        protected int _port;
        public bool IsActive { get; protected set; }
        public IServiceConfiguration Configuration { get; protected set; }
        public IRobotMemory<IRobotVariable> VirtualizedMemory { get; protected set; }
        protected abstract RobotConnectionStatus Connect();

        public readonly string MemoryConfiguratorFilename = "MemoryConfigurator.json";
        public readonly string MemoryConfiguratorPath = "";

        public BaseRobot()
        {
        }

        public abstract Task<T> ReadCommandAsync<T>(string monitorCommand);

        public abstract bool ReadCommand<T>(string monitorCommand, ref T ret);

        public abstract bool SetVariable<T>(string monitorCommand, T ret);
        public abstract Task<bool> SetVariableAsync<T>(string monitorCommand, T ret);


        public virtual async Task<IService> Start()
        {
            //await Task.Delay(100);
            return this;
        }

        public virtual void Stop()
        {
            IsActive = false;




        }

        /// <summary>
        /// 
        /// </summary>
        ///// <returns></returns>
        public bool SaveMemoryConfiguration()
        {
            string filename = Path.Combine(MemoryConfiguratorPath, MemoryConfiguratorFilename);
            return SaveMemoryConfiguration(filename);
        }

        //public virtual bool SaveMemoryConfiguration(string path)
        //{
        //    memorySaverConfig.Save<IRobotMemory<IRobotVariable>>(path, VirtualizedMemory);
        //    return true;
        //}

        public abstract bool SetCommand(string monitorCommand, ref string ret);

        /// <summary>
        /// This function will load the last robot memory virtualized
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        //public virtual IRobotMemory<V> LoadMemoryConfiguration(string filename)
        //{
        //    if (!File.Exists(filename))
        //    {
        //        CreateNewVirtualMemory();

        //        memorySaverConfig.Save(filename, VirtualizedMemory);
        //    }

        //    return memorySaverConfig.Load<IRobotMemory<V>>(filename);
        //}

        //public virtual IRobotMemory<V> LoadMemoryConfiguration()
        //{
        //    string filename = Path.Combine(MemoryConfiguratorPath, MemoryConfiguratorFilename);
        //    return LoadMemoryConfiguration(filename);
        //}

        public abstract void CreateNewVirtualMemory();

        public virtual IService SetLogger(LoggerConfigurator loggerConfig)
        {
            Log = loggerConfig?.GetLogger(this);

            Log?.Info($"Create the {Name} at {ServiceURI.AbsoluteUri}");

            return this;
        }
        #endregion

        public void ResetReactivatingTry()
        {
            _reconnectionTryRemain = 0;
        }

        public void Deactivating()
        {
            if (_reconnectionTryRemain < MAX_DEACTIVATING_TRY)
            {
                _reconnectionTryRemain++;
                Log?.Warn($"{Name} tried to reconnect {_reconnectionTryRemain} times");
            }
            else
            {
                RSACustomEvents.OnServiceStop(new RSACustomEvents.ServiceStopEventArgs(this, "FutureServer"));
                Log?.Error($"{Name} robot service will be stopped");
                Stop();
            }
        }

        public abstract bool SaveMemoryConfiguration(string filepath);
        public abstract IRobotMemory<IRobotVariable> LoadMemoryConfiguration();
        public abstract IRobotMemory<IRobotVariable> LoadMemoryConfiguration(string file);


    }
}
