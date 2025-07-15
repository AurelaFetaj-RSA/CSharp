using log4net;
using Robot;
using RSACommon.Diagnostic.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSACommon.Diagnostic
{
    public class ConcreteDiagnostics : RobotDiagnostics
    {
        ILog _logger;

        Dictionary<int, Type> _diagnostics = new Dictionary<int, Type>();
        public string DiagnosticTaskName { get; private set; }
        private string _stateVariable { get; }

        /// <summary>
        /// The VariableToCheck is the string to check in VirtualizedMemory, RobotMemo is the memory allocated, dictionaryState is the dictionary state value => class type in State Design Pattern
        /// </summary>
        /// <param name="robotMemo"></param>
        /// <param name="logger"></param>
        /// <param name="taskName"></param>
        /// <param name="dictionaryState"></param>
        /// <param name="variableToCheck"></param>
        public ConcreteDiagnostics(IRobotMemory<BaseRobotVariable> robotMemo, ILog logger, string taskName, Dictionary<int, Type> dictionaryState, string variableToCheck) : base(robotMemo)
        {
            _logger = logger;
            _diagnostics = dictionaryState;
            _stateVariable = variableToCheck;

            DiagnosticTaskName = taskName;

            StartDiagnostic();
        }

        public void StartDiagnostic()
        {
            _context = new ConcreteDiagnosticContext(null, _memory, _logger);
        }
        /*
        public ulong DiagnosticResult()
        {
            int stateClass = _memory.GetMemoryValue<int>($"{_stateVariable}");

            if (_diagnostics.TryGetValue(stateClass, out Type t))
            {
                DiagnosticState createdState = (DiagnosticState)Activator.CreateInstance(t, _memory);
                _context.TransitionTo(createdState);
                _context.CheckExecution();

                return createdState.BitMask;
            }

            return ulong.MaxValue;
        }
        */

        public class ConcreteDiagnosticContext : RSACommon.Diagnostic.Abstract.DiagnosticContext
        {
            public ConcreteDiagnosticContext(DiagnosticState state, IRobotMemory<BaseRobotVariable> robotMem, ILog log = null) : base(robotMem, log)
            {
                if (state != null)
                {
                    TransitionTo(state);
                    CheckExecution();
                }

            }

            public override void CheckExecution()
            {
                _state.InternalStatus();
            }
        }
    }
}
