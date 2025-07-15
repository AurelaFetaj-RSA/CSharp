using RSAInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSACommon.Configuration
{
    public enum UserPrivilegiesType
    {
        Admin,
        User
    }
    public class OpcUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public UserPrivilegiesType UserP { get; set; } = UserPrivilegiesType.User;
    }

    public class OpcServerConfiguration : IServiceConfigurationRemote
    {
        public string ServiceName { get; set; }
        public int Port { get; set; }
        public string Host { get; set; }
        public string Scheme { get; set; }
        public bool Active { get; set; } = false;
        public int DefaultKeepAliveFutureValueExpected { get; set; } = 0;
        public int DefaultKeepAliveFutureValueToSet { get; set; } = 1;
        public List<OpcUser> UserList { get; set; } = new List<OpcUser>();
        public int TimeoutMilliseconds { get; set; } = 10000;
        public bool RobotIsActive { get; set; } = true;
        //private Type _serviceType = typeof(OpcServerConfigurator);
        //public Type ServiceType => _serviceType;
    }

}
