using RSACommon;
using RSAInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSACommon.Alarm.Configuration
{
    public class AlarmConfiguration : IServiceConfiguration
    {
        public string ServiceName { get; set; } = "Alarm";
        public string Scheme { get; set; }
        public bool Active { get; set; } = false;
        public List<string> Files { get; set; } = new List<string>();
    }
}
