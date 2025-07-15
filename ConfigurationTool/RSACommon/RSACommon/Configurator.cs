using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RSAInterface;


namespace RSACommon
{
    public class CoreConfiguration : IServiceConfiguration
    {
        public string ServiceName { get; set; } = "Core1X";
        public string Scheme { get; set; } = "";
        public bool Active { get; set; } = true;
    }
}
