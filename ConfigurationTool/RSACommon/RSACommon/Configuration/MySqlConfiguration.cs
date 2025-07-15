using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSAInterface;

namespace RSACommon.Configuration
{
    public class SqlUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class MySqlConfiguration : IServiceConfigurationRemote
    {
        public int Port { get; set; } = 3306;
        public string Host { get; set; } = "localhost";
        public string ServiceName { get; set; } = "MySQL80";
        public string Scheme { get; set; } = "http";
        public bool Active { get; set; }
        public string DBName { get; set; } = null;
        public SqlUser User { get; set; } = null;
    }
}
