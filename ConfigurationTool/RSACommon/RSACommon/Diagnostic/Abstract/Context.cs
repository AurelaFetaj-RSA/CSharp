using Diagnostic.State;
using log4net;
using Robot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagnostic.Abstract
{

    public abstract class DiagnosticContext
    {
        protected IRobotMemory<BaseRobotVariable> _memory = null;
        protected DiagnosticState _state = null;
        public ILog Logger { get; private set; }    
        public DiagnosticContext(IRobotMemory<BaseRobotVariable> robotMem, ILog log = null)
        {
            _memory = robotMem;
            Logger = log;
        }

        public virtual void TransitionTo(DiagnosticState state)
        {
            this._state = state;
            this._state.SetContext(this);
        }

        public abstract void CheckExecution();
    }

}
