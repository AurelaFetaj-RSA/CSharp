using log4net;
using Robot;
using Diagnostic.Abstract;
using System;


namespace Diagnostic.State
{
    /// <summary>
    /// For checking the diagnostics, state pattern is implemented
    /// </summary>
    public class RobotDiagnostics
    {
        protected IRobotMemory<BaseRobotVariable> _memory { get; set; } = null;
        protected DiagnosticContext _context = null;
        protected DiagnosticState _state = null;

        public RobotDiagnostics(IRobotMemory<BaseRobotVariable> irobotMemo)
        {
            _memory = irobotMemo;

        }
    }

    public class DiagnosticState : AbstractDiagnosticState
    {
        public DiagnosticState(int stateNumber, string stateName,string message, IRobotMemory<BaseRobotVariable> mem) : base(mem)
        {
            StatusValue = stateNumber;
            DiagnosticMessage = message;
            DiagnosticStateName = stateName;
        }

        public override void InternalStatus()
        {
            _context.Logger.Info($"{DiagnosticStateName}: {DiagnosticMessage}");
        }
    }
}
