using RSAInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSACommon.Configuration
{
    public class ReadProgramsConfiguration : IServiceConfiguration
    {
        public string ServiceName { get; set; } = "READ PROGRAMS SERVICE";
        public string Scheme { get; set; } = "file://";
        public bool Active { get; set; } = false;
        public string[] Extensions { get; set; } = new string[] { "PROG","LAV","AU1","AU2" };
        public string[] ProgramsPath { get; set; } = new string[] { @"C:\PROGRAMS\" };
    }
}
