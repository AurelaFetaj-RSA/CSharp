using Robot;
using RSACommon;
using RSACommon.GraphicsForm;
using RSAInterface.Logger;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System;
using DiagnosticToolbox.Properties;
using Diagnostic.State;
using OpcCustom;
using RSAInterface;

namespace DiagnosticTool
{
    public partial class DiagnosticToolForm : Form
    {
        static Core myCore;
        readonly SplashScreen _splashScreen = null;
        IRobot<IRobotVariable> myRobot;

        public DiagnosticToolForm(SplashScreen splash)
        {
            _splashScreen?.WriteOnTextboxAsync($"Initialization...");

            InitCore();

            InitializeComponent();

            _splashScreen?.WriteOnTextboxAsync($"Set the GUI");
            StartDiagnosticGUI(Settings.Default.DiagnosticFilePath);
            _splashScreen?.WriteOnTextboxAsync($"Loaded Diagnostic File");
        }

        Dictionary<string, DiagnosticWindowsControl> DiagnosticVariableGroupbox = new Dictionary<string, DiagnosticWindowsControl>();

        private void InitCore()
        {
            myCore = new Core("Core1x");
            myCore.LoadConfiguration(myCore.ConfigFile);

            _splashScreen?.WriteOnTextboxAsync($"{DateTime.Now} Init Core Configuration");

            string logName = $"Log\\{DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss")}.log";
            LoggerConfigurator loadedloggerConfigurator = new LoggerConfigurator("LoggerConfigurations.json").Load().SetAllLogFileName(logName).Save();

            myCore.AddScoped<Robot.Kawasaki>();

            var listOfService = myCore.CreateServiceList(myCore.CoreConfigurations, loadedloggerConfigurator);

            foreach (var service in listOfService)
            {
                _splashScreen?.WriteOnTextboxAsync($"Service: {service.Name} loaded");
            }

            InitServices();
            _splashScreen?.WriteOnTextboxAsync($"Core Configuration ended");

            myCore.Start();
            _splashScreen?.WriteOnTextboxAsync($"Core Started");
        }

        public void StartUpdateGuiTask()
        {
            CancellationToken token = new CancellationToken();
            Task.Run(async () => await UpdateDiagnosticGUI(TimeSpan.FromMilliseconds(Settings.Default.UpdateDiagnosticTimeMS), token));
        }

        private void InitServices()
        {
            //webapi config

            //robot config
            IService service = myCore.ServiceList.Find(t => t.GetType() == typeof(Kawasaki));

            if (service is IRobot<IRobotVariable> robot)
            {
                robot.LoadMemoryConfiguration();
                myRobot = robot;
            }

        }

        public async Task UpdateDiagnosticGUI(TimeSpan interval, CancellationToken cancellationToken)
        {

            while (true)
            {
                await CheckDiagnostic();
                await Task.Delay(interval, cancellationToken);
            }
        }

        private void StartDiagnosticGUI(string file)
        {
            IService robot = myCore.ServiceList.Find(t => t.GetType() == typeof(Kawasaki));

            if (robot is Kawasaki kwRobot)
            {
                myCore.CreateDiagnosticTool(file, kwRobot.Log);
            }

            MatrixPanel diagnosticMatrix = new MatrixPanel(5, 0, this.Width - 25, this.Height - 50, Settings.Default.DiagnosticRow, Settings.Default.DiagnosticCol, this);


            foreach (string value in myCore.DiagnosticService.VariableList)

            {
                DiagnosticWindowsControl form = new DiagnosticWindowsControl(0, 0, value, null, null);
                DiagnosticVariableGroupbox[value] = form;

                if (!diagnosticMatrix.AddElements(form, 5, 5))
                    MessageBox.Show("Too much Diagnostic Variable for Panel Matrix");
            }

            //int size = diagnosticMatrix.MatrixControls.Count;
            
        }

        private Dictionary<string, int> _lastState = new Dictionary<string, int>();

        public async Task CheckDiagnostic()
        {


            List<string> variableList = myCore.DiagnosticService.DiagnosticStatus.Keys.ToList();


            foreach (string diagnosticVariableName in variableList)
            {
                //int value = await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>(diagnosticVariableName);
                string command = KawasakiMemoryVariable.MakeCommand(KawasakiCommand.ty, diagnosticVariableName);
                int value = await myRobot.ReadCommandAsync<int>(command);

                if (myCore.DiagnosticService.DiagnosticResult(diagnosticVariableName, value, out DiagnosticState state))
                {

                    //controllo aggiunto per tracciare solo la variazione di Stato.
                    if (!_lastState.ContainsKey(diagnosticVariableName) || _lastState[diagnosticVariableName] != value)
                    {
                        string stateOutput = state.DiagnosticMessage;

                        if (DiagnosticVariableGroupbox.Count != 0)
                        {
                            DiagnosticVariableGroupbox[diagnosticVariableName].ThreadSafeWriteMessage($"| {value:000} | {stateOutput}");
                        }

                        _lastState[diagnosticVariableName] = value;
                    }

                }

            }
            
        }
    }
}