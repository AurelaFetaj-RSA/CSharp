using RSACommon;
using RSAInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagnostic.Configuration
{
    public class DiagnosticConfiguration : IServiceConfiguration
    {
        public string ServiceName { get; set; } = "Diagnostic";
        public string Scheme { get; set; }
        public bool Active { get; set; } = false;
        public int DiagnosticFormRows { get; set; } = 1;
        public int DiagnosticFormColumns { get; set; } = 2;
        public int DiagnosticPolling { get; set; } = 500;
        public string DiagnosticFile { get; set; } = string.Empty;
    }
}
