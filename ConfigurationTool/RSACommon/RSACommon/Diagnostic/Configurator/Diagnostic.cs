using Diagnostic.Configuration;
using Diagnostic.Parser;
using Diagnostic.State;
using log4net;
using Newtonsoft.Json;
using Robot;
using RSACommon;
using RSAInterface.Logger;
using RSAInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSAInterface.Helper;

namespace Diagnostic.Core
{

    public class Diagnostic: IService
    {
        public string FileName { get; protected set; }

        /// <summary>
        /// variable to check for the diagnostic state
        /// </summary>
        public Dictionary<string, StateDiagnosticManager> DiagnosticStatus { get; protected set; }
        public List<string> VariableList => DiagnosticStatus.Keys.ToList();

        public string Name { get; private set; }

        public ILog Log { get; private set; }

        public Uri ServiceURI => throw new NotImplementedException();

        public bool IsActive { get; set; } = false;
        public IServiceConfiguration Configuration { get; private set; }
        public int DiagnosticFormRows { get; private set; } = 1;
        public int DiagnosticFormCols { get; private set; } = 1;
        public DiagnosticConfiguration DiagnosticConfig { get; private set; }

        public int DiagnosticCount(string variableName)
        {
            if(DiagnosticStatus.TryGetValue(variableName, out StateDiagnosticManager value))
            {
                return value.DiagnosticData.Count;
            }

            return 0;
        }

        public Diagnostic(string fileName = "diagnostic.json", ILog logger = null) 
        {
            FileName = fileName;
            Log = logger;
            DiagnosticStatus = new Dictionary<string, StateDiagnosticManager>();

        }

        public Diagnostic(IServiceConfiguration conf)
        {
            if (conf is DiagnosticConfiguration diagConfg)
            {
                DiagnosticConfig = diagConfg;
                FileName = diagConfg.DiagnosticFile;
                Name = diagConfg.ServiceName;
                Configuration = diagConfg;
                DiagnosticFormCols = diagConfg.DiagnosticFormColumns;
                DiagnosticFormRows = diagConfg.DiagnosticFormRows;
                IsActive = diagConfg.Active;
            }
            else
                return;

            DiagnosticStatus = new Dictionary<string, StateDiagnosticManager>();


            Load(FileName);

        }

        public StateDiagnosticManager State(string variableName)
        {
            StateDiagnosticManager toReturn = null;

            if(!DiagnosticStatus.TryGetValue(variableName, out toReturn))
            {
                Log.Warn("Error on diagnostic search");
                return toReturn;
            }

            return toReturn;
        }


        /// <summary>
        /// This function will execute the function target for every DiagnosticState, usable in a lot of ways
        /// </summary>
        /// <param name="function"></param>
        public void DiagnosticCheck(Action<DiagnosticState> function)
        {
            foreach(var variableNameManager in DiagnosticStatus)
            {
                foreach(var keyState in DiagnosticStatus[variableNameManager.Key].DiagnosticData)
                {
                    function(keyState.Value);
                }
            }
        }


        public bool Remove(string variableState, string selectedState)
        {
            if (DiagnosticStatus.ContainsKey(variableState))
            {
                var selected = DiagnosticStatus[variableState];

                if (selected.DiagnosticData.ContainsKey(selectedState))
                {
                    selected.DiagnosticData.Remove(selectedState);
                    return true;
                }
            }

            return false;
        }

        public bool DiagnosticResult(string variableState, int selectedState, out DiagnosticState returningState)
        {
            return DiagnosticResult(variableState, selectedState.ToString(), out returningState);
        }

        public bool DiagnosticResult(string variableState, string selectedState, out DiagnosticState returningState)
        {
            if (DiagnosticStatus.ContainsKey(variableState))
            {
                var selected = DiagnosticStatus[variableState];

                if (selected.DiagnosticData.TryGetValue(selectedState, out DiagnosticState foundState))
                {
                    returningState = foundState;
                    return true;
                }
                else
                {
                    Log?.Warn($"Diagnostic {variableState} => {selectedState} not present");
                    returningState = selected.ErrorState;
                    return true;
                }

            }

            returningState = null;
            return false;
        }

        public void Add(string diagnosticStateVariable, DiagnosticState stateToAdd, bool overwrite = false)
         {
            if(DiagnosticStatus.TryGetValue(diagnosticStateVariable, out StateDiagnosticManager stateDiagn))
            {
                stateDiagn.Add(stateToAdd, overwrite);
            }
            else
            {
                var diagnostic = new StateDiagnosticManager(diagnosticStateVariable);

                if(stateToAdd != null)
                    diagnostic.Add(stateToAdd);

                DiagnosticStatus[diagnosticStateVariable] = diagnostic;
            }
        }

        public void Load(string filename)
        {
            if (File.Exists(filename))
            {
                if (filename.Contains("$$$"))
                {
                    DiagnosticStatus = ParserDiagnosticText.Load(filename);
                }
                else if (filename.Contains("json"))
                {
                    JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings();
                    settings.Formatting = Formatting.Indented;

                    DiagnosticStatus = Helper.Load<Dictionary<string, StateDiagnosticManager>>(filename, settings);

                }
            }

            foreach(var keyDiagnostic in DiagnosticStatus)
            {
                string errorDiagnosticStateValue = keyDiagnostic.Value.ErrorState.StatusValueString;
                keyDiagnostic.Value.DiagnosticData[errorDiagnosticStateValue] = keyDiagnostic.Value.ErrorState;
            }
        }

        public void Load()
        {
            Load(FileName);
        }

        public void Save()
        {
            Save(FileName);
        }

        public void Save(string filename)
        {
            JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings();
            settings.Formatting = Formatting.Indented;
            Helper.Save(filename, DiagnosticStatus, settings);
        }

        public async Task<IService> Start()
        {
            await Task.Delay(100);
            Log?.Info($"Started the {Name} service");

            return this;
        }

        public void Stop()
        {
            Log?.Info($"Closed the {Name} service");
        }

        public IService SetLogger(LoggerConfigurator logger)
        {
            Log = logger?.GetLogger(this);
            Log?.Info($"Create the {Name} service");

            return this;
        }
    }

    public class ErrorDiagnosticState : DiagnosticState
    {
        public ErrorDiagnosticState(int stateNumber, string stateName, string message, IRobotMemory<BaseRobotVariable> mem) : base(stateNumber, stateName, message, mem)
        {
        }
    }


    public class StateDiagnosticManager
    {
        public string DiagnosticVariableState { get; set; } = string.Empty;
        public Dictionary<string, DiagnosticState> DiagnosticData = new Dictionary<string, DiagnosticState>(); 
        public ErrorDiagnosticState ErrorState { get; set; } = null;
        
        public StateDiagnosticManager(string variable)
        {
            DiagnosticVariableState = variable;
            ErrorState = new ErrorDiagnosticState(0xFFFF, "ErrorStatus", "Stato non presente nella configurazione", null);
        }



        public StateDiagnosticManager() { }

        public bool Add(DiagnosticState state, bool overwrite = false)
        {
            if (state == null)
                return false;

            if(overwrite || !DiagnosticData.ContainsKey(state.StatusValueString))
            {
                DiagnosticData[state.StatusValueString] = state;
            }

            return true;
        }
    }
}
