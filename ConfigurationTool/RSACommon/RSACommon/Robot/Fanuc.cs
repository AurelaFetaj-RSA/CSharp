using log4net;
using RSACommon;
using RSAInterface.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Robot
{
    public class Fanuc : BaseRobot<BaseRobotVariable>
    {
        public override bool IsConnected => throw new NotImplementedException();

        public override void CreateNewVirtualMemory()
        {
            throw new NotImplementedException();
        }

        public override IRobotMemory<IRobotVariable> LoadMemoryConfiguration()
        {
            throw new NotImplementedException();
        }

        public override IRobotMemory<IRobotVariable> LoadMemoryConfiguration(string file)
        {
            throw new NotImplementedException();
        }

        public override bool ReadCommand<T>(string monitorCommand, ref T ret)
        {
            throw new NotImplementedException();
        }

        public override Task<T> ReadCommandAsync<T>(string monitorCommand)
        {
            throw new NotImplementedException();
        }

        public override bool SaveMemoryConfiguration(string filepath)
        {
            throw new NotImplementedException();
        }

        public override bool SetCommand(string monitorCommand, ref string ret)
        {
            throw new NotImplementedException();
        }

        public override bool SetVariable<T>(string monitorCommand, T ret)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> SetVariableAsync<T>(string monitorCommand, T ret)
        {
            throw new NotImplementedException();
        }

        protected override RobotConnectionStatus Connect()
        {
            throw new NotImplementedException();
        }
    }
}
