using log4net;
using RSACommon;
using RSAInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Robot
{
    public class RobotConfiguration : IServiceConfiguration
    {
        public string ServiceName { get; set; } = "Robot";
        public int Port { get; set; } = 0;
        public string Host { get; set; } = String.Empty;
        public string Scheme { get; set; } = String.Empty;
        public bool Active { get; set; } = false;
        public Type RobotType { get; set; } = null;
    }

    
    public enum RobotConnectionStatus
    {
        Connected = 0,
        ConnectionError = 1,
        LoginError = 2
    }

    public interface IRobot<out V>: IService where V: IRobotVariable
    {
        string Host { get; }
        int Port { get; }
        string CommunicationString { get; }
        bool IsConnected { get; }
        bool ReadCommand<T>(string monitorCommand, ref T ret);
        Task<T> ReadCommandAsync<T>(string monitorCommand);
        bool SetVariable<T>(string monitorCommand, T ret);
        Task<bool> SetVariableAsync<T>(string monitorCommand, T ret);
        bool SaveMemoryConfiguration();
        bool SaveMemoryConfiguration(string filepath);
        IRobotMemory<IRobotVariable> VirtualizedMemory { get; }
        /// <summary>
        /// This function will add RobotVariable to the virtualized memory, this will not create the var on the robot's memory
        /// </summary>
        /// <param name="name"></param>
        /// <param name="varToAdd"></param>
        /// <returns></returns>
        //bool AddMemory(string name, RobotVariable varToAdd);
        IRobotMemory<IRobotVariable> LoadMemoryConfiguration();
        IRobotMemory<IRobotVariable> LoadMemoryConfiguration(string file);
        void CreateNewVirtualMemory();
        bool SetCommand(string monitorCommand, ref string ret);
    }
}
