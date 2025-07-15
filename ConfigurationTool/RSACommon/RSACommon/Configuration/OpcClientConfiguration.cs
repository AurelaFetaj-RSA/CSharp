using RSAInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSACommon.Configuration
{
    public class OpcClientConfiguration: IServiceConfigurationRemote
    {

        public string ServiceName { get; set; }
        public int Port { get; set; }
        public string Host { get; set; }
        public string Scheme { get; set; }
        public bool Active { get; set; } = false;
        public int DefaultKeepAliveFutureValueExpected { get; set; } = 0;
        public int DefaultKeepAliveFutureValueToSet { get; set; } = 1;
        public int TimeoutMilliseconds { get; set; } = 10000;
        public int DisconnectionTimeoutMilliseconds { get; set; } = 10000;
    }
}
