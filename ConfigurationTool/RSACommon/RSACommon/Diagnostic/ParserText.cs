using Diagnostic.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diagnostic.Core;

namespace Diagnostic.Parser
{
    public static class ParserDiagnosticText
    {
        static int _variableNameValue => 0;
        static int _variableStatusValue => 1;
        static int _variableNameStatus => 2;
        static int _variableStatusMessage => 3;

        public static Dictionary<string, StateDiagnosticManager> Load(string filename)
        {
            Diagnostic.Core.Diagnostic configurator = new Diagnostic.Core.Diagnostic();
            Dictionary<string, StateDiagnosticManager> output = new Dictionary<string, StateDiagnosticManager>();

            string text = string.Empty;

            using (var sr = new StreamReader(filename))
            {
                text = sr.ReadToEnd();

                string[] splittedLine = text.Split('\n');

                try
                {
                    foreach (string line in splittedLine)
                    {

                        string[] dataInLine = line.Split(';');                        

                        if (dataInLine.Length > 1 &&int.TryParse(dataInLine[_variableStatusValue], out int value))
                        {
                            DiagnosticState state = new DiagnosticState(value, dataInLine[_variableNameStatus], dataInLine[_variableStatusMessage], null);
                            configurator.Add(dataInLine[_variableNameValue], state);
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("Error on parsing files");
                }
            }

            return configurator.DiagnosticStatus;

        }

    }
}
