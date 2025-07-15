using log4net;
using Newtonsoft.Json;
using Robot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagnostic.Abstract
{

    public abstract class AbstractDiagnosticState
    {
        public virtual int StatusValue { get; protected set; } = -1;
        [JsonIgnore]
        public virtual string StatusValueString { get => StatusValue.ToString(); }

        [JsonIgnore]
        protected IRobotMemory<BaseRobotVariable> _rbotMem = null;

        public ulong BitMask { get; protected set; }
        public string DiagnosticMessage = "";
        public string DiagnosticStateName { get; set; }
        public TimeSpan TimeOut = TimeSpan.Zero;
        public AbstractDiagnosticState(IRobotMemory<BaseRobotVariable> rbotMem)
        {
            _rbotMem = rbotMem;
        }

        protected DiagnosticContext _context;
        public void SetContext(DiagnosticContext context)
        {
            this._context = context;
        }

        public abstract void InternalStatus();

        public void ChangeStatusValue(int value)
        {
            StatusValue = value;
        }
    }
}
