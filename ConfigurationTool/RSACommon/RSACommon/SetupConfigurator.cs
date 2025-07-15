using RSACommon;
using RSAInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner.Setup
{
    public class ScannerConfigurator : IServiceConfiguration
    {
        public string ServiceName { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Host { get; set; } = string.Empty;    
        public string Scheme { get; set; } = string.Empty;
        public bool Active { get; set; }
    }

}
