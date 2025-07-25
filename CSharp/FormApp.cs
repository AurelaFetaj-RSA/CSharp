
using FutureServerCore;
using FutureServerCore.Core;
using Robot;
using RSACommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using WebApi;
using RSAInterface.Logger;
using RSACommon.WebApiDefinitions;
using LidorSystems.IntegralUI.Containers;
using System.IO;
using System.Web.UI.WebControls;
using log4net;
using System.Threading;
using RSACommon.GraphicsForm;
using ProductionLaunch.Properties;
using RSACommon.Event;
using Newtonsoft.Json.Linq;
using System.Timers;
using Opc.UaFx;
using OpcCustom;
using RSACommon.Service;
using RSACommon.Configuration;
using RSACommon.ProgramParser;
using System.Net;
using System.Net.Sockets;
using xDialog;

using RSAInterface;

using System.Drawing.Printing;
using Opc.UaFx.Client;


using System.Text.RegularExpressions;
using static System.Collections.Specialized.BitVector32;
using System.Data.OleDb;
using Microsoft.VisualBasic;
using System.Windows.Media.Animation;
using Nevron.UI.WinForm.Controls;
using log4net.Config;
using Opc.Ua;
using CodeVendor.Controls;
using System.Globalization;
using TCP_Asynchronous_Client;
using System.ComponentModel.Composition.Primitives;
using LBSoft.IndustrialCtrls.Leds;
using static Nevron.Interop.Win32.NUser32;
using System.Reflection;

namespace ProductionLaunch
{
    public partial class FormApp : Form
    {
        //core instance
        public static Core myCore;
        OpcClientService ccService = null;
        MySQLService mysqlService = null;
        Kawasaki myR1Service = null;
        Kawasaki myR2Service = null;

        //asyncronous client
        // For Display Data in Text Box and Info - UI Thread Invoke
        public delegate void AddLogDeligate(string data);
        public AddLogDeligate AddLog;
        public delegate void AddNotificationDelegate(int type, bool status);
        public AddNotificationDelegate UpdateStatusIcons;

        // Client Object
        AsynchronousClient tcp;



        private LidorSystems.IntegralUI.Containers.TabPage lastPage { get; set; } = null;

        private readonly SplashScreen _splashScreen = null;
        private Form _configForm { get; set; } = null;
        private Form _clientForm { get; set; } = null;

        CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private ReadProgramsService _readProgramService { get; set; } = null;

        //create static instance of gui configurator (singleton)
        public static Configurator guiConfigurator = new Configurator();
        //create static instance of input configurator (singleton)
        public static Configurator inputConfigurator = new Configurator();
        //create static instance of output configurator (singleton)
        public static Configurator outputConfigurator = new Configurator();
        //create static instance of alarms configurator (singleton)
        public static Configurator alarmsConfigurator = new Configurator();
        //create static instance of auto configurator (singleton)
        public static Configurator autoConfigurator = new Configurator();
        //create static instance of brtask configurator (singleton)
        public static Configurator brTaskConfigurator = new Configurator();
        //create static instance of check box configurator (singleton)
        public static Configurator cbTaskConfigurator = new Configurator();

        public Dictionary<int, bool> PLCLineInputDictionary = new Dictionary<int, bool>();
        public Dictionary<int, bool> PLCLineOutputDictionary = new Dictionary<int, bool>();

        public Dictionary<int, bool> PLCR1InputDictionary = new Dictionary<int, bool>();
        public Dictionary<int, bool> PLCR1OutputDictionary = new Dictionary<int, bool>();

        public Dictionary<int, bool> PLCR2InputDictionary = new Dictionary<int, bool>();
        public Dictionary<int, bool> PLCR2OutputDictionary = new Dictionary<int, bool>();

        public Dictionary<int, bool> WAGOR3InputDictionary = new Dictionary<int, bool>();
        public Dictionary<int, bool> WAGOR3OutputDictionary = new Dictionary<int, bool>();

        //public Dictionary<int, bool> R2InputDictionary = new Dictionary<int, bool>();
        //public Dictionary<int, bool> R2OutputDictionary = new Dictionary<int, bool>();
        public Dictionary<int, short> AlarmsDictionary = new Dictionary<int, short>();

        public Dictionary<int, short> GD1AlarmsDictionary = new Dictionary<int, short>();
        public Dictionary<int, short> GD2AlarmsDictionary = new Dictionary<int, short>();

        public static bool restartApp = false;

        public DateTime dt = DateTime.Now;

        public string loginLevel = "Operator";
        public string loginPassword = "";
        public Pen pNoAlarm = new Pen(Color.FromArgb(107, 227, 162), 10);
        public Brush bNoAlarm = new SolidBrush(Color.FromArgb(107, 227, 162));
        public Pen pAlarm = new Pen(Color.Red, 10);
        public Brush bAlarm = new SolidBrush(Color.Red);
        public Pen pIOOn = new Pen(Color.FromArgb(107, 227, 162), 10);
        public Brush bIOOn = new SolidBrush(Color.FromArgb(107, 227, 162));
        public Pen pIOOff = new Pen(Color.Black, 10);
        public Brush bIOOff = new SolidBrush(Color.Black);
        public Pen pOffline = new Pen(Color.Black, 10);
        public Brush bOffline = new SolidBrush(Color.Black);
        public Brush bBlackText = new SolidBrush(Color.Black);
        public Brush bWhiteText = new SolidBrush(Color.White);
        public Brush brRed = new SolidBrush(Color.Red);
        public Brush brLoadL1 = new SolidBrush(Color.FromArgb(251, 225, 116));
        public Brush brLoadL2 = new SolidBrush(Color.FromArgb(214, 190, 87));
        public Brush brLoadL3 = new SolidBrush(Color.FromArgb(175, 153, 58));
        public Brush brLoadL4 = new SolidBrush(Color.FromArgb(142, 122, 35));
        public Brush brLoadL5 = new SolidBrush(Color.FromArgb(96, 80, 13));
        public Brush brLoadL6 = new SolidBrush(Color.FromArgb(76, 60, 10));

        public int wCircle = 12;
        public int hCircle = 12;

        public float drawKFactor = 1.4f;
        public float PalletSizeX = (124 * 1.4f);
        public float PalletSizeY = (124 * 1.4f);
        public float gScaleFactorX = 0.7f;
        public float gScaleFactorY = 0.7f;
        public string file_log_R1 = "C:\\Robotsys\\";
        public string file_log_R2 = "C:\\Robotsys\\";
        public static string file_log_line = "C:\\Robotsys\\";

        private RSA myRSA = new RSA();

        public bool RuntimeKeepAliveFromOPCUAClient = false;
        public bool RuntimeKeepAliveFromOPCUAServer = false;

        private List<IoSignal> inputSignals;
        private List<IoSignal> outputSignals;

        private Dictionary<string, bool> _plcStates = new Dictionary<string, bool>();

        public FormApp(SplashScreen splashScreen)
        {
            _splashScreen = splashScreen;
            _splashScreen?.WriteOnTextboxAsync($"Initialization...");

            InitCore();

            InitializeComponent();
            this.Load += new System.EventHandler(this.Form1_Load);

            if (LoadG1RFIDReader())
            {
                _splashScreen?.WriteOnTextboxAsync($"RFID cpu connected");
            }

            InitGUI();

            _splashScreen?.WriteOnTextboxAsync($"Set the GUI");
            //Splash Screen filler
            _splashScreen?.WriteOnTextboxAsync($"Update GUI syncrozionation Thread Started");

            SetEvent();
            if (ccService != null)
            {
                if (ccService.ClientIsConnected)
                {
                }
            }

            #region(* init I/O, alarms dictionaries *)
            int i = 1;

            for (i = 1; i <= Properties.Settings.Default.LinePLCInputNumber; i++)
            {
                PLCLineInputDictionary.Add(i, false);
            }
            for (i = 1; i <= Properties.R2.Default.InputNumber; i++)
            {
                PLCR2InputDictionary.Add(i, false);
            }

            for (i = 1; i <= Properties.R1.Default.InputNumber; i++)
            {
                PLCR1InputDictionary.Add(i, false);
            }

            for (i = 1; i <= Properties.R3.Default.InputNumber; i++)
            {
                WAGOR3InputDictionary.Add(i, false);
            }

            for (i = 1; i <= Properties.Settings.Default.LinePLCOutputNumber; i++)
            {
                PLCLineOutputDictionary.Add(i, false);
            }

            for (i = 1; i <= Properties.R1.Default.OutputNumber; i++)
            {
                PLCR1OutputDictionary.Add(i, false);
            }

            for (i = 1; i <= Properties.R2.Default.OutputNumber; i++)
            {
                PLCR2OutputDictionary.Add(i, false);
            }

            for (i = 1; i <= Properties.R3.Default.OutputNumber; i++)
            {
                WAGOR3OutputDictionary.Add(i, false);
            }

            for (i = 1; i <= Properties.Settings.Default.AlarmsNumber; i++)
            {
                AlarmsDictionary.Add(i, -1);
            }
            myCore.Log?.Info($"I/O and alarms dictionaries reinitialized.");
            #endregion
        }

        private async void InitCore()
        {
            myCore = new Core("PLCore");
            myCore.LoadConfiguration(myCore.ConfigFile);

            _splashScreen?.WriteOnTextboxAsync($"Init Core Configuration");

            string logName = $"Log\\{DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss")}.log";
            file_log_R1 = Application.StartupPath + "\\Log\\R1_" + DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".$$$";
            file_log_R2 = Application.StartupPath + "\\Log\\R2_" + DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".$$$";
            file_log_line = Application.StartupPath + "\\Log\\line_" + DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".$$$";
            Common.file_log_hmi = Application.StartupPath + "\\Log\\hmi_" + DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".$$$"; ;
            LoggerConfigurator loadedloggerConfigurator = new LoggerConfigurator("LoggerConfigurations.json").Load().SetAllLogFileName(logName).Save();

            myCore.AddScoped<Diagnostic.Core.Diagnostic>();




            myCore.AddScoped<OpcClientService>();





            var listOfService = myCore.CreateServiceList(myCore.CoreConfigurations, loadedloggerConfigurator);

            List<IService> listFound = myCore.FindPerType(typeof(OpcClientService));

            string pp = Properties.Settings.Default.OpcClient_1_URI;

            foreach (IService serv in listFound)
            {
                string aa = serv.ServiceURI.AbsoluteUri;
                if (serv.ServiceURI == new Uri(Properties.Settings.Default.OpcClient_1_URI) && serv is OpcClientService clientOpcService)
                {
                    ccService = clientOpcService;
                    break;
                }
            }

            foreach (var service in listOfService)
            {
                _splashScreen?.WriteOnTextboxAsync($"Service: {service.Name} loaded");
            }

            if (ccService != null)
            {
                ccService.SetObjectData(new OpcClientConfig().Config());
            }

            myCore?.Start();


            _splashScreen?.WriteOnTextboxAsync($"Core Configuration ended");
            _splashScreen?.WriteOnTextboxAsync($"Core Started");


            if (LoadDatabases())
            {
                _splashScreen?.WriteOnTextboxAsync($"Databases loaded");
            }

            if (StartAllServer())
            {
                _splashScreen?.WriteOnTextboxAsync($"All servers in listening");
            }

            //client connection
            ClientConnect("172.31.10.126", "49155");

            Common.WritelogFile(file_log_line, "init application");
        }


        public void ClientConnect(string ipAddress, string port)
        {
            try
            {
                tcp = new AsynchronousClient(ipAddress, int.Parse(port));
                tcp.OnConnectEvent += new AsynchronousClient.OnConnectEventHandler(OnConnect);
                tcp.OnDataRecievedEvent += new AsynchronousClient.DataReceivedEventHandler(OnRecieved);

                tcp.Connect();
            }
            catch (Exception ex)
            {
                //server is not listening
                int j = 0;
            }
        }

        // Connection Status Listner
        private void OnConnect(bool status)
        {
            Console.WriteLine("client connection status: " + status);

            //rtfLog.Invoke(AddLog, "Connection : " + status.ToString());
            //lblConnected.Invoke(UpdateStatusIcons, 1, status);
        }

        private void OnDisconnect()
        {
            tcp.Disconnect();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            inputSignals = GetInputSignals();
            outputSignals = GetOutputSignals();
        }


        // Data Recieved Listner
        private void OnRecieved(string data)
        {
            // lblRead.Invoke(UpdateStatusIcons, 3, true);
            // rtfLog.Invoke(AddLog, "Recieved : " + data);

            //parse data
            if (data.Substring(0, 4).Equals("SCAN"))
            {
                MachineRuntime.scannerMessage = data;

                string[] splitted = data.Split(',');
                //get mode
                short mode = short.Parse(splitted[2]);
                if (mode == 1)
                {
                    //calibration mode: TODO
                    if (splitted[2] == "0")
                    {

                    }
                    else
                    {

                    }
                }
                else if (mode == 2)
                {
                    //reset start scanner flag
                    lbLedTrainingStarted.State = LBLed.LedState.Off;
                    //training mode
                    if (splitted[3] == "0")
                    {
                        WriteAsyncOnTextbox(textBoxXT, splitted[4]);
                        WriteAsyncOnTextbox(textBoxYT, splitted[5]);
                        WriteAsyncOnTextbox(textBoxZT, splitted[6]);
                        WriteAsyncOnTextbox(textBoxRxT, splitted[7]);
                        WriteAsyncOnTextbox(textBoxRyT, splitted[8]);
                        WriteAsyncOnTextbox(textBoxRzT, splitted[9]);
                        WriteAsyncOnTextbox(textBoxTLenght, splitted[10]);
                        lbLedScannerResult.State = LBLed.LedState.On;
                        lbLedScannerResult.LedColor = Color.LightGreen;
                    }
                    else
                    {
                        //todo manage error
                        lbLedScannerResult.State = LBLed.LedState.On;
                        lbLedScannerResult.LedColor = Color.Red;
                    }
                }
                else if (mode == 3)
                {
                    if (splitted[3] == "0")
                    {
                        //automatic mode
                        //1 dx plc
                        //2 sx plc                    
                        MachineRuntime.rfidReadingResultStringG1A2 = MachineRuntime.rfidReadingResultStringG1A2 + "localization received from scanner" + "\r\n";
                        string calcTSC = "-1";
                        if (MachineRuntime.rfidTypeFromPlcG1A2 == "RG") calcTSC = "1";
                        else if (MachineRuntime.rfidTypeFromPlcG1A2 == "LF") calcTSC = "0";

                        WriteAsyncOnTextbox(textBoxAX, splitted[4]);
                        WriteAsyncOnTextbox(textBoxAY, splitted[5]);
                        WriteAsyncOnTextbox(textBoxAZ, splitted[6]);
                        WriteAsyncOnTextbox(textBoxRX, splitted[7]);
                        WriteAsyncOnTextbox(textBoxRY, splitted[8]);
                        WriteAsyncOnTextbox(textBoxRZ, splitted[9]);
                        WriteAsyncOnTextbox(textBoxALenght, splitted[10]);

                        string runtimeLoc = splitted[4] + "," + splitted[5] + "," + splitted[6] + "," + splitted[7] + "," + splitted[8] + "," +
                            splitted[9] + "," + splitted[10] + "," + splitted[11] + "," + calcTSC;
                        try
                        {
                            myRSA.GetDBL().GetRFIDTable().UpdateRFIDParam1ByCode(MachineRuntime.rfidCodeG1A2, runtimeLoc, myRSA.GetDBL().GetConnection());
                            MachineRuntime.rfidTypeFromScannerG1A2 = (splitted[11] == "0") ? "LF" : "RG";
                            WriteAsyncOnTextbox(textBoxAType, MachineRuntime.rfidTypeFromScannerG1A2);
                            MachineRuntime.ackFromScanner = true;
                            MachineRuntime.rfidReadingResultStringG1A2 = MachineRuntime.rfidReadingResultStringG1A2 + "parameters saved for rfid " + MachineRuntime.rfidCodeG1A2 + "\r\n";
                        }
                        catch (Exception ex)
                        {
                            MachineRuntime.ackFromScanner = false;
                            MachineRuntime.rfidReadingResultStringG1A2 = MachineRuntime.rfidReadingResultStringG1A2 + "parameters NOT saved for rfid " + MachineRuntime.rfidCodeG1A2 + "\r\n";
                        }
                    }
                    else
                    {

                    }
                }
            }

            var entries = data.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var entry in entries)
            {
                var parts = entry.Split('=');
                if (parts.Length != 2) continue;

                string address = parts[0].Trim();    // e.g. I0.0;I0.1;Q0.0;Q0.1
                bool isOn = parts[1].Trim() == "1";  // true if ON

                // Update input signal
                var input = inputSignals.FirstOrDefault(sig => sig.Address == address);
                if (input != null)
                {
                    input.IsOn = isOn;
                    continue;
                }

                // Update output signal
                var output = outputSignals.FirstOrDefault(sig => sig.Address == address);
                if (output != null)
                {
                    output.IsOn = isOn;
                    continue;
                }
            }

            tabPageT8_1.Invalidate();


            Console.WriteLine("received: " + data);
        }

        private bool OnSend(string msgStr)
        {
            // Write to TCP
            try
            {
                if (tcp.Write(msgStr))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void InitGUI()
        {
            InitLastParameter();
            this.Location = new Point(0, 0);
            Size formSize = new Size(Screen.GetWorkingArea(this).Width, Screen.GetWorkingArea(this).Height);
            this.Size = new Size(formSize.Width, formSize.Height);

            timerRobotsStatus.Start();
            dataGridViewDevice1.Visible = Properties.Device1.Default.D1Present;
            dataGridViewDevice2.Visible = Properties.Device2.Default.D2Present;
            dataGridViewDevice3.Visible = Properties.Device3.Default.D3Present;
            dataGridViewDevice4.Visible = Properties.Device4.Default.D4Present;
            grouperManageD1.Visible = Properties.Device1.Default.D1Present;
            grouperManageD2.Visible = Properties.Device2.Default.D2Present;
            grouperManageD3.Visible = Properties.Device3.Default.D3Present;
            grouperManageD4.Visible = Properties.Device4.Default.D4Present;

            #region(* init gdevice 1 gui *)            
            labelGD1RuntimeParam1Text.Visible = Properties.GDevice1.Default.GD1Runtime1IsOn;
            labelGD1RuntimeParam2Text.Visible = Properties.GDevice1.Default.GD1Runtime2IsOn;
            labelGD1RuntimeParam1Text.Text = Properties.GDevice1.Default.GD1Runtime1Name;
            labelGD1RuntimeParam2Text.Text = Properties.GDevice1.Default.GD1Runtime2Name;
            #endregion

            #region(* init gdevice 1 gui *)
            labelGD1Param1NameEdit.Visible = Properties.GDevice1.Default.GD1Param1IsOn;
            labelGD1Param1NameEdit.Text = Properties.GDevice1.Default.GD1Param1Name;
            numericUpDownGD1Param1.Visible = Properties.GDevice1.Default.GD1Param1IsOn;

            labelGD1Param2NameEdit.Visible = Properties.GDevice1.Default.GD1Param2IsOn;
            labelGD1Param2NameEdit.Text = Properties.GDevice1.Default.GD1Param2Name;
            numericUpDownGD1Param2.Visible = Properties.GDevice1.Default.GD1Param2IsOn;

            labelGD1RuntimeParam1Text.Visible = Properties.GDevice1.Default.GD1Runtime1IsOn;
            labelGD1RuntimeParam2Text.Visible = Properties.GDevice1.Default.GD1Runtime2IsOn;

            labelGD1RuntimeParam1Text.Text = Properties.GDevice1.Default.GD1Runtime1Name;
            labelGD1RuntimeParam2Text.Text = Properties.GDevice1.Default.GD1Runtime2Name;
            #endregion

            #region(* init gdevice 2 gui *)
            labelGD2Param1NameEdit.Visible = Properties.GDevice2.Default.GD2Param1IsOn;
            labelGD2Param1NameEdit.Text = Properties.GDevice2.Default.GD2Param1Name;
            numericUpDownGD2Param1.Visible = Properties.GDevice2.Default.GD2Param1IsOn;

            labelGD2Param2NameEdit.Visible = Properties.GDevice2.Default.GD2Param2IsOn;
            labelGD2Param2NameEdit.Text = Properties.GDevice2.Default.GD2Param2Name;
            numericUpDownGD2Param2.Visible = Properties.GDevice2.Default.GD2Param2IsOn;

            labelGD2Param7NameEdit.Visible = Properties.GDevice2.Default.GD2Param7IsOn;
            labelGD2Param7NameEdit.Text = Properties.GDevice2.Default.GD2Param7Name;
            numericUpDownGD2Param7.Visible = Properties.GDevice2.Default.GD2Param7IsOn;

            labelGD2RuntimeParam1Text.Visible = Properties.GDevice2.Default.GD2Runtime1IsOn;
            labelGD2RuntimeParam2Text.Visible = Properties.GDevice2.Default.GD2Runtime2IsOn;
            labelGD2RuntimeParam3Text.Visible = Properties.GDevice2.Default.GD2Runtime3IsOn;
            labelGD2RuntimeParam4Text.Visible = Properties.GDevice2.Default.GD2Runtime4IsOn;

            labelGD2RuntimeParam1Text.Text = Properties.GDevice2.Default.GD2Runtime1Name;
            labelGD2RuntimeParam2Text.Text = Properties.GDevice2.Default.GD2Runtime2Name;
            labelGD2RuntimeParam3Text.Text = Properties.GDevice2.Default.GD2Runtime3Name;
            labelGD2RuntimeParam4Text.Text = Properties.GDevice2.Default.GD2Runtime4Name;
            #endregion

            #region(* gd1 register alarms dictionary *)
            int i = 0;
            for (i = 1; i <= 8; i++)
            {
                GD1AlarmsDictionary.Add(i, 0);
                GD2AlarmsDictionary.Add(i, 0);
            }
            #endregion

            InitRefreshModelNameList();

            LoadRobotsSettings();
        }

        public void LoadRobotsSettings()
        {
            #region(^ R1 *)
            //load settings for robot 1            
            labelR1Aux0_00.Text = Properties.R1.Default.R1Lav;
            labelR1Aux1_00.Text = Properties.R1.Default.R1Au1;
            labelR1Aux2_00.Text = Properties.R1.Default.R1Au2;
            labelR1LavEdit.Text = Properties.R1.Default.R1Lav;
            labelR1Au1Edit.Text = Properties.R1.Default.R1Au1;
            labelR1Au2Edit.Text = Properties.R1.Default.R1Au2;
            #endregion

            #region(* R2 *)
            //load settings for robot 2            
            labelR2Aux0_00.Text = Properties.R2.Default.R2Lav;
            labelR2Aux1_00.Text = Properties.R2.Default.R2Au1;
            labelR2Aux2_00.Text = Properties.R2.Default.R2Au2;
            labelR2LavEdit.Text = Properties.R2.Default.R2Lav;
            labelR2Au1Edit.Text = Properties.R2.Default.R2Au1;
            labelR2Au2Edit.Text = Properties.R2.Default.R2Au2;
            #endregion

            #region(* R3 *)            
            labelR3Aux0_00.Text = Properties.R3.Default.R3Lav;
            labelR3Aux1_00.Text = Properties.R3.Default.R3Au1;
            labelR3Aux2_00.Text = Properties.R3.Default.R3Au2;
            labelR3LavEdit.Text = Properties.R3.Default.R3Lav;
            labelR3Au1Edit.Text = Properties.R3.Default.R3Au1;
            labelR3Au2Edit.Text = Properties.R3.Default.R3Au2;
            #endregion
        }

        private void InitRefreshModelNameList()
        {
            List<string> mylist = new List<string>();
            int i = 0;


            mylist.AddRange(myRSA.GetDBL().GetModelTable().GetAllModelNameRecord(myRSA.GetDBL().GetConnection()));

            mylist = mylist.Distinct().ToList();

            comboBoxRFIDModelName_T0.Items.Clear();
            comboBoxRFIDLoadRotary_T0.Items.Clear();
            comboBoxSearchRobotCode.Items.Clear();
            comboBoxTModelName.Items.Clear();

            comboBoxRFIDModelName_T0.Items.AddRange(mylist.ToArray());
            comboBoxRFIDLoadRotary_T0.Items.AddRange(mylist.ToArray());
            comboBoxSearchRobotCode.Items.AddRange(mylist.ToArray());
            comboBoxTModelName.Items.AddRange(mylist.ToArray());
        }

        private void InitLastParameter()
        {
            #region(* init tabControlMain *)
            tabControlMain.SelectedPage = tabPageT0;
            #endregion

            TimeZone zone = TimeZone.CurrentTimeZone;
            DateTime local = zone.ToLocalTime(DateTime.Now);
            //gui config
            guiConfigurator.LoadFromFile("guiconfig.xml", Configurator.FileType.Xml);
            //input config
            inputConfigurator.LoadFromFile("inputconfig.xml", Configurator.FileType.Xml);
            myCore.Log?.Info($"inputconfig loaded");
            //output config
            outputConfigurator.LoadFromFile("outputconfig.xml", Configurator.FileType.Xml);
            myCore.Log?.Info($"outputconfig loaded");
            //alarms config
            alarmsConfigurator.LoadFromFile("alarmsconfig.xml", Configurator.FileType.Xml);
            myCore.Log?.Info($"alarmsconfig loaded");
            //automatic config
            //    autoConfigurator.LoadFromFile("autoconfig.xml", Configurator.FileType.Xml);
            //barcode reading task config
            brTaskConfigurator.LoadFromFile("brtask.xml", Configurator.FileType.Xml);
            //check box task config
            cbTaskConfigurator.LoadFromFile("cbtask.xml", Configurator.FileType.Xml);

            myCore.Log?.Info($"init form station/bank");









            UpdateGUIByUser();
        }

        private void UpdateGUIByUser()
        {

            //tabPageT6.Enabled = (loginLevel == "Operator") ? false : true;

        }

        public void Start()
        {
            //myCore.Start();
        }

        public async void SetEvent()
        {
            Core.CoreIsStarted += CoreIsStartedCallBack;
            //RSACustomEvents.KeepAliveTimeoutEvent += RSACustomEvents_KeepAliveTimeoutEvent;
            //RSACustomEvents.OPCServerSubscriptionEvent += RSACustomEvents_OPCSubscriptionEvent;
            //RSACustomEvents.KeepAliveTimeoutEvent += RSACustomEvents_KeepAliveTimeoutEvent;
            RSACustomEvents.ServiceConnectionEvent += RSACustomEvents_ServiceConnectionEvent;
            RSACustomEvents.ServiceDisconnectionEvent += RSACustomEvents_ServiceDisconnectionEvent;
            RSACustomEvents.ServiceStopEvent += RSACustomEvents_ServiceStopEvent;
            RSACustomEvents.SpeedIsChangedEvent += RSACustomEvents_SpeedIsChangedEvent;
        }

        private void RSACustomEvents_SpeedIsChangedEvent(object sender, RSACustomEvents.SpeedIsChangedEventArgs e)
        {

        }



        private void RSACustomEvents_ServiceStopEvent(object sender, RSACustomEvents.ServiceStopEventArgs e)
        {
            if (e.Service is Kawasaki kw)
            {
                myCore.Log?.Warn($"{kw.Name} is stopped");

                //if (lbLedRobot1Connection.InvokeRequired)
                //{
                //    lbLedRobot1Connection.Invoke((MethodInvoker)delegate
                //    {
                //        //lbLedRobotConnection.State = LBLed.LedState.Off;
                //        lbLedRobot1Connection.Visible = false;
                //    });
                //}
            }
        }

        private void RSACustomEvents_ServiceDisconnectionEvent(object sender, RSACustomEvents.ServiceConnectionEventArgs e)
        {
            if (e.Service is IRobot<KawasakiMemoryVariable>)
            {

                pictureBoxR1Node.Image = imageListNodes.Images[5];
            }
            else if (e.Service is OpcServerService opcService) //In questo caso no nfaccio niente perchè non ho modo, solo Subscription ( per ora )
            {

            }
            else if (e.Service is MySQLService mySQLService)
            {

            }

        }

        private void RSACustomEvents_ServiceConnectionEvent(object sender, RSACustomEvents.ServiceConnectionEventArgs e)
        {
            if (e.Service is IRobot<KawasakiMemoryVariable>)
            {


            }
            else if (e.Service is OpcServerService opcService) //In questo caso non faccio niente perchè non ho modo, solo Subscription ( per ora )
            {

            }
            else if (e.Service is WebApiCore api)
            {

            }
        }

        public void SaveSettings()
        {
            guiConfigurator.AddValue("LOGIN", "PWD", loginPassword, true);
            guiConfigurator.AddValue("T0", "R1INCLUSION", (MachineRuntime.r1.GetRobotInclusion() ? "1" : "0"), true);
            guiConfigurator.AddValue("T0", "R1INCLUSION_CLEANING", (MachineRuntime.r1.GetRobotInclusionCleaning() ? "1" : "0"), true);
            guiConfigurator.AddValue("T0", "R2INCLUSION", (MachineRuntime.r2.GetRobotInclusion() ? "1" : "0"), true);
            guiConfigurator.AddValue("T0", "SCALEINCLUSION", (MachineRuntime.gdevice1.GetDeviceInclusion() ? "1" : "0"), true);
            guiConfigurator.AddValue("T0", "RFIDMODELNAME", MachineRuntime.rfidModelNameG1A1.ToString(), true);
            guiConfigurator.AddValue("T0", "RFIDSIZE", MachineRuntime.rfidSizeG1A1.ToString(), true);
            guiConfigurator.AddValue("T0", "ROTARYMODELNAME", comboBoxRFIDLoadRotary_T0.Text, true);
            guiConfigurator.AddValue("T0", "AUTOREADINGTIMER", MachineRuntime.lineTimerReadingAuto.ToString(), true);
            guiConfigurator.AddValue("T0", "EMPTYPALLETTIMER", MachineRuntime.lineTimerEmpty.ToString(), true);
            guiConfigurator.AddValue("T0", "READINGTIMEOUTTIMER", MachineRuntime.lineTimerTimeout.ToString(), true);


            //guiConfigurator.AddValue("T0", "OVEN1INCLUSION", (MachineRuntime.gdevice1.GetDeviceInclusion() ? "1" : "0"), true);
            //guiConfigurator.AddValue("T0", "OVEN2INCLUSION", (MachineRuntime.gdevice2.GetDeviceInclusion() ? "1" : "0"), true);

            //guiConfigurator.AddValue("T0", "READINGMODE", MachineRuntime.lineReadingAuto.ToString(), true);
            //guiConfigurator.AddValue("T0", "READINGUNLOADMODE", MachineRuntime.lineUnloadReadingAuto.ToString(), true);

            guiConfigurator.AddValue("T0", "GD1PARAM1", MachineRuntime.gdevice1.GetDeviceParam1(), true);
            guiConfigurator.AddValue("T0", "GD1PARAM2", MachineRuntime.gdevice1.GetDeviceParam2(), true);

            guiConfigurator.AddValue("T0", "PCOUNTER", textBoxLastCounter.Text, true);


            //guiConfigurator.AddValue("T0", "OVEN1TIMER", MachineRuntime.gdevice1.GetDeviceParam4(), true);

            //guiConfigurator.AddValue("T0", "OVEN2SETPOINT1", MachineRuntime.gdevice2.GetDeviceParam1(), true);
            //guiConfigurator.AddValue("T0", "OVEN2SETPOINT2", MachineRuntime.gdevice2.GetDeviceParam2(), true);
            //guiConfigurator.AddValue("T0", "OVEN2TIMER", MachineRuntime.gdevice2.GetDeviceParam4(), true);





            guiConfigurator.Save("guiconfig.xml", Configurator.FileType.Xml);
        }
        public void CoreIsStartedCallBack(object sender, EventArgs args)
        {
            loginPassword = guiConfigurator.GetValue("LOGIN", "PWD", "RSA");
            //R1 inclusion
            string value = guiConfigurator.GetValue("T0", "R1INCLUSION", "RSA");
            checkBoxR1Inclusion.CheckState = (value == "1") ? CheckState.Checked : CheckState.Unchecked;
            //R1 cleaning inclusion
            value = guiConfigurator.GetValue("T0", "R1INCLUSION_CLEANING", "RSA");
            checkBoxR1InclusionCleaning.CheckState = (value == "1") ? CheckState.Checked : CheckState.Unchecked;
            //R2 inclusion
            value = guiConfigurator.GetValue("T0", "R2INCLUSION", "RSA");
            checkBoxR2Inclusion.CheckState = (value == "1") ? CheckState.Checked : CheckState.Unchecked;
            //scale
            value = guiConfigurator.GetValue("T0", "SCALEINCLUSION", "RSA");
            checkBoxGD1.CheckState = (value == "1") ? CheckState.Checked : CheckState.Unchecked;
            //model name
            value = guiConfigurator.GetValue("T0", "RFIDMODELNAME", "RSA");
            MachineRuntime.rfidModelNameG1A1 = value;
            comboBoxRFIDModelName_T0.Text = value;
            //size
            value = guiConfigurator.GetValue("T0", "RFIDSIZE", "RSA");
            MachineRuntime.rfidSizeG1A1 = value;
            numericUpDownSize_T0.Text = value;
            //rotary model name
            value = guiConfigurator.GetValue("T0", "ROTARYMODELNAME", "RSA");
            comboBoxRFIDLoadRotary_T0.Text = value;
            //timer
            value = guiConfigurator.GetValue("T0", "AUTOREADINGTIMER", "RSA");
            numericUpDownRFIDAutoReading.Value = Convert.ToInt32(value);
            MachineRuntime.lineTimerReadingAuto = short.Parse(numericUpDownRFIDAutoReading.Value.ToString());

            value = guiConfigurator.GetValue("T0", "EMPTYPALLETTIMER", "RSA");
            numericUpDownRFIDOnEmptyPallet.Value = Convert.ToInt32(value);
            MachineRuntime.lineTimerEmpty = short.Parse(numericUpDownRFIDOnEmptyPallet.Value.ToString());

            value = guiConfigurator.GetValue("T0", "READINGTIMEOUTTIMER", "RSA");
            numericUpDownRFIDReadingTimeout.Value = Convert.ToInt32(value);
            MachineRuntime.lineTimerTimeout = short.Parse(numericUpDownRFIDReadingTimeout.Value.ToString());





            //value = guiConfigurator.GetValue("T0", "OVEN2INCLUSION", "RSA");
            //checkBoxOven2.CheckState = (value == "1") ? CheckState.Checked : CheckState.Unchecked;

            //value = guiConfigurator.GetValue("T0", "READINGMODE", "RSA");
            //if (value == "1")
            //{
            //    radioAutomaticReading.Checked = true;
            //    radioManualReading.Checked = false;
            //}
            //else
            //{
            //    radioAutomaticReading.Checked = false;
            //    radioManualReading.Checked = true;
            //}
            //MachineRuntime.lineReadingAuto = short.Parse(value);

            //value = guiConfigurator.GetValue("T0", "READINGUNLOADMODE", "RSA");
            //if (value == "1")
            //{ 
            //    radioUnloadAutomaticReading.Checked = true;
            //    radioUnloadManualReading.Checked = false;
            //}
            //else
            //{
            //    radioUnloadAutomaticReading.Checked = false;
            //    radioUnloadManualReading.Checked = true;
            //}
            //MachineRuntime.lineUnloadReadingAuto = short.Parse(value);            

            //value = guiConfigurator.GetValue("T0", "OVEN2TIMER", "RSA");
            //numericUpDownGD2Param7.Value = Convert.ToInt32(value);



            value = guiConfigurator.GetValue("T0", "GD1PARAM1", "RSA");
            numericUpDownGD1Param1.Value = Convert.ToDecimal(float.Parse(value.ToString()));

            value = guiConfigurator.GetValue("T0", "GD1PARAM2", "RSA");
            numericUpDownGD1Param2.Value = Convert.ToDecimal(float.Parse(value.ToString()));

            value = guiConfigurator.GetValue("T0", "PCOUNTER", "RSA");
            textBoxLastCounter.Text = Convert.ToDecimal(float.Parse(value.ToString())).ToString();




            //opcua connected
            MachineRuntime.r1.SetRobotInclusion((checkBoxR1Inclusion.CheckState == CheckState.Checked) ? true : false);
            MachineRuntime.r2.SetRobotInclusion((checkBoxR2Inclusion.CheckState == CheckState.Checked) ? true : false);
            MachineRuntime.gdevice1.SetDeviceInclusion((checkBoxGD1.CheckState == CheckState.Checked) ? true : false);

            MachineRuntime.gdevice1.SetDeviceParam1(numericUpDownGD1Param1.Value.ToString());
            MachineRuntime.gdevice1.SetDeviceParam1(numericUpDownGD1Param2.Value.ToString());

            //MachineRuntime.gdevice2.SetDeviceParam2(numericUpDownGD2Param2.Value.ToString());
            //MachineRuntime.gdevice2.SetDeviceParam3(numericUpDownGD2Param3.Value.ToString());
            //MachineRuntime.gdevice2.SetDeviceParam4(numericUpDownGD2Param7.Value.ToString());
            SendOPCUATopicOnRestart();
        }

        public void StartUpdateTask()
        {
            Task.Run(async () => await UpdateOPCUAStatus(TimeSpan.FromMilliseconds(1000), _cancellationTokenSource));
            Task.Run(async () => await UpdateGUI(TimeSpan.FromMilliseconds(500), _cancellationTokenSource));
            Task.Run(async () => await UpdateOPCUAReadOnly(TimeSpan.FromMilliseconds(500), _cancellationTokenSource));
        }

        private void RSACustomEvents_ServiceHasLoadProgramEvent(object sender, RSACustomEvents.ProgramsReadEndedEventArgs e)
        {
            if (e.Service is ReadProgramsService progRS)
            {
                //modelCombobox.Items.Clear();
                //modelCombobox.Items.AddRange(progRS.ModelDictionary.Keys.ToArray<string>());
            }
        }

        private void tabControlMain_SelectedPageChanged(object sender, LidorSystems.IntegralUI.ObjectEventArgs e)
        {
            if (this.tabControlMain.SelectedPage != null)
            {
                LidorSystems.IntegralUI.Containers.TabPage nextPage = null;
                this.tabControlMain.SelectedPage.TabShape = TabShape.Rectangular;
                this.tabControlMain.SelectedPage.TabStripPlacement = TabStripPlacement.Left;
                LidorSystems.IntegralUI.Containers.TabPage parentPage = this.tabControlMain.SelectedPage;

                if (parentPage.Pages.Count != 0) nextPage = parentPage.Pages[0];
                if (nextPage != null)
                {

                    this.tabControlMain.SelectedPage = nextPage;
                }

                if (nextPage != null)
                {
                    if (tabControlMain.SelectedPage.Index == 0)
                    {
                        tabControlMain.SelectedPage.ContextMenuStrip.Show();
                    }
                    nextPage.ResumeLayout();
                }
                parentPage.SuspendLayout();

                if (parentPage.Index == 0)
                {

                }

                if (parentPage.Index == 2)
                {

                }

                if (parentPage.Index == 3)
                {

                }

                if (parentPage.Index == 4)
                {

                }

                if (parentPage.Index == 5)
                {
                    this.WindowState = FormWindowState.Minimized;
                }

                if (parentPage.Index == 6)
                {
                    //ask user to close application
                    DialogResult res = xDialog.MsgBox.Show("are you sure you want to exit from application?", "FMSS PL", xDialog.MsgBox.Buttons.YesNo);
                    if (res == DialogResult.Yes)
                    {
                        SaveSettings();

                        //exit from application
                        myCore?.StopAllService();

                        timerDeviceConnection.Enabled = false;
                        timerRePaint.Enabled = false;
                        timerRobotsStatus.Enabled = false;

                        timerDeviceConnection.Stop();
                        timerRePaint.Stop();
                        timerRobotsStatus.Stop();

                        if (System.Windows.Forms.Application.MessageLoop)
                        {
                            System.Windows.Forms.Application.ExitThread();
                            // WinForms app
                            System.Windows.Forms.Application.Exit();
                        }
                        else
                        {
                            // Console app
                            System.Environment.Exit(1);
                        }

                        int i = 0;
                        for (i = 0; i < tabControlMain.Pages.Count; i++)
                        {
                            tabControlMain.Pages[i].Dispose();
                        }
                        tabControlMain.Pages.Clear();
                        tabControlMain.Dispose();
                    }
                }
            }
            else
            {

            }
        }

        private void RunKeyboard()
        {
            Process foo = new Process();

            foo.StartInfo.FileName = @AppDomain.CurrentDomain.BaseDirectory + "RSAKeyboard.exe.lnk";

            //foo.StartInfo.Arguments = " 100 500 1 ";
            var processExists = Process.GetProcesses().Any(p => p.ProcessName.Contains("RSAKeyboard.exe.lnk"));

            if (processExists)
            {
                //TODO: Switch to foo.exe process
                foo.CloseMainWindow();
                foo.Start();
            }
            else
            {
                foo.Start();
            }
        }

        private void checkBoxT0Keyboard_Click(object sender, EventArgs e)
        {
            RunKeyboard();
        }

        private void checkBoxRecipeNewKey_CheckedChanged(object sender, EventArgs e)
        {
            RunKeyboard();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            RunKeyboard();
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            RunKeyboard();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            RunKeyboard();
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            RunKeyboard();
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            RunKeyboard();
        }

        private void checkBox4_Click(object sender, EventArgs e)
        {
            RunKeyboard();
        }

        private void checkBox5_Click(object sender, EventArgs e)
        {
            RunKeyboard();
        }

        private void timerRePaint_Tick(object sender, EventArgs e)
        {
            if (tabControlMain.SelectedPage == tabPageT0)
                tabPageT0.Invalidate(false);

            //if (tabControlMain.SelectedPage == tabPageT4_1)
            //    tabPageT4_1.Invalidate(false);

            //if (tabControlMain.SelectedPage == tabPageT4_2)
            //    tabPageT4_2.Invalidate(false);

            //if (tabControlMain.SelectedPage == tabPageT4_3)
            //    tabPageT4_3.Invalidate(false);

            //if (tabControlMain.SelectedPage == tabPageT4_4)
            //    tabPageT4_4.Invalidate(false);

            //if (tabControlMain.SelectedPage == tabPageT4_5)
            //    tabPageT4_5.Invalidate(false);

            //if (tabControlMain.SelectedPage == tabPageT4_6)
            //    tabPageT4_6.Invalidate(false);
        }

        private void tabPageT0_1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush bRuntime = new SolidBrush(Color.Black);
            Pen pRuntime = new Pen(Color.LightGray, 2);
            Brush bText = new SolidBrush(Color.Black);
            Brush bTextBold = new SolidBrush(Color.Black);

            Pen pBlue = new Pen(Color.Blue, 10);
            Brush bBlue = new SolidBrush(Color.Blue);

            Pen pBlack = new Pen(Color.Black, 10);
            Brush bBlack = new SolidBrush(Color.Black);

            Pen pRed = new Pen(Color.Red, 10);
            Brush bRed = new SolidBrush(Color.Red);

            Pen pOrange = new Pen(Color.Orange, 10);
            Brush bOrange = new SolidBrush(Color.Red);

            Pen pLightGreen = new Pen(Color.LightGreen, 10);
            Brush bLightGreen = new SolidBrush(Color.LightGreen);

            g.DrawRectangle(pRuntime, 150, 150, 400, 200);
            //g.FillEllipse(bRuntime, new Rectangle(new Point(wCircle, hCircle), new Size(wCircle, hCircle)));


            //if (MachineRuntime.lineStatus == 0)
            //{
            //    pRuntime = pRed;
            //    bRuntime = bRed;
            //}

            //if (MachineRuntime.lineStatus == 1)
            //{
            //    pRuntime = pLightGreen;
            //    bRuntime = bLightGreen;
            //}

            //if (MachineRuntime.lineStatus == 2)
            //{
            //    pRuntime = pOrange;
            //    bRuntime = bOrange;
            //}

            //if (MachineRuntime.lineStatus == 3)
            //{
            //    pRuntime = pBlue;
            //    bRuntime = bBlue;
            //}

            //if (MachineRuntime.lineStatus == 4)
            //{
            //    pRuntime = pRed;
            //    bRuntime = bRed;
            //}

            //g.DrawEllipse(pRuntime, wCircle, hCircle, wCircle, hCircle);
            //g.FillEllipse(bRuntime, new Rectangle(new Point(wCircle, hCircle), new Size(wCircle, hCircle)));
            //g.DrawString("line in " + GetDescriptionFromMachineState(MachineRuntime.lineStatus), new Font("Verdana", 14), bRuntime, new Point(wCircle * 2 + 5, hCircle / 2));
            g.ResetTransform();

            // g.DrawString(DateTime.Now.ToString(), new Font("Verdana", 12), bBlack, new Point(20, 680));
        }

        public string GetDescriptionFromMachineState(short state)
        {
            if (state == 0) return "emergency";
            if (state == 1) return "automatic";
            if (state == 2) return "manual";
            if (state == 3) return "cycle";
            if (state == 4) return "alarm";

            return "";
        }

        private void timerRobotsStatus_Tick(object sender, EventArgs e)
        {
            pictureBoxR1Node.Image = (AlarmsDictionary[6] == 1) ? imageListNodes.Images[5] : imageListNodes.Images[4];
            pictureBoxR2Node.Image = (AlarmsDictionary[9] == 1) ? imageListNodes.Images[5] : imageListNodes.Images[4];
            pictureBoxR3Node.Image = (AlarmsDictionary[12] == 1) ? imageListNodes.Images[5] : imageListNodes.Images[4];

            pictureBoxWagoR1Node.Image = (AlarmsDictionary[7] == 1) ? imageListNodes.Images[8] : imageListNodes.Images[9];
            pictureBoxWagoR2Node.Image = (AlarmsDictionary[10] == 1) ? imageListNodes.Images[8] : imageListNodes.Images[9];
            pictureBoxWagoR3Node.Image = (AlarmsDictionary[13] == 1) ? imageListNodes.Images[8] : imageListNodes.Images[9];

            pictureBoxIOTNode.Image = (AlarmsDictionary[20] == 1) ? imageListNodes.Images[3] : imageListNodes.Images[2];
            pictureBoxWagoLineNode.Image = (AlarmsDictionary[1] == 1) ? imageListNodes.Images[8] : imageListNodes.Images[9];
            pictureBoxPLCSecurityNode.Image = (AlarmsDictionary[19] == 1) ? imageListNodes.Images[1] : imageListNodes.Images[0];

            textBoxRFIDCodeUnload.Text = MachineRuntime.rfidCodeG1A4;
            textBoxRFIDModelNameUnload.Text = MachineRuntime.rfidModelNameG1A4;
            textBoxRFIDSizeUnload.Text = MachineRuntime.rfidSizeG1A4;
            textBoxRFIDFootUnload.Text = MachineRuntime.rfidFootG1A4;
            textBoxLastCounter.Text = MachineRuntime.lineLastCounter.ToString();

            pictureBoxPLCNode.Image = imageListNodes.Images[0];
            //datetime
            labelDateTime.Text = DateTime.Now.ToString();
            MachineRuntime.rfidModelNameG1A1 = comboBoxRFIDModelName_T0.Text;
            MachineRuntime.rfidSizeG1A1 = numericUpDownSize_T0.Value.ToString();
        }

        private void tabPageT0_Paint(object sender, PaintEventArgs e)
        {
            Pen blackPen = new Pen(Color.Black, 2);
            Brush blackBrush = new SolidBrush(Color.Black);
            Pen rectBorderPen = new Pen(Color.FromArgb(80, 157, 187), 2);
            rectBorderPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

            Pen rectBorderOfflinePen = new Pen(Color.Black, 2);
            rectBorderOfflinePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

            Pen whitePen = new Pen(Color.WhiteSmoke, 2);
            Brush whiteSmokeBrush = new SolidBrush(Color.WhiteSmoke);
            Font whiteFont = new Font("Verdana", 14);

            Brush windowsTextBrush = new SolidBrush(Color.Black);
            Pen windowsTextPen = new Pen(Color.Black, 2);

            Pen redPen = new Pen(Color.Red, 2);
            Brush redBrush = new SolidBrush(Color.Red);

            Pen orangePen = new Pen(Color.Orange, 2);
            Brush orangeBrush = new SolidBrush(Color.Orange);

            Pen greenPen = new Pen(Color.LightGreen, 2);
            Brush greenBrush = new SolidBrush(Color.LightGreen);

            Pen darkGreenPen = new Pen(Color.DarkGreen, 2);
            Brush darkGreenBrush = new SolidBrush(Color.DarkGreen);

            Pen yellowPen = new Pen(Color.Yellow, 2);
            Brush yellowBrush = new SolidBrush(Color.Yellow);

            Brush darkGrayBrush = new SolidBrush(Color.DarkGray);

            Pen groupBoxLineOnTop = new Pen(Color.FromArgb(63, 124, 203), 4);
            Pen groupBoxLineInAlarmTop = new Pen(Color.Red, 4);
            Pen groupBoxLineOffTop = new Pen(Color.Black, 4);
            Pen runtimePen = new Pen(Color.Black);

            Brush groupBoxFillColorOn = new SolidBrush(Color.FromArgb(242, 243, 245));

            Graphics myGraphics = e.Graphics;
            Rectangle rectBox = new Rectangle(0, 0, 1100, 180);

            myGraphics.ResetTransform();
            myGraphics.TranslateTransform(20, 20);
            //rfid 
            myGraphics.DrawRectangle(rectBorderPen, rectBox);
            myGraphics.FillRectangle(groupBoxFillColorOn, rectBox);
            myGraphics.DrawLine(groupBoxLineOnTop, new Point(0, 0), new Point(rectBox.Width, 0));
            myGraphics.TranslateTransform(1140, 0);


            #region(* general control *)
            Rectangle rectControlBoxGC = new Rectangle(0, 0, 520, 180);
            myGraphics.DrawRectangle(rectBorderPen, rectControlBoxGC);
            myGraphics.FillRectangle(groupBoxFillColorOn, rectControlBoxGC);
            myGraphics.DrawString("general control", whiteFont, windowsTextBrush, new Point(20, 20));
            myGraphics.DrawLine(windowsTextPen, new Point(20, 45), new Point(500, 45));
            myGraphics.DrawLine(groupBoxLineOnTop, new Point(0, 0), new Point(rectControlBoxGC.Width, 0));
            #endregion

            #region ( R1 drawing *)
            myGraphics.ResetTransform();
            myGraphics.TranslateTransform(20, 240);

            Rectangle rectControlBoxR1 = new Rectangle(0, 0, 260, 540);
            myGraphics.DrawRectangle(rectBorderPen, rectControlBoxR1);
            myGraphics.FillRectangle(groupBoxFillColorOn, rectControlBoxR1);

            //myGraphics.DrawLine(groupBoxLineOnTop, new Point(0, 0), new Point(rectControlBoxR1.Width, 0));
            if (MachineRuntime.r1.GetRobotIsDisconnected() == false)
            {
                myGraphics.DrawString(R1.Default.R1Name, whiteFont, windowsTextBrush, new Point(20, 20));
                myGraphics.DrawLine(windowsTextPen, new Point(20, 45), new Point(240, 45));
                runtimePen = groupBoxLineOnTop;
            }
            else
            {
                myGraphics.DrawString(R1.Default.R1Name + " disconnected", whiteFont, windowsTextBrush, new Point(20, 20));
                myGraphics.DrawLine(windowsTextPen, new Point(20, 45), new Point(240, 45));
                runtimePen = groupBoxLineOffTop;
            }
            myGraphics.DrawLine(runtimePen, new Point(0, 0), new Point(rectControlBoxR1.Width, 0));
            #endregion

            #region ( R2 drawing *)
            myGraphics.ResetTransform();
            myGraphics.TranslateTransform(300, 0);
            myGraphics.TranslateTransform(20, 240);

            Rectangle rectControlBoxR2 = new Rectangle(0, 0, 260, 540);
            myGraphics.DrawRectangle(rectBorderPen, rectControlBoxR2);
            myGraphics.FillRectangle(groupBoxFillColorOn, rectControlBoxR2);


            if (MachineRuntime.r2.GetRobotIsDisconnected() == false)
            {
                myGraphics.DrawString(R2.Default.R2Name, whiteFont, windowsTextBrush, new Point(20, 20));
                myGraphics.DrawLine(windowsTextPen, new Point(20, 45), new Point(240, 45));
                runtimePen = groupBoxLineOnTop;
            }
            else
            {
                myGraphics.DrawString(R2.Default.R2Name + " disconnected", whiteFont, windowsTextBrush, new Point(20, 20));
                myGraphics.DrawLine(windowsTextPen, new Point(20, 45), new Point(240, 45));
                runtimePen = groupBoxLineOffTop;
            }
            myGraphics.DrawLine(runtimePen, new Point(0, 0), new Point(rectControlBoxR2.Width, 0));


            #endregion

            #region ( OVEN 1 drawing *)
            myGraphics.ResetTransform();
            myGraphics.TranslateTransform(600, 0);
            myGraphics.TranslateTransform(20, 240);
            Rectangle rectControlBoxGD1 = new Rectangle(0, 0, 360, 540);
            myGraphics.DrawRectangle(rectBorderPen, rectControlBoxGD1);
            myGraphics.FillRectangle(groupBoxFillColorOn, rectControlBoxGD1);


            if (MachineRuntime.gdevice1.gdIsDisconnected == false)
            {
                myGraphics.DrawString(GDevice1.Default.GD1Name, whiteFont, windowsTextBrush, new Point(20, 20));
                myGraphics.DrawLine(windowsTextPen, new Point(20, 45), new Point(340, 45));
                runtimePen = groupBoxLineOnTop;
            }
            else
            {
                myGraphics.DrawString(GDevice1.Default.GD1Name + " disconnected", whiteFont, windowsTextBrush, new Point(20, 20));
                myGraphics.DrawLine(windowsTextPen, new Point(20, 45), new Point(340, 45));
                runtimePen = groupBoxLineOffTop;
            }

            if (MachineRuntime.gdevice1.gdIsInAlarm == true)
            {
                myGraphics.DrawString(GDevice1.Default.GD1Name + " in alarm", whiteFont, windowsTextBrush, new Point(20, 20));
                myGraphics.DrawLine(windowsTextPen, new Point(20, 45), new Point(340, 45));
                runtimePen = groupBoxLineInAlarmTop;
            }

            myGraphics.DrawLine(runtimePen, new Point(0, 0), new Point(rectControlBoxGD1.Width, 0));
            #endregion

            #region ( R3 drawing *)
            myGraphics.ResetTransform();
            myGraphics.TranslateTransform(1000, 0);
            myGraphics.TranslateTransform(20, 240);

            Rectangle rectControlBoxR3 = new Rectangle(0, 0, 660, 540);
            myGraphics.DrawRectangle(rectBorderPen, rectControlBoxR3);
            myGraphics.FillRectangle(groupBoxFillColorOn, rectControlBoxR3);

            //myGraphics.DrawLine(groupBoxLineOnTop, new Point(0, 0), new Point(rectControlBoxR3.Width, 0));

            if (MachineRuntime.r3.GetRobotIsDisconnected() == false)
            {
                myGraphics.DrawString(R3.Default.R3Name, whiteFont, windowsTextBrush, new Point(20, 20));
                myGraphics.DrawLine(windowsTextPen, new Point(20, 45), new Point(640, 45));
                runtimePen = groupBoxLineOnTop;
            }
            else
            {
                myGraphics.DrawString(R3.Default.R3Name + " disconnected", whiteFont, windowsTextBrush, new Point(20, 20));
                myGraphics.DrawLine(windowsTextPen, new Point(20, 45), new Point(640, 45));
                runtimePen = groupBoxLineOffTop;
            }
            myGraphics.DrawLine(runtimePen, new Point(0, 0), new Point(rectControlBoxR3.Width, 0));

            #endregion

            //#region ( OVEN 2 drawing *)
            //myGraphics.ResetTransform();
            //myGraphics.TranslateTransform(1300, 0);
            //myGraphics.TranslateTransform(20, 240);
            //Rectangle rectControlBoxGD2 = new Rectangle(0, 0, 360, 540);
            //myGraphics.DrawRectangle(rectBorderPen, rectControlBoxGD2);
            //myGraphics.FillRectangle(groupBoxFillColorOn, rectControlBoxGD2);
            //runtimePen = new Pen(Color.Black);


            //if (MachineRuntime.gdevice2.gdIsDisconnected == false)
            //{
            //    myGraphics.DrawString(GDevice2.Default.GD2Name, whiteFont, windowsTextBrush, new Point(20, 20));
            //    myGraphics.DrawLine(windowsTextPen, new Point(20, 45), new Point(340, 45));
            //    runtimePen = groupBoxLineOnTop;
            //}
            //else
            //{
            //    myGraphics.DrawString(GDevice2.Default.GD2Name + " disconnected", whiteFont, windowsTextBrush, new Point(20, 20));
            //    myGraphics.DrawLine(windowsTextPen, new Point(20, 45), new Point(340, 45));
            //    runtimePen = groupBoxLineOffTop;
            //}

            //if (MachineRuntime.gdevice2.gdIsInAlarm == true)
            //{
            //    myGraphics.DrawString(GDevice1.Default.GD1Name + " in alarm", whiteFont, windowsTextBrush, new Point(20, 20));
            //    myGraphics.DrawLine(windowsTextPen, new Point(20, 45), new Point(340, 45));
            //    runtimePen = groupBoxLineInAlarmTop;
            //}

            //myGraphics.DrawLine(runtimePen, new Point(0, 0), new Point(rectControlBoxGD2.Width, 0));
            //#endregion

            #region ( timer drawing *)
            myGraphics.ResetTransform();
            myGraphics.TranslateTransform(320, 820);
            Rectangle rectControlBox10 = new Rectangle(0, 0, 260, 200);
            myGraphics.DrawRectangle(rectBorderPen, rectControlBox10);
            myGraphics.FillRectangle(groupBoxFillColorOn, rectControlBox10);

            myGraphics.DrawString("timer", whiteFont, windowsTextBrush, new Point(20, 20));
            myGraphics.DrawLine(windowsTextPen, new Point(20, 45), new Point(240, 45));
            myGraphics.DrawLine(groupBoxLineOnTop, new Point(0, 0), new Point(rectControlBox10.Width, 0));


            #endregion

            #region(* unload *)
            myGraphics.TranslateTransform(300, 0);
            Rectangle rectControlBoxUnload = new Rectangle(0, 0, 480, 200);
            myGraphics.DrawRectangle(rectBorderPen, rectControlBoxUnload);
            myGraphics.FillRectangle(groupBoxFillColorOn, rectControlBoxUnload);
            myGraphics.DrawString("unload", whiteFont, windowsTextBrush, new Point(20, 20));
            myGraphics.DrawLine(windowsTextPen, new Point(20, 45), new Point(340, 45));
            myGraphics.DrawLine(groupBoxLineOnTop, new Point(0, 0), new Point(rectControlBoxUnload.Width, 0));


            #endregion

            #region(* general status *)
            //draw control box
            myGraphics.ResetTransform();
            myGraphics.TranslateTransform(1140, 820);
            Rectangle rectControlBox3 = new Rectangle(0, 0, 540, 200);
            myGraphics.DrawRectangle(rectBorderPen, rectControlBox3);
            myGraphics.FillRectangle(groupBoxFillColorOn, rectControlBox3);
            myGraphics.DrawString("general status", whiteFont, windowsTextBrush, new Point(20, 20));
            myGraphics.DrawLine(windowsTextPen, new Point(20, 45), new Point(520, 45));
            myGraphics.DrawLine(groupBoxLineOnTop, new Point(0, 0), new Point(rectControlBox3.Width, 0));
            //main semaphore
            #endregion            
        }

        #region (* DB MANAGEMENT *)
        private bool LoadDatabases()
        {
            OleDbConnection connL = null;

            //loading all databases
            try
            {
                //init model name table
                RSA.DB.ModelTable myModelTableL = new RSA.DB.ModelTable(Properties.SettingsDB.Default.modelTableName, Properties.SettingsDB.Default.modelCodeField, Properties.SettingsDB.Default.modelR1FieldNameAux0_00, Properties.SettingsDB.Default.modelR1FieldNameAux1_00, Properties.SettingsDB.Default.modelR1FieldNameAux2_00,
                    Properties.SettingsDB.Default.modelR1FieldNameAux0_01, Properties.SettingsDB.Default.modelR1FieldNameAux1_01, Properties.SettingsDB.Default.modelR1FieldNameAux2_01,
                    Properties.SettingsDB.Default.modelR2FieldNameAux0_00, Properties.SettingsDB.Default.modelR2FieldNameAux1_00, Properties.SettingsDB.Default.modelR2FieldNameAux2_00,
                    Properties.SettingsDB.Default.modelR2FieldNameAux0_01, Properties.SettingsDB.Default.modelR2FieldNameAux1_01, Properties.SettingsDB.Default.modelR2FieldNameAux2_01,
                    Properties.SettingsDB.Default.modelR3FieldNameAux0_00, Properties.SettingsDB.Default.modelR3FieldNameAux1_00, Properties.SettingsDB.Default.modelR3FieldNameAux2_00,
                    Properties.SettingsDB.Default.modelR3FieldNameAux0_01, Properties.SettingsDB.Default.modelR3FieldNameAux1_01, Properties.SettingsDB.Default.modelR3FieldNameAux2_01,
                    Properties.SettingsDB.Default.modelR4FieldNameAux0_00, Properties.SettingsDB.Default.modelR4FieldNameAux1_00, Properties.SettingsDB.Default.modelR4FieldNameAux2_00,
                    Properties.SettingsDB.Default.modelR4FieldNameAux0_01, Properties.SettingsDB.Default.modelR4FieldNameAux1_01, Properties.SettingsDB.Default.modelR4FieldNameAux2_01
                    );

                //init barcode table
                RSA.DB.BarcodeTable myNoteTableL = new RSA.DB.BarcodeTable(Properties.SettingsDB.Default.noteTableName, Properties.SettingsDB.Default.noteCodeFieldName, Properties.SettingsDB.Default.noteRobotCodeFieldName, Properties.SettingsDB.Default.noteArticleFieldName,
                    Properties.SettingsDB.Default.noteTargetNumberedFieldName, Properties.SettingsDB.Default.noteDoneNumberedFieldName, Properties.SettingsDB.Default.noteLoadedNumberedFieldName, Properties.SettingsDB.Default.noteCustom1FieldName,
                    Properties.SettingsDB.Default.noteCustom2FieldName, Properties.SettingsDB.Default.noteCustom3FieldName, Properties.SettingsDB.Default.noteCustom4FieldName, Properties.SettingsDB.Default.noteCustom5FieldName);

                //init programs for archive table 1
                RSA.DB.ArchiveTable myArchiveTableR1 = new RSA.DB.ArchiveTable(Properties.SettingsDB.Default.archiveR1TableName,
                    Properties.SettingsDB.Default.archiveCodeFieldName, Properties.SettingsDB.Default.archiveAux0FieldName, Properties.SettingsDB.Default.archiveAux1FieldName,
                    Properties.SettingsDB.Default.archiveAux2FieldName, Properties.SettingsDB.Default.archiveAux3FieldName);

                RSA.DB.ParameterTable myParameterTableR1 = new RSA.DB.ParameterTable();
                RSA.DB.ParameterTable myParameterTableR2 = new RSA.DB.ParameterTable();

                //init programs for archive table 2
                RSA.DB.ArchiveTable myArchiveTableR2 = new RSA.DB.ArchiveTable(Properties.SettingsDB.Default.archiveR2TableName,
                    Properties.SettingsDB.Default.archiveCodeFieldName, Properties.SettingsDB.Default.archiveAux0FieldName, Properties.SettingsDB.Default.archiveAux1FieldName,
                    Properties.SettingsDB.Default.archiveAux2FieldName, Properties.SettingsDB.Default.archiveAux3FieldName);

                //init programs archive table 3
                RSA.DB.ArchiveTable myArchiveTableR3 = new RSA.DB.ArchiveTable(Properties.SettingsDB.Default.archiveR3TableName,
                   Properties.SettingsDB.Default.archiveCodeFieldName, Properties.SettingsDB.Default.archiveAux0FieldName, Properties.SettingsDB.Default.archiveAux1FieldName,
                   Properties.SettingsDB.Default.archiveAux2FieldName, Properties.SettingsDB.Default.archiveAux3FieldName);

                //init programs archive table 4
                RSA.DB.ArchiveTable myArchiveTableR4 = new RSA.DB.ArchiveTable(Properties.SettingsDB.Default.archiveR4TableName,
                   Properties.SettingsDB.Default.archiveCodeFieldName, Properties.SettingsDB.Default.archiveAux0FieldName, Properties.SettingsDB.Default.archiveAux1FieldName,
                   Properties.SettingsDB.Default.archiveAux2FieldName, Properties.SettingsDB.Default.archiveAux3FieldName);

                //init local rfid table
                RSA.DB.RFIDTable myRFIDTableL = new RSA.DB.RFIDTable(Properties.SettingsDB.Default.rfidTableName, Properties.SettingsDB.Default.rfidCodeFieldName,
                    Properties.SettingsDB.Default.rfidRobotCodeFieldName, Properties.SettingsDB.Default.rfidNoteFieldName, Properties.SettingsDB.Default.rfidSizeFieldName,
                    Properties.SettingsDB.Default.rfidTypeFieldName, Properties.SettingsDB.Default.rfidVariantFieldName, Properties.SettingsDB.Default.rfidStatusFieldName,
                    Properties.SettingsDB.Default.rfidParam1FieldName, Properties.SettingsDB.Default.rfidParam2FieldName, Properties.SettingsDB.Default.rfidParam3FieldName);

                //recipe D1 table
                RSA.DB.RecipeD1Table myRecipeD1TableL = new RSA.DB.RecipeD1Table(Properties.SettingsDB.Default.recipeD1TableName, Properties.SettingsDB.Default.recipeD1CodeFieldName, Properties.SettingsDB.Default.recipeD1Param1FieldName,
                    Properties.SettingsDB.Default.recipeD1Param2FieldName, Properties.SettingsDB.Default.recipeD1Param3FieldName, Properties.SettingsDB.Default.recipeD1Param4FieldName);

                //recipe D2 table
                RSA.DB.RecipeD2Table myRecipeD2TableL = new RSA.DB.RecipeD2Table(Properties.SettingsDB.Default.recipeD2TableName, Properties.SettingsDB.Default.recipeD2CodeFieldName, Properties.SettingsDB.Default.recipeD2Param1FieldName,
                    Properties.SettingsDB.Default.recipeD2Param2FieldName, Properties.SettingsDB.Default.recipeD2Param3FieldName, Properties.SettingsDB.Default.recipeD2Param4FieldName);

                //recipe D3 table
                RSA.DB.RecipeD3Table myRecipeD3TableL = new RSA.DB.RecipeD3Table(Properties.SettingsDB.Default.recipeD3TableName, Properties.SettingsDB.Default.recipeD3CodeFieldName, Properties.SettingsDB.Default.recipeD3Param1FieldName,
                    Properties.SettingsDB.Default.recipeD3Param2FieldName, Properties.SettingsDB.Default.recipeD3Param3FieldName, Properties.SettingsDB.Default.recipeD3Param4FieldName);

                //recipe D4 table
                RSA.DB.RecipeD4Table myRecipeD4TableL = new RSA.DB.RecipeD4Table(Properties.SettingsDB.Default.recipeD4TableName, Properties.SettingsDB.Default.recipeD4CodeFieldName, Properties.SettingsDB.Default.recipeD4Param1FieldName,
                    Properties.SettingsDB.Default.recipeD4Param2FieldName, Properties.SettingsDB.Default.recipeD4Param3FieldName, Properties.SettingsDB.Default.recipeD4Param4FieldName);

                //db connection
                OleDbConnection connR1 = new OleDbConnection("Provider=Microsoft.Jet.OleDb.4.0;" + "Data Source=" + Properties.SettingsFilesPath.Default.dbRemotePath1);
                OleDbConnection connR2 = new OleDbConnection("Provider=Microsoft.Jet.OleDb.4.0;" + "Data Source=" + Properties.SettingsFilesPath.Default.dbRemotePath2);
                OleDbConnection connR3 = new OleDbConnection("Provider=Microsoft.Jet.OleDb.4.0;" + "Data Source=" + Properties.SettingsFilesPath.Default.dbRemotePath3);
                OleDbConnection connR4 = new OleDbConnection("Provider=Microsoft.Jet.OleDb.4.0;" + "Data Source=" + Properties.SettingsFilesPath.Default.dbRemotePath4);

                connL = new OleDbConnection("Provider=Microsoft.Jet.OleDb.4.0;" + "Data Source=" + Properties.SettingsFilesPath.Default.dbLocalPath);

                //init db instance
                RSA.DB myDB_Remote1 = new RSA.DB(myParameterTableR1, connR1);
                RSA.DB myDB_Remote2 = new RSA.DB(myParameterTableR2, connR2);

                RSA.DB myDB_Remote3 = new RSA.DB(myArchiveTableR3, connR3);
                RSA.DB myDB_Remote4 = new RSA.DB(myArchiveTableR3, connR4);
                RSA.DB myDB_Local = new RSA.DB(myModelTableL, myRFIDTableL, myNoteTableL, myRecipeD1TableL, myRecipeD2TableL, myRecipeD3TableL, myRecipeD4TableL, connL);

                RSA.Kawasaki myRobot1 = new RSA.Kawasaki();
                RSA.Kawasaki myRobot3 = new RSA.Kawasaki();

                //init RSA instance
                myRSA = new RSA(myDB_Remote1, myDB_Remote2, myDB_Remote3, myDB_Remote4, myDB_Local, myRobot1, myRobot3);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region(* CLIENT/SERVER MANAGEMENT *)
        private bool StartAllServer()
        {
            //start all rsa servers
            try
            {
                myRSA.GetPLServer().ServerStart();
            }
            catch (Exception ex)
            {
                return false;
            }

            //start all rsa servers
            try
            {
                myRSA.GetPLServer().ServerStart2();
            }
            catch (Exception ex)
            {

                return false;
            }

            return true;
        }

        #endregion

        #region (* RFID MANAGEMENT *)
        private bool LoadG1RFIDReader()
        {
            RFIDReader tmpRFID = new RFIDReader();

            MachineRuntime.RFIDReaderList.Add(tmpRFID);
            //launch production station
            if (Properties.Simulation.Default.rfidG1Simulation == false)
            {
                if (Properties.Settings.Default.readerType == "BALLUFF")
                {
                    try
                    {
                        myRSA.GetG1Balluff().IPort1 = Convert.ToInt32(comboBoxAntennaPLP1.Text);
                        myRSA.GetG1Balluff().IPort2 = Convert.ToInt32(comboBoxAntennaPLP2.Text);
                        myRSA.GetG1Balluff().IPort3 = Convert.ToInt32(comboBoxAntennaPLP3.Text);
                        myRSA.GetG1Balluff().IPort4 = Convert.ToInt32(comboBoxAntennaPLP4.Text);
                        myRSA.GetG1Balluff().IAddress = Convert.ToString(ipAddressG1RFIDReader.Text);
                        if (myRSA.GetG1Balluff().Connect(1) == true)
                        {
                            lbLedConnectionG1.LedColor = Color.LightGreen;
                        }
                        else
                        {
                            lbLedConnectionG1.LedColor = Color.Red;
                        }
                    }
                    catch
                    {
                        return false;
                    }
                    timerDeviceConnection.Enabled = true;
                    timerDeviceConnection.Start();
                }
                else if (Properties.Settings.Default.readerType == "ASE")
                {
                    try
                    {
                        if (myRSA.GetG1ASE().BBConnect(Convert.ToString(ipAddressG1RFIDReader.Text), 3000, 255) == 0)
                        {
                            lbLedConnectionG1.LedColor = Color.LightGreen;
                        }
                        else
                        {
                            lbLedConnectionG1.LedColor = Color.Red;
                        }
                    }
                    catch
                    {
                        return false;
                    }
                    timerDeviceConnection.Enabled = true;
                    timerDeviceConnection.Start();
                }
            }

            return true;
        }

        private DBRetCode ManageRequestG1A1(string rfid)
        {
            DBRetCode retRFID = DBRetCode.RFID_NOT_READ;
            string size = "";
            string type = "";
            string variant = "";
            string robotCode = "";
            string note = "";

            //check rfid code
            if (rfid == "")
            {
                MachineRuntime.rfidCodeG1A1 = "";
                //MachineRuntime.rfidModelNameG1A1 = "";
                //MachineRuntime.rfidSizeG1A1 = "";
                MachineRuntime.rfidFootG1A1 = "";
                MachineRuntime.rfidVariantG1A1 = "";

                return retRFID;
            }

            try
            {
                //find rfid code association info
                retRFID = myRSA.GetDBL().GetRFIDTable().FindRFIDAssociationInfoByRFID(rfid, ref robotCode, ref size, ref type, ref variant, ref note, myRSA.GetDBL().GetConnection());
            }
            catch (Exception Ex)
            {
                MachineRuntime.rfidCodeG1A1 = "";
                MachineRuntime.rfidModelNameG1A1 = "";
                MachineRuntime.rfidSizeG1A1 = "";
                MachineRuntime.rfidFootG1A1 = "";
                MachineRuntime.rfidVariantG1A1 = "";
                return DBRetCode.RFID_ERROR;
            }

            //check rfid fields result
            if (retRFID == DBRetCode.RFID_NEW)
            {
                //missing recipe
                MachineRuntime.rfidCodeG1A1 = rfid;
                //MachineRuntime.rfidSizeG1A1 = "";
                MachineRuntime.rfidFootG1A1 = "";
                MachineRuntime.rfidVariantG1A1 = "";
                return DBRetCode.RFID_NEW;
            }
            else if (retRFID == DBRetCode.RFID_OLD)
            {
                //recipe founded
                MachineRuntime.rfidCodeG1A1 = rfid;
                //MachineRuntime.rfidSizeG1A1 = size;
                MachineRuntime.rfidFootG1A1 = type;
                MachineRuntime.rfidVariantG1A1 = variant;
                return DBRetCode.RFID_OLD;
            }

            return retRFID;
        }

        private DBRetCode ManageRequestG1A4(string rfid)
        {
            DBRetCode retRFID = DBRetCode.RFID_NOT_READ;
            string size = "";
            string type = "";
            string variant = "";
            string robotCode = "";
            string note = "";

            //check rfid code
            if (rfid == "")
            {
                MachineRuntime.rfidCodeG1A4 = "";
                MachineRuntime.rfidModelNameG1A4 = "";
                MachineRuntime.rfidSizeG1A4 = "";
                MachineRuntime.rfidFootG1A4 = "";
                MachineRuntime.rfidVariantG1A4 = "";

                return retRFID;
            }

            try
            {
                //find rfid code association info
                retRFID = myRSA.GetDBL().GetRFIDTable().FindRFIDAssociationInfoByRFID(rfid, ref robotCode, ref size, ref type, ref variant, ref note, myRSA.GetDBL().GetConnection());
            }
            catch (Exception Ex)
            {
                MachineRuntime.rfidCodeG1A4 = "";
                MachineRuntime.rfidModelNameG1A4 = "";
                MachineRuntime.rfidSizeG1A4 = "";
                MachineRuntime.rfidFootG1A4 = "";
                MachineRuntime.rfidVariantG1A4 = "";
                return DBRetCode.RFID_ERROR;
            }

            //check rfid fields result
            if (retRFID == DBRetCode.RFID_NEW)
            {
                //missing recipe
                MachineRuntime.rfidCodeG1A4 = rfid;
                MachineRuntime.rfidSizeG1A4 = "";
                MachineRuntime.rfidFootG1A4 = "";
                MachineRuntime.rfidVariantG1A4 = "";
                return DBRetCode.RFID_NEW;
            }
            else if (retRFID == DBRetCode.RFID_OLD)
            {
                //recipe founded
                MachineRuntime.rfidCodeG1A4 = rfid;
                MachineRuntime.rfidSizeG1A4 = size;
                MachineRuntime.rfidFootG1A4 = type;
                MachineRuntime.rfidVariantG1A4 = variant;
                return DBRetCode.RFID_OLD;
            }

            return retRFID;
        }

        private DBRetCode ManageRequestG1A2(string rfid)
        {
            DBRetCode retRFID = DBRetCode.RFID_NOT_READ;
            string size = "";
            string type = "";
            string variant = "";
            string robotCode = "";
            string note = "";

            //check rfid code
            if (rfid == "")
            {
                MachineRuntime.rfidCodeG1A2 = "";
                MachineRuntime.rfidModelNameG1A2 = "";
                MachineRuntime.rfidSizeG1A2 = "";
                MachineRuntime.rfidTypeFromPlcG1A2 = "";
                MachineRuntime.rfidVariantG1A2 = "";

                return retRFID;
            }

            try
            {
                //find rfid code association info
                retRFID = myRSA.GetDBL().GetRFIDTable().FindRFIDAssociationInfoByRFID(rfid, ref robotCode, ref size, ref type, ref variant, ref note, myRSA.GetDBL().GetConnection());
            }
            catch (Exception Ex)
            {
                MachineRuntime.rfidCodeG1A2 = "";
                MachineRuntime.rfidModelNameG1A2 = "";
                MachineRuntime.rfidSizeG1A2 = "";
                MachineRuntime.rfidTypeFromPlcG1A2 = "";
                MachineRuntime.rfidVariantG1A2 = "";
                return DBRetCode.RFID_ERROR;
            }

            //check rfid fields result
            if (retRFID == DBRetCode.RFID_NEW)
            {
                //missing recipe
                MachineRuntime.rfidCodeG1A2 = rfid;
                MachineRuntime.rfidSizeG1A2 = "";
                MachineRuntime.rfidTypeFromPlcG1A2 = "";
                MachineRuntime.rfidVariantG1A2 = "";
                return DBRetCode.RFID_NEW;
            }
            else if (retRFID == DBRetCode.RFID_OLD)
            {
                //recipe founded
                MachineRuntime.rfidCodeG1A2 = rfid;
                MachineRuntime.rfidModelNameG1A2 = robotCode;
                //MachineRuntime.rfidSizeG1A2 = size;
                //MachineRuntime.rfidFootG1A2 = type;
                //MachineRuntime.rfidVariantG1A2 = variant;
                return DBRetCode.RFID_OLD;
            }

            return retRFID;
        }
        #endregion

        private void timerDeviceConnection_Tick(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.readerType == "BALLUFF")
            {
                //check RFID reader connection
                if (myRSA.GetG1Balluff().isConnected() == false)
                {
                    //do nothing
                    lbLedConnectionG1.LedColor = Color.Red;
                    myRSA.GetG1Balluff().Disconnect();
                    myRSA.GetG1Balluff().Connect(1);
                }
                else
                {
                    lbLedConnectionG1.LedColor = Color.LightGreen;
                }
            }
            else
            {
            }
        }

        private async void checkBoxR1Inclusion_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService == null) return;
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcR1Inclusion";
                chkValue = (checkBoxR1Inclusion.CheckState == CheckState.Checked) ? true : false;
                MachineRuntime.r1.SetRobotInclusion(chkValue);
                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxR1Inclusion.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxR1Inclusion.ImageIndex = 2;
                }
            }
            else
            {

            }
        }

        private async void checkBoxR2Inclusion_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService == null) return;
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcR2Inclusion";
                chkValue = (checkBoxR2Inclusion.CheckState == CheckState.Checked) ? true : false;
                MachineRuntime.r2.SetRobotInclusion(chkValue);
                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxR2Inclusion.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxR2Inclusion.ImageIndex = 2;
                }
            }
            else
            {

            }
        }

        private async void checkBoxGD1_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService == null) return;
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcGD1Inclusion";
                chkValue = (checkBoxGD1.CheckState == CheckState.Checked) ? true : false;
                MachineRuntime.gdevice1.SetDeviceInclusion(chkValue);


                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxGD1.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxGD1.ImageIndex = 2;
                }
            }
            else
            {
                //todo manage
            }
        }

        private async void checkBoxOven2_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService == null) return;
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcGD2Inclusion";
                chkValue = (checkBoxOven2.CheckState == CheckState.Checked) ? true : false;
                MachineRuntime.gdevice2.SetDeviceInclusion(chkValue);
                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxOven2.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxOven2.ImageIndex = 2;
                }
            }
            else
            {

            }
        }

        private async void numericUpDownGD2Param1_ValueChanged(object sender, EventArgs e)
        {
            if (ccService == null) return;
            if (ccService.ClientIsConnected)
            {
                MachineRuntime.gdevice2.SetDeviceParam1(numericUpDownGD2Param1.Value.ToString());
                string keyToSend = "pcGD2Param1";
                var sendResult = await ccService.Send(keyToSend, float.Parse(numericUpDownGD2Param1.Value.ToString()));

                if (sendResult.OpcResult)
                {
                }
                else
                {

                }
            }
        }

        private async void numericUpDownGD2Param2_ValueChanged(object sender, EventArgs e)
        {
            if (ccService == null) return;
            if (ccService.ClientIsConnected)
            {
                MachineRuntime.gdevice2.SetDeviceParam2(numericUpDownGD2Param2.Value.ToString());
                string keyToSend = "pcGD2Param2";
                var sendResult = await ccService.Send(keyToSend, float.Parse(numericUpDownGD2Param2.Value.ToString()));

                if (sendResult.OpcResult)
                {
                }
                else
                {

                }
            }
        }

        private async void numericUpDownGD2Param4_ValueChanged(object sender, EventArgs e)
        {
            if (ccService == null) return;
            if (ccService.ClientIsConnected)
            {
                MachineRuntime.gdevice2.SetDeviceParam4(numericUpDownGD2Param7.Value.ToString());
                string keyToSend = "pcGD2Param4";
                var sendResult = await ccService.Send(keyToSend, float.Parse(numericUpDownGD2Param7.Value.ToString()));

                if (sendResult.OpcResult)
                {
                }
                else
                {

                }
            }
        }

        private async void numericUpDownRFIDAutoReading_ValueChanged(object sender, EventArgs e)
        {
            if (ccService == null) return;
            if (ccService.ClientIsConnected)
            {
                MachineRuntime.lineTimerReadingAuto = short.Parse(numericUpDownRFIDAutoReading.Value.ToString());
                string keyToSend = "pcTimerAutoReading";
                var sendResult = await ccService.Send(keyToSend, short.Parse(numericUpDownRFIDAutoReading.Value.ToString()));

                if (sendResult.OpcResult)
                {
                }
                else
                {

                }
            }
        }

        private async void numericUpDownRFIDOnEmptyPallet_ValueChanged(object sender, EventArgs e)
        {
            if (ccService == null) return;
            if (ccService.ClientIsConnected)
            {
                MachineRuntime.lineTimerEmpty = short.Parse(numericUpDownRFIDOnEmptyPallet.Value.ToString());
                string keyToSend = "pcTimerOnEmptyPallet";
                var sendResult = await ccService.Send(keyToSend, short.Parse(numericUpDownRFIDOnEmptyPallet.Value.ToString()));

                if (sendResult.OpcResult)
                {
                }
                else
                {

                }
            }
        }

        private async void numericUpDownRFIDReadingTimeout_ValueChanged(object sender, EventArgs e)
        {
            if (ccService == null) return;
            if (ccService.ClientIsConnected)
            {
                MachineRuntime.lineTimerTimeout = short.Parse(numericUpDownRFIDReadingTimeout.Value.ToString());
                string keyToSend = "pcTimerReadingTimeout";
                var sendResult = await ccService.Send(keyToSend, short.Parse(numericUpDownRFIDReadingTimeout.Value.ToString()));

                if (sendResult.OpcResult)
                {
                }
                else
                {

                }
            }
        }

        private bool ReadRequestG1(string readerType, int portNumber, ref string rfid)
        {
            string tmpRfid = "";
            bool ret = false;

            if (readerType == "BALLUFF")
            {
                try
                {
                    //read rfid code from port
                    if (myRSA.GetG1Balluff().Read(ref tmpRfid, portNumber) == true)
                    {
                        rfid = tmpRfid;
                        ret = true;
                    }
                    else
                    {
                        rfid = "";
                        ret = false;
                    }
                }
                catch (Exception Ex)
                {
                    throw new Exception("Errore di lettura rfid: " + Ex.Message);
                    ret = false;
                }
            }
            else
            {
                int err = myRSA.GetG1ASE().BBDataRequest(ref rfid, ref portNumber);
                if (err == 0) ret = true;
                else ret = false;
            }

            return ret;
        }

        private bool ReadRequestG2(string readerType, int portNumber, ref string rfid)
        {
            string tmpRfid = "";
            bool ret = false;

            if (readerType == "BALLUFF")
            {
                try
                {
                    //read rfid code from port
                    if (myRSA.GetG1Balluff().Read(ref tmpRfid, portNumber) == true)
                    {
                        rfid = tmpRfid;
                        ret = true;
                    }
                    else
                    {
                        rfid = "";
                        ret = false;
                    }
                }
                catch (Exception Ex)
                {
                    throw new Exception("Errore di lettura rfid: " + Ex.Message);
                    ret = false;
                }
            }
            else
            {
                int err = myRSA.GetG1ASE().BBDataRequest(ref rfid, ref portNumber);
                if (err == 0) ret = true;
                else ret = false;
            }

            return ret;
        }

        private void buttonRFIDModify_T0_Click(object sender, EventArgs e)
        {
            if (textBoxRFID_T0.Text == "")
            {
                return;
            }

            if (IsFormOpened()) { return; }

            if (MachineRuntime.rfidReadingResultG1A1 == 0 || MachineRuntime.rfidReadingResultG1A1 == 1 || MachineRuntime.rfidReadingResultG1A1 == -2)
            {
                frm.rfid = MachineRuntime.rfidCodeG1A1;
                if (MachineRuntime.rfidReadingResultG1A1 == 0)
                {
                    //new rfid
                    frm.size = "340";
                    frm.foot = "LF";
                    frm.readingResult = 0;
                }
                else
                {
                    //old rfid
                    frm.size = MachineRuntime.rfidSizeG1A1;
                    frm.foot = MachineRuntime.rfidFootG1A1;
                    frm.readingResult = 1;
                }

                frm.InitForm();
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    MachineRuntime.rfidSizeG1A1 = frm.size;
                    MachineRuntime.rfidFootG1A1 = frm.foot;
                    myRSA.GetDBL().GetRFIDTable().UpdateRFIDAssociationInfo(myRSA.GetDBL().GetConnection(), MachineRuntime.rfidCodeG1A1, MachineRuntime.rfidModelNameG1A1, MachineRuntime.rfidSizeG1A1, MachineRuntime.rfidFootG1A1);
                }
            }
            else
            {

            }
        }

        private void checkBoxKeyboard_T0_Click(object sender, EventArgs e)
        {
            RunKeyboard();
        }

        private void comboBoxRFIDModelName_T0_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxRFIDModelName_T0.SelectedIndex != -1)
                MachineRuntime.rfidModelNameG1A1 = comboBoxRFIDModelName_T0.Text;

            if (comboBoxRFIDModelName_T0.Text != "")
                SendRecipeInAutomatic(comboBoxRFIDModelName_T0.Text);
        }

        private async void radioManualReading_CheckedChanged(object sender, EventArgs e)
        {
            if (ccService == null) return;
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcAutoManMode";
                string value = "";
                if (radioManualReading.Checked)
                {
                    value = "0";
                    MachineRuntime.lineReadingAuto = 0;
                }
                else
                {
                    value = "1";
                    MachineRuntime.lineReadingAuto = 1;
                }
                var sendResult = await ccService.Send(keyToSend, value);

                if (sendResult.OpcResult)
                {
                }
                else
                {

                }
            }
        }

        private async void radioAutomaticReading_CheckedChanged(object sender, EventArgs e)
        {
            if (ccService == null) return;
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcAutoManMode";
                string value = "";
                if (radioAutomaticReading.Checked)
                {
                    value = "1";
                    MachineRuntime.lineReadingAuto = 1;
                }
                else
                {
                    value = "0";
                    MachineRuntime.lineReadingAuto = 0;
                }

                var sendResult = await ccService.Send(keyToSend, value);

                if (sendResult.OpcResult)
                {
                }
                else
                {

                }
            }
        }

        private void buttonAddNeRobotCodew_Click(object sender, EventArgs e)
        {
            if (textBoxManageRobotCode.Text == "")
            {
                MessageBox.Show("missing model name", "add new model name error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (textBoxManageRobotCode.Text.Length < 4)
            {
                MessageBox.Show(textBoxManageRobotCode.Text + " model name too short", "model name text lenght must be 4", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (textBoxManageRobotCode.Text.Length > 4)
            {
                MessageBox.Show(textBoxManageRobotCode.Text + " model name too long", "model name text lenght must be 4", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //get flags working programs
            List<string> flags = new List<string>();
            flags.Add((checkBoxR1Aux0_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR1Aux1_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR1Aux2_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR1Aux0_01.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR1Aux1_01.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR1Aux2_01.CheckState == CheckState.Checked) ? "1" : "0");

            flags.Add((checkBoxR2Aux0_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR2Aux1_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR2Aux2_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR2Aux0_01.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR2Aux1_01.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR2Aux2_01.CheckState == CheckState.Checked) ? "1" : "0");

            flags.Add((checkBoxR3Aux0_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR3Aux1_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR3Aux2_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR3Aux0_01.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR3Aux1_01.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR3Aux2_01.CheckState == CheckState.Checked) ? "1" : "0");

            flags.Add((checkBoxR4Aux0_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR4Aux1_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR4Aux2_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR4Aux0_01.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR4Aux1_01.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR4Aux2_01.CheckState == CheckState.Checked) ? "1" : "0");

            //add new model name/robot code record
            try
            {
                myRSA.GetDBL().GetModelTable().AddNewModelNameRecord(textBoxManageRobotCode.Text, flags, myRSA.GetDBL().GetConnection());
                //update all robot code combobox
                InitRefreshModelNameList();
                MessageBox.Show(textBoxManageRobotCode.Text + " succesfully added", "add new model name", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(" error adding " + textBoxManageRobotCode.Text, "add new model name", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonEditDeleteRobotCode_Click(object sender, EventArgs e)
        {
            DialogResult res = xDialog.MsgBox.Show("are you sure you want to delete " + comboBoxSearchRobotCode.Text + " from database?", "FMSS PL", xDialog.MsgBox.Buttons.YesNo);
            if (res == DialogResult.Yes)
            {
                try
                {
                    myRSA.GetDBL().GetModelTable().DeleteModelNameRecordByName(comboBoxSearchRobotCode.Text, myRSA.GetDBL().GetConnection());
                    InitRefreshModelNameList();
                    MessageBox.Show(comboBoxSearchRobotCode.Text + " succesfully deleted", "delete model name", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(" error deleting " + comboBoxSearchRobotCode.Text, "delete model name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


            }
        }

        private void buttonRefreshByRobotCode_Click(object sender, EventArgs e)
        {

        }

        private void comboBoxSearchRobotCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            //check robot code
            if (comboBoxSearchRobotCode.Text == "")
            {
                return;
            }

            List<string> flags = new List<string>();

            try
            {
                flags = myRSA.GetDBL().GetModelTable().GetModelNameRecord(comboBoxSearchRobotCode.Text, myRSA.GetDBL().GetConnection());
            }
            catch (Exception Ex)
            {

            }

            if (flags.Count == 0)
            {
                //todo message to user
                return;
            }

            //R2 working flag checkbox
            checkBoxEditR1Aux0_00.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR1Aux0_00));
            checkBoxEditR1Aux1_00.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR1Aux1_00));
            checkBoxEditR1Aux2_00.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR1Aux2_00));
            checkBoxEditR1Aux0_01.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR1Aux0_01));
            checkBoxEditR1Aux1_01.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR1Aux1_01));
            checkBoxEditR1Aux2_01.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR1Aux2_01));
            //R2 working flag label
            //labelEditR2Aux0_00.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR2Aux0_00));
            //labelEDitR2Aux1_00.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR2Aux1_00));
            //labelEditR2Aux2_00.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR2Aux2_00));
            //labelEDitR2Aux0_01.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR2Aux0_01));
            //labelEditR2Aux1_01.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR2Aux1_01));
            //labelEditR2Aux2_01.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR2Aux2_01));   

            checkBoxEditR1Aux0_00.CheckState = (flags[0] == "1") ? CheckState.Checked : CheckState.Unchecked;
            checkBoxEditR1Aux1_00.CheckState = (flags[1] == "1") ? CheckState.Checked : CheckState.Unchecked;
            checkBoxEditR1Aux2_00.CheckState = (flags[2] == "1") ? CheckState.Checked : CheckState.Unchecked;
            checkBoxEditR1Aux0_01.CheckState = (flags[3] == "1") ? CheckState.Checked : CheckState.Unchecked;
            checkBoxEditR1Aux1_01.CheckState = (flags[4] == "1") ? CheckState.Checked : CheckState.Unchecked;
            checkBoxEditR1Aux2_01.CheckState = (flags[5] == "1") ? CheckState.Checked : CheckState.Unchecked;

            checkBoxEditR1Aux0_00.BackgroundImage = (checkBoxEditR1Aux0_00.CheckState == CheckState.Unchecked) ? ProductionLaunch.Properties.Resources.ledoff : ProductionLaunch.Properties.Resources.ledon;
            checkBoxEditR1Aux1_00.BackgroundImage = (checkBoxEditR1Aux1_00.CheckState == CheckState.Unchecked) ? ProductionLaunch.Properties.Resources.ledoff : ProductionLaunch.Properties.Resources.ledon;
            checkBoxEditR1Aux2_00.BackgroundImage = (checkBoxEditR1Aux2_00.CheckState == CheckState.Unchecked) ? ProductionLaunch.Properties.Resources.ledoff : ProductionLaunch.Properties.Resources.ledon;
            checkBoxEditR1Aux0_01.BackgroundImage = (checkBoxEditR1Aux0_01.CheckState == CheckState.Unchecked) ? ProductionLaunch.Properties.Resources.ledoff : ProductionLaunch.Properties.Resources.ledon;
            checkBoxEditR1Aux1_01.BackgroundImage = (checkBoxEditR1Aux1_01.CheckState == CheckState.Unchecked) ? ProductionLaunch.Properties.Resources.ledoff : ProductionLaunch.Properties.Resources.ledon;
            checkBoxEditR1Aux2_01.BackgroundImage = (checkBoxEditR1Aux2_01.CheckState == CheckState.Unchecked) ? ProductionLaunch.Properties.Resources.ledoff : ProductionLaunch.Properties.Resources.ledon;


            //R2 working flag checkbox
            checkBoxEditR2Aux0_00.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR2Aux0_00));
            checkBoxEditR2Aux1_00.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR2Aux1_00));
            checkBoxEditR2Aux2_00.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR2Aux2_00));
            checkBoxEditR2Aux0_01.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR2Aux0_01));
            checkBoxEditR2Aux1_01.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR2Aux1_01));
            checkBoxEditR2Aux2_01.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR2Aux2_01));
            //R2 working flag label
            //labelEditR2Aux0_00.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR2Aux0_00));
            //labelEDitR2Aux1_00.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR2Aux1_00));
            //labelEditR2Aux2_00.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR2Aux2_00));
            //labelEDitR2Aux0_01.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR2Aux0_01));
            //labelEditR2Aux1_01.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR2Aux1_01));
            //labelEditR2Aux2_01.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR2Aux2_01));           

            checkBoxEditR2Aux0_00.CheckState = (flags[6] == "1") ? CheckState.Checked : CheckState.Unchecked;
            checkBoxEditR2Aux1_00.CheckState = (flags[7] == "1") ? CheckState.Checked : CheckState.Unchecked;
            checkBoxEditR2Aux2_00.CheckState = (flags[8] == "1") ? CheckState.Checked : CheckState.Unchecked;
            checkBoxEditR2Aux0_01.CheckState = (flags[9] == "1") ? CheckState.Checked : CheckState.Unchecked;
            checkBoxEditR2Aux1_01.CheckState = (flags[10] == "1") ? CheckState.Checked : CheckState.Unchecked;
            checkBoxEditR2Aux2_01.CheckState = (flags[11] == "1") ? CheckState.Checked : CheckState.Unchecked;

            checkBoxEditR2Aux0_00.BackgroundImage = (checkBoxEditR2Aux0_00.CheckState == CheckState.Unchecked) ? ProductionLaunch.Properties.Resources.ledoff : ProductionLaunch.Properties.Resources.ledon;
            checkBoxEditR2Aux1_00.BackgroundImage = (checkBoxEditR2Aux1_00.CheckState == CheckState.Unchecked) ? ProductionLaunch.Properties.Resources.ledoff : ProductionLaunch.Properties.Resources.ledon;
            checkBoxEditR2Aux2_00.BackgroundImage = (checkBoxEditR2Aux2_00.CheckState == CheckState.Unchecked) ? ProductionLaunch.Properties.Resources.ledoff : ProductionLaunch.Properties.Resources.ledon;
            checkBoxEditR2Aux0_01.BackgroundImage = (checkBoxEditR2Aux0_01.CheckState == CheckState.Unchecked) ? ProductionLaunch.Properties.Resources.ledoff : ProductionLaunch.Properties.Resources.ledon;
            checkBoxEditR2Aux1_01.BackgroundImage = (checkBoxEditR2Aux1_01.CheckState == CheckState.Unchecked) ? ProductionLaunch.Properties.Resources.ledoff : ProductionLaunch.Properties.Resources.ledon;
            checkBoxEditR2Aux2_01.BackgroundImage = (checkBoxEditR2Aux2_01.CheckState == CheckState.Unchecked) ? ProductionLaunch.Properties.Resources.ledoff : ProductionLaunch.Properties.Resources.ledon;

            //R3 working flag checkbox
            checkBoxEditR3Aux0_00.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR3Aux0_00));
            checkBoxEditR3Aux1_00.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR3Aux1_00));
            checkBoxEditR3Aux2_00.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR3Aux2_00));
            checkBoxEditR3Aux0_01.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR3Aux0_01));
            checkBoxEditR3Aux1_01.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR3Aux1_01));
            checkBoxEditR3Aux2_01.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR3Aux2_01));

            //labelEditR3Aux0_00.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR3Aux0_00));
            //labelEditR3Aux1_00.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR3Aux1_00));
            //labelEditR3Aux2_00.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR3Aux2_00));
            //labelEditR3Aux0_01.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR3Aux0_01));
            //labelEditR3Aux1_01.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR3Aux1_01));
            //labelEditR3Aux2_01.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR3Aux2_01));           

            checkBoxEditR3Aux0_00.CheckState = (flags[12] == "1") ? CheckState.Checked : CheckState.Unchecked;
            checkBoxEditR3Aux1_00.CheckState = (flags[13] == "1") ? CheckState.Checked : CheckState.Unchecked;
            checkBoxEditR3Aux2_00.CheckState = (flags[14] == "1") ? CheckState.Checked : CheckState.Unchecked;
            checkBoxEditR3Aux0_01.CheckState = (flags[15] == "1") ? CheckState.Checked : CheckState.Unchecked;
            checkBoxEditR3Aux1_01.CheckState = (flags[16] == "1") ? CheckState.Checked : CheckState.Unchecked;
            checkBoxEditR3Aux2_01.CheckState = (flags[17] == "1") ? CheckState.Checked : CheckState.Unchecked;

            checkBoxEditR3Aux0_00.BackgroundImage = (checkBoxEditR3Aux0_00.CheckState == CheckState.Unchecked) ? ProductionLaunch.Properties.Resources.ledoff : ProductionLaunch.Properties.Resources.ledon;
            checkBoxEditR3Aux1_00.BackgroundImage = (checkBoxEditR3Aux1_00.CheckState == CheckState.Unchecked) ? ProductionLaunch.Properties.Resources.ledoff : ProductionLaunch.Properties.Resources.ledon;
            checkBoxEditR3Aux2_00.BackgroundImage = (checkBoxEditR3Aux2_00.CheckState == CheckState.Unchecked) ? ProductionLaunch.Properties.Resources.ledoff : ProductionLaunch.Properties.Resources.ledon;
            checkBoxEditR3Aux0_01.BackgroundImage = (checkBoxEditR3Aux0_01.CheckState == CheckState.Unchecked) ? ProductionLaunch.Properties.Resources.ledoff : ProductionLaunch.Properties.Resources.ledon;
            checkBoxEditR3Aux1_01.BackgroundImage = (checkBoxEditR3Aux1_01.CheckState == CheckState.Unchecked) ? ProductionLaunch.Properties.Resources.ledoff : ProductionLaunch.Properties.Resources.ledon;
            checkBoxEditR3Aux2_01.BackgroundImage = (checkBoxEditR3Aux2_01.CheckState == CheckState.Unchecked) ? ProductionLaunch.Properties.Resources.ledoff : ProductionLaunch.Properties.Resources.ledon;

            //R4 working flag checkbox
            checkBoxEditR4Aux0_00.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR4Aux0_00));
            checkBoxEditR4Aux1_00.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR4Aux1_00));
            checkBoxEditR4Aux2_00.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR4Aux2_00));
            checkBoxEditR4Aux0_01.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR4Aux0_01));
            checkBoxEditR4Aux1_01.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR4Aux1_01));
            checkBoxEditR4Aux2_01.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR4Aux2_01));

            //labelEditR4Aux0_00.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR4Aux0_00));
            //labelEditR4Aux1_00.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR4Aux1_00));
            //labelEditR4Aux2_00.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR4Aux2_00));
            //labelEditR4Aux0_01.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR4Aux0_01));
            //labelEditR4Aux1_01.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR4Aux1_01));
            //labelEditR4Aux2_01.Visible = Convert.ToBoolean(Convert.ToInt32(Properties.Settings.Default.modelNameR4Aux2_01));

            checkBoxEditR4Aux0_00.CheckState = (flags[18] == "1") ? CheckState.Checked : CheckState.Unchecked;
            checkBoxEditR4Aux1_00.CheckState = (flags[19] == "1") ? CheckState.Checked : CheckState.Unchecked;
            checkBoxEditR4Aux2_00.CheckState = (flags[20] == "1") ? CheckState.Checked : CheckState.Unchecked;
            checkBoxEditR4Aux0_01.CheckState = (flags[21] == "1") ? CheckState.Checked : CheckState.Unchecked;
            checkBoxEditR4Aux1_01.CheckState = (flags[22] == "1") ? CheckState.Checked : CheckState.Unchecked;
            checkBoxEditR4Aux2_01.CheckState = (flags[23] == "1") ? CheckState.Checked : CheckState.Unchecked;

            checkBoxEditR4Aux0_00.BackgroundImage = (checkBoxEditR4Aux0_00.CheckState == CheckState.Unchecked) ? ProductionLaunch.Properties.Resources.ledoff : ProductionLaunch.Properties.Resources.ledon;
            checkBoxEditR4Aux1_00.BackgroundImage = (checkBoxEditR4Aux1_00.CheckState == CheckState.Unchecked) ? ProductionLaunch.Properties.Resources.ledoff : ProductionLaunch.Properties.Resources.ledon;
            checkBoxEditR4Aux2_00.BackgroundImage = (checkBoxEditR4Aux2_00.CheckState == CheckState.Unchecked) ? ProductionLaunch.Properties.Resources.ledoff : ProductionLaunch.Properties.Resources.ledon;
            checkBoxEditR4Aux0_01.BackgroundImage = (checkBoxEditR4Aux0_01.CheckState == CheckState.Unchecked) ? ProductionLaunch.Properties.Resources.ledoff : ProductionLaunch.Properties.Resources.ledon;
            checkBoxEditR4Aux1_01.BackgroundImage = (checkBoxEditR4Aux1_01.CheckState == CheckState.Unchecked) ? ProductionLaunch.Properties.Resources.ledoff : ProductionLaunch.Properties.Resources.ledon;
            checkBoxEditR4Aux2_01.BackgroundImage = (checkBoxEditR4Aux2_01.CheckState == CheckState.Unchecked) ? ProductionLaunch.Properties.Resources.ledoff : ProductionLaunch.Properties.Resources.ledon;



            if (MachineRuntime.robotCodePL == comboBoxSearchRobotCode.Text)
            {
                MachineRuntime.r1.SetRobotFlags(myRSA.GetDBL().GetModelTable().GetModelNameRecordByRobot(MachineRuntime.robotCodePL, 1, myRSA.GetDBL().GetConnection()));
                MachineRuntime.r2.SetRobotFlags(myRSA.GetDBL().GetModelTable().GetModelNameRecordByRobot(MachineRuntime.robotCodePL, 2, myRSA.GetDBL().GetConnection()));
                MachineRuntime.r3.SetRobotFlags(myRSA.GetDBL().GetModelTable().GetModelNameRecordByRobot(MachineRuntime.robotCodePL, 3, myRSA.GetDBL().GetConnection()));
                MachineRuntime.r4.SetRobotFlags(myRSA.GetDBL().GetModelTable().GetModelNameRecordByRobot(MachineRuntime.robotCodePL, 4, myRSA.GetDBL().GetConnection()));
            }
        }

        private void checkBoxR1Aux0_00_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBoxR1Aux0_00.CheckState == CheckState.Unchecked)
            {
                checkBoxR1Aux0_00.BackgroundImage = ProductionLaunch.Properties.Resources.ledoff;
            }
            else
            {
                checkBoxR1Aux0_00.BackgroundImage = ProductionLaunch.Properties.Resources.ledon;

            }
            //UpdateAllModelFlag(textBoxManageRobotCode.Text);
        }

        private void checkBoxR1Aux1_00_CheckStateChanged(object sender, EventArgs e)
        {

            if (checkBoxR1Aux1_00.CheckState == CheckState.Unchecked)
            {
                checkBoxR1Aux1_00.BackgroundImage = ProductionLaunch.Properties.Resources.ledoff;
            }
            else
            {
                checkBoxR1Aux1_00.BackgroundImage = ProductionLaunch.Properties.Resources.ledon;

            }
            //UpdateAllModelFlag(textBoxManageRobotCode.Text);

        }

        public void UpdateAllModelFlag(string modelName)
        {

            List<string> flags = new List<string>();

            flags.Add((checkBoxR1Aux0_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR1Aux1_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR1Aux2_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR1Aux0_01.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR1Aux1_01.CheckState == CheckState.Checked) ? "1" : "0");

            flags.Add("0");

            flags.Add((checkBoxR2Aux0_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR2Aux1_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR2Aux2_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR2Aux0_01.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR2Aux1_01.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add("0");

            flags.Add((checkBoxR3Aux0_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR3Aux1_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR3Aux2_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR3Aux0_01.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxR3Aux1_01.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add("0");

            myRSA.GetDBL().GetModelTable().UpdateModelNameRecord(modelName, flags, myRSA.GetDBL().GetConnection());

        }

        private void checkBoxR1Aux2_00_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBoxR1Aux2_00.CheckState == CheckState.Unchecked)
            {
                checkBoxR1Aux2_00.BackgroundImage = ProductionLaunch.Properties.Resources.ledoff;
            }
            else
            {
                checkBoxR1Aux2_00.BackgroundImage = ProductionLaunch.Properties.Resources.ledon;
            }
        }

        private void checkBoxR2Aux0_00_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBoxR2Aux0_00.CheckState == CheckState.Unchecked)
            {
                checkBoxR2Aux0_00.BackgroundImage = ProductionLaunch.Properties.Resources.ledoff;
            }
            else
            {
                checkBoxR2Aux0_00.BackgroundImage = ProductionLaunch.Properties.Resources.ledon;
            }
        }

        private void checkBoxR2Aux1_00_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBoxR2Aux1_00.CheckState == CheckState.Unchecked)
            {
                checkBoxR2Aux1_00.BackgroundImage = ProductionLaunch.Properties.Resources.ledoff;
            }
            else
            {
                checkBoxR2Aux1_00.BackgroundImage = ProductionLaunch.Properties.Resources.ledon;
            }
        }

        private void checkBoxR2Aux2_00_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBoxR2Aux2_00.CheckState == CheckState.Unchecked)
            {
                checkBoxR2Aux2_00.BackgroundImage = ProductionLaunch.Properties.Resources.ledoff;
            }
            else
            {
                checkBoxR2Aux2_00.BackgroundImage = ProductionLaunch.Properties.Resources.ledon;
            }
        }

        private void checkBoxR3Aux0_00_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBoxR3Aux0_00.CheckState == CheckState.Unchecked)
            {
                checkBoxR3Aux0_00.BackgroundImage = ProductionLaunch.Properties.Resources.ledoff;
            }
            else
            {
                checkBoxR3Aux0_00.BackgroundImage = ProductionLaunch.Properties.Resources.ledon;
            }
        }

        private void checkBoxR3Aux1_00_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBoxR3Aux1_00.CheckState == CheckState.Unchecked)
            {
                checkBoxR3Aux1_00.BackgroundImage = ProductionLaunch.Properties.Resources.ledoff;
            }
            else
            {
                checkBoxR3Aux1_00.BackgroundImage = ProductionLaunch.Properties.Resources.ledon;
            }
        }

        private void checkBoxR3Aux2_00_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBoxR3Aux2_00.CheckState == CheckState.Unchecked)
            {
                checkBoxR3Aux2_00.BackgroundImage = ProductionLaunch.Properties.Resources.ledoff;
            }
            else
            {
                checkBoxR3Aux2_00.BackgroundImage = ProductionLaunch.Properties.Resources.ledon;
            }
        }

        private void checkBoxR4Aux0_00_CheckStateChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxR4Aux1_00_CheckStateChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxR4Aux2_00_CheckStateChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxEditR1Aux0_00_CheckStateChanged(object sender, EventArgs e)
        {
            checkBoxEditR1Aux0_00.BackgroundImage = (checkBoxEditR1Aux0_00.CheckState == CheckState.Checked) ? ProductionLaunch.Properties.Resources.ledon : ProductionLaunch.Properties.Resources.ledoff;

            UpdateEditRobotCodeFlag(comboBoxSearchRobotCode.Text);
        }

        public void UpdateEditRobotCodeFlag(string robotCode)
        {
            List<string> flags = new List<string>();

            flags.Add((checkBoxEditR1Aux0_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxEditR1Aux1_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxEditR1Aux2_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxEditR1Aux0_01.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxEditR1Aux1_01.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxEditR1Aux2_01.CheckState == CheckState.Checked) ? "1" : "0");

            flags.Add((checkBoxEditR2Aux0_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxEditR2Aux1_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxEditR2Aux2_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxEditR2Aux0_01.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxEditR2Aux1_01.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxEditR2Aux2_01.CheckState == CheckState.Checked) ? "1" : "0");

            flags.Add((checkBoxEditR3Aux0_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxEditR3Aux1_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxEditR3Aux2_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxEditR3Aux0_01.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxEditR3Aux1_01.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxEditR3Aux2_01.CheckState == CheckState.Checked) ? "1" : "0");

            flags.Add((checkBoxEditR4Aux0_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxEditR4Aux1_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxEditR4Aux2_00.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxEditR4Aux0_01.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxEditR4Aux1_01.CheckState == CheckState.Checked) ? "1" : "0");
            flags.Add((checkBoxEditR4Aux2_01.CheckState == CheckState.Checked) ? "1" : "0");

            myRSA.GetDBL().GetModelTable().UpdateModelNameRecord(robotCode, flags, myRSA.GetDBL().GetConnection());

            if (MachineRuntime.robotCodePL == comboBoxSearchRobotCode.Text)
            {
                MachineRuntime.r1.SetRobotFlags(myRSA.GetDBL().GetModelTable().GetModelNameRecordByRobot(MachineRuntime.robotCodePL, 1, myRSA.GetDBL().GetConnection()));
                MachineRuntime.r2.SetRobotFlags(myRSA.GetDBL().GetModelTable().GetModelNameRecordByRobot(MachineRuntime.robotCodePL, 2, myRSA.GetDBL().GetConnection()));
                MachineRuntime.r3.SetRobotFlags(myRSA.GetDBL().GetModelTable().GetModelNameRecordByRobot(MachineRuntime.robotCodePL, 3, myRSA.GetDBL().GetConnection()));
                MachineRuntime.r4.SetRobotFlags(myRSA.GetDBL().GetModelTable().GetModelNameRecordByRobot(MachineRuntime.robotCodePL, 4, myRSA.GetDBL().GetConnection()));
            }
        }

        private void checkBoxEditR1Aux1_00_CheckStateChanged(object sender, EventArgs e)
        {
            checkBoxEditR1Aux1_00.BackgroundImage = (checkBoxEditR1Aux1_00.CheckState == CheckState.Checked) ? ProductionLaunch.Properties.Resources.ledon : ProductionLaunch.Properties.Resources.ledoff;

            UpdateEditRobotCodeFlag(comboBoxSearchRobotCode.Text);
        }

        private void checkBoxEditR1Aux2_00_CheckStateChanged(object sender, EventArgs e)
        {
            checkBoxEditR1Aux2_00.BackgroundImage = (checkBoxEditR1Aux2_00.CheckState == CheckState.Checked) ? ProductionLaunch.Properties.Resources.ledon : ProductionLaunch.Properties.Resources.ledoff;

            UpdateEditRobotCodeFlag(comboBoxSearchRobotCode.Text);
        }

        private void checkBoxEditR2Aux0_00_CheckStateChanged(object sender, EventArgs e)
        {
            checkBoxEditR2Aux0_00.BackgroundImage = (checkBoxEditR2Aux0_00.CheckState == CheckState.Checked) ? ProductionLaunch.Properties.Resources.ledon : ProductionLaunch.Properties.Resources.ledoff;

            UpdateEditRobotCodeFlag(comboBoxSearchRobotCode.Text);
        }

        private void checkBoxEditR2Aux1_00_CheckStateChanged(object sender, EventArgs e)
        {
            checkBoxEditR2Aux1_00.BackgroundImage = (checkBoxEditR2Aux1_00.CheckState == CheckState.Checked) ? ProductionLaunch.Properties.Resources.ledon : ProductionLaunch.Properties.Resources.ledoff;

            UpdateEditRobotCodeFlag(comboBoxSearchRobotCode.Text);
        }

        private void checkBoxEditR2Aux2_00_CheckStateChanged(object sender, EventArgs e)
        {
            checkBoxEditR2Aux2_00.BackgroundImage = (checkBoxEditR2Aux2_00.CheckState == CheckState.Checked) ? ProductionLaunch.Properties.Resources.ledon : ProductionLaunch.Properties.Resources.ledoff;

            UpdateEditRobotCodeFlag(comboBoxSearchRobotCode.Text);
        }

        private void checkBoxEditR3Aux0_00_CheckStateChanged(object sender, EventArgs e)
        {
            checkBoxEditR3Aux0_00.BackgroundImage = (checkBoxEditR3Aux0_00.CheckState == CheckState.Checked) ? ProductionLaunch.Properties.Resources.ledon : ProductionLaunch.Properties.Resources.ledoff;

            UpdateEditRobotCodeFlag(comboBoxSearchRobotCode.Text);
        }

        private void checkBoxEditR3Aux1_00_CheckStateChanged(object sender, EventArgs e)
        {
            checkBoxEditR3Aux1_00.BackgroundImage = (checkBoxEditR3Aux1_00.CheckState == CheckState.Checked) ? ProductionLaunch.Properties.Resources.ledon : ProductionLaunch.Properties.Resources.ledoff;

            UpdateEditRobotCodeFlag(comboBoxSearchRobotCode.Text);
        }

        private void checkBoxEditR3Aux2_00_CheckStateChanged(object sender, EventArgs e)
        {
            checkBoxEditR3Aux2_00.BackgroundImage = (checkBoxEditR3Aux2_00.CheckState == CheckState.Checked) ? ProductionLaunch.Properties.Resources.ledon : ProductionLaunch.Properties.Resources.ledoff;

            UpdateEditRobotCodeFlag(comboBoxSearchRobotCode.Text);
        }

        private void tabControlMain_ToolItemClicked(object sender, LidorSystems.IntegralUI.ObjectClickEventArgs e)
        {
            if (tabControlMain.ToolBar[0].Key == "RSA")
            {
                //stop all timers
                timerRobotsStatus.Stop();
                timerRePaint.Stop();
                timerDeviceConnection.Stop();

                //UpdateGUIStop();

                //save hmi settings
                //SaveHMISettings();
                //save hmi status
                SaveHMIStatus();

                if (Properties.SettingsKAWASAKI.Default.krcdllIsPresent)
                {
                    //disconnect from krcdll: R1
                    if (myRSA.GetCommuKawasakiR1() != null)
                        myRSA.GetCommuKawasakiR1().Disconnect();

                    //disconnect from krcdll: R3
                    if (myRSA.GetCommuKawasakiR3() != null)
                        myRSA.GetCommuKawasakiR3().Disconnect();
                }

                //exit from application
                myCore?.StopAllService();

                Client.Close();
                //exit application
                Environment.Exit(0);
            }
            else
            {
            }
        }

        public void SaveHMIStatus()
        {
            //save hmi status file hmiconfig.xml
            //hmiConfigurator.AddValue("MAINPAGE", "NOTE", comboBoxBarcode1.Text, true);
            //hmiConfigurator.AddValue("HELPPAGE", "LOGIN_LEVEL", loginLevel, true);
            //hmiConfigurator.AddValue("HELPPAGE", "LOGIN_PWD", loginPassword, true);
            //hmiConfigurator.AddValue("MAINPAGE", "R1L_INCLUSION", (((int)checkBoxR1LInclusion.CheckState)).ToString(), true);
            //hmiConfigurator.AddValue("MAINPAGE", "R1R_INCLUSION", (((int)checkBoxR1RInclusion.CheckState)).ToString(), true);
            //hmiConfigurator.AddValue("MAINPAGE", "R2L_INCLUSION", (((int)checkBoxR2LInclusion.CheckState)).ToString(), true);
            //hmiConfigurator.AddValue("MAINPAGE", "R2R_INCLUSION", (((int)checkBoxR2RInclusion.CheckState)).ToString(), true);
            //hmiConfigurator.AddValue("MAINPAGE", "R3L_INCLUSION", (((int)checkBoxR3LInclusion.CheckState)).ToString(), true);
            //hmiConfigurator.AddValue("MAINPAGE", "R3R_INCLUSION", (((int)checkBoxR3RInclusion.CheckState)).ToString(), true);
            //hmiConfigurator.AddValue("MAINPAGE", "R4L_INCLUSION", (((int)checkBoxR4LInclusion.CheckState)).ToString(), true);
            //hmiConfigurator.AddValue("MAINPAGE", "R4R_INCLUSION", (((int)checkBoxR4RInclusion.CheckState)).ToString(), true);
            //hmiConfigurator.AddValue("MAINPAGE", "D1L_INCLUSION", (((int)checkBoxD1LInclusion.CheckState)).ToString(), true);
            //hmiConfigurator.AddValue("MAINPAGE", "D1R_INCLUSION", (((int)checkBoxD1RInclusion.CheckState)).ToString(), true);
            //hmiConfigurator.AddValue("MAINPAGE", "D2L_INCLUSION", (((int)checkBoxD2LInclusion.CheckState)).ToString(), true);
            //hmiConfigurator.AddValue("MAINPAGE", "D2R_INCLUSION", (((int)checkBoxD2RInclusion.CheckState)).ToString(), true);
            //hmiConfigurator.AddValue("MAINPAGE", "D3L_INCLUSION", (((int)checkBoxD3LInclusion.CheckState)).ToString(), true);
            //hmiConfigurator.AddValue("MAINPAGE", "D3R_INCLUSION", (((int)checkBoxD3RInclusion.CheckState)).ToString(), true);
            //hmiConfigurator.AddValue("MAINPAGE", "D4L_INCLUSION", (((int)checkBoxD4LInclusion.CheckState)).ToString(), true);
            //hmiConfigurator.AddValue("MAINPAGE", "D4R_INCLUSION", (((int)checkBoxD4RInclusion.CheckState)).ToString(), true);
            //hmiConfigurator.AddValue("MAINPAGE", "GD1_INCLUSION", (((int)checkBoxGD1Inclusion.CheckState)).ToString(), true);
            //hmiConfigurator.AddValue("MAINPAGE", "GD2_INCLUSION", (((int)checkBoxGD2Inclusion.CheckState)).ToString(), true);
            //int mode = (radioManualReading.Checked == true) ? 0 : 1;

            //hmiConfigurator.AddValue("SETTINGPAGE", "PL_READING_MODE", mode.ToString(), true);
            //hmiConfigurator.AddValue("SETTINGPAGE", "PL_READING_TIMEOUT", (((int)numericUpDownRFIDReadingTimeoutBis.Value)).ToString(), true);
            //hmiConfigurator.AddValue("SETTINGPAGE", "PL_READING_TIMER_ON_AUTO", (((int)numericUpDownTimerAutoModeBis.Value)).ToString(), true);

            //hmiConfigurator.AddValue("SETTINGPAGE", "GD1_PARAM1", (((int)numericUpDownGD1Param1.Value)).ToString(), true);
            //hmiConfigurator.AddValue("SETTINGPAGE", "GD1_PARAM2", (((int)numericUpDownGD1Param2.Value)).ToString(), true);
            //hmiConfigurator.AddValue("SETTINGPAGE", "GD1_PARAM3", (((int)numericUpDownGD1Param3.Value)).ToString(), true);
            //hmiConfigurator.AddValue("SETTINGPAGE", "GD1_PARAM4", (((int)numericUpDownGD1Param4.Value)).ToString(), true);
            //hmiConfigurator.AddValue("SETTINGPAGE", "GD1_PARAM5", (((int)numericUpDownGD1Param5.Value)).ToString(), true);
            //hmiConfigurator.AddValue("SETTINGPAGE", "GD1_PARAM6", (((int)numericUpDownGD1Param6.Value)).ToString(), true);
            //hmiConfigurator.AddValue("SETTINGPAGE", "GD1_PARAM7", (((int)numericUpDownGD1Param7.Value)).ToString(), true);

            //hmiConfigurator.AddValue("SETTINGPAGE", "GD2_PARAM1", (((int)numericUpDownGD2Param1.Value)).ToString(), true);
            //hmiConfigurator.AddValue("SETTINGPAGE", "GD2_PARAM2", (((int)numericUpDownGD2Param2.Value)).ToString(), true);
            //hmiConfigurator.AddValue("SETTINGPAGE", "GD2_PARAM3", (((int)numericUpDownGD2Param3.Value)).ToString(), true);
            //hmiConfigurator.AddValue("SETTINGPAGE", "GD2_PARAM4", (((int)numericUpDownGD2Param4.Value)).ToString(), true);
            //hmiConfigurator.AddValue("SETTINGPAGE", "GD2_PARAM5", (((int)numericUpDownGD2Param5.Value)).ToString(), true);
            //hmiConfigurator.AddValue("SETTINGPAGE", "GD2_PARAM6", (((int)numericUpDownGD2Param6.Value)).ToString(), true);
            //hmiConfigurator.AddValue("SETTINGPAGE", "GD2_PARAM7", (((int)numericUpDownGD2Param7.Value)).ToString(), true);

            //hmiConfigurator.Save("hmiconfig.xml", Configurator.FileType.Xml);
        }

        private void buttonResetRotary_Click(object sender, EventArgs e)
        {
            DialogResult result = MsgBox.Show("Are you sure to reset ROBOTS buffer programs?", "Exit", MsgBox.Buttons.YesNo, MsgBox.Icon.Info);

            if (result == DialogResult.Yes)
            {
                if (Properties.Simulation.Default.R1Simulation == false) myRSA.GetPLServer().SendCommandToRobot(IPAddress.Parse("172.31.10.146"), "ROTARY_EMPTY;", 9999);
                if (Properties.Simulation.Default.R2Simulation == false) myRSA.GetPLServer().SendCommandToRobot(IPAddress.Parse("172.31.10.166"), "ROTARY_EMPTY;", 9999);
                //if (Properties.Simulation.Default.R3Simulation == false) myRSA.GetPLServer().SendCommandToRobot(IPAddress.Parse("172.31.10.166"), "ROTARY_EMPTY;", 9999);
            }
        }

        private void buttonLoadRotary_Click(object sender, EventArgs e)
        {
            DialogResult result = MsgBox.Show("Are you sure to LOAD ROBOTS buffer programs of model name" + comboBoxRFIDLoadRotary_T0.Text + "?", "Exit", MsgBox.Buttons.YesNo, MsgBox.Icon.Info);

            if (result == DialogResult.Yes)
            {
                if ((Properties.Simulation.Default.R1Simulation == false)) myRSA.GetPLServer().SendCommandToRobot(IPAddress.Parse("172.31.10.146"), "ROTARY_LOAD," + comboBoxRFIDLoadRotary_T0.Text + "," + "00" + ";", 9999);
                if ((Properties.Simulation.Default.R2Simulation == false)) myRSA.GetPLServer().SendCommandToRobot(IPAddress.Parse("172.31.10.166"), "ROTARY_LOAD," + comboBoxRFIDLoadRotary_T0.Text + "," + "00" + ";", 9999);
                //if ((Properties.Simulation.Default.R3Simulation == false)) myRSA.GetPLServer().SendCommandToRobot(IPAddress.Parse("172.31.10.166"), "ROTARY_LOAD," + comboBoxRFIDLoadRotary_T0.Text + "," + "00" + ";", 9999);
            }
        }

        private void buttonGD1RegisterAlarms_Click(object sender, EventArgs e)
        {
            string alarmString = "";
            alarmString = GetGD1RegisterAlarmsDescription(1, GD1AlarmsDictionary[1]) + "\r\n";
            alarmString = alarmString + GetGD1RegisterAlarmsDescription(2, GD1AlarmsDictionary[2]) + "\r\n";
            alarmString = alarmString + GetGD1RegisterAlarmsDescription(3, GD1AlarmsDictionary[3]) + "\r\n";
            alarmString = alarmString + GetGD1RegisterAlarmsDescription(4, GD1AlarmsDictionary[4]) + "\r\n";
            alarmString = alarmString + GetGD1RegisterAlarmsDescription(5, GD1AlarmsDictionary[5]) + "\r\n";
            alarmString = alarmString + GetGD1RegisterAlarmsDescription(6, GD1AlarmsDictionary[6]) + "\r\n";
            alarmString = alarmString + GetGD1RegisterAlarmsDescription(7, GD1AlarmsDictionary[7]) + "\r\n";

            if (GD1AlarmsDictionary[1] == 0 & GD1AlarmsDictionary[2] == 0 & GD1AlarmsDictionary[3] == 0 & GD1AlarmsDictionary[4] == 0 & GD1AlarmsDictionary[5] == 0 &
                GD1AlarmsDictionary[6] == 0 & GD1AlarmsDictionary[7] == 0)
            {
                MessageBox.Show("no active alarms", "scale alarms", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else MessageBox.Show(alarmString, GDevice1.Default.GD1Name + " in alarm", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void buttonGD2RegisterAlarms_Click(object sender, EventArgs e)
        {
            string alarmString = "";
            alarmString = GetGD2RegisterAlarmsDescription(1, GD2AlarmsDictionary[1]) + "\r\n";
            alarmString = alarmString + GetGD2RegisterAlarmsDescription(2, GD2AlarmsDictionary[2]) + "\r\n";
            alarmString = alarmString + GetGD2RegisterAlarmsDescription(3, GD2AlarmsDictionary[3]) + "\r\n";
            alarmString = alarmString + GetGD2RegisterAlarmsDescription(4, GD2AlarmsDictionary[4]) + "\r\n";
            alarmString = alarmString + GetGD2RegisterAlarmsDescription(5, GD2AlarmsDictionary[5]) + "\r\n";
            alarmString = alarmString + GetGD2RegisterAlarmsDescription(6, GD2AlarmsDictionary[6]) + "\r\n";
            alarmString = alarmString + GetGD2RegisterAlarmsDescription(7, GD2AlarmsDictionary[7]) + "\r\n";
            if (GD2AlarmsDictionary[1] == 0 & GD2AlarmsDictionary[2] == 0 & GD2AlarmsDictionary[3] == 0 & GD2AlarmsDictionary[4] == 0 & GD2AlarmsDictionary[5] == 0 &
               GD2AlarmsDictionary[6] == 0 & GD2AlarmsDictionary[7] == 0)
            {
                MessageBox.Show("no active alarms", "glue dehumidifier alarms", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else MessageBox.Show(alarmString, GDevice2.Default.GD2Name + " in alarm", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void lbLedRFIDResult_T0_Click(object sender, EventArgs e)
        {
            string browser = "msedge.exe";
            string url = "http://172.31.10.130";
            Process.Start(browser, url);
        }

        private void buttonManualRead_Click(object sender, EventArgs e)
        {
            //only in stop auto
            if (PLCLineOutputDictionary[8] == true)
            {
                MessageBox.Show("manual reading permitted when stop is pressed", "read RFID", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //go on read
            int antenna = 0;
            if (Properties.Settings.Default.readerType == "BALLUFF") antenna = myRSA.GetG1Balluff().IPort1;
            else antenna = 1;
            string tmpG1A1 = "";
            bool retG1A1 = false;
            try
            {
                retG1A1 = ReadRequestG1(MachineRuntime.RFIDReaderType, antenna, ref tmpG1A1);
            }
            catch (Exception ex)
            {
                retG1A1 = false;
            }
            if (retG1A1)
            {
                //rfid code
                MachineRuntime.rfidCodeG1A1 = tmpG1A1;
                //find rfid association
                DBRetCode ret = ManageRequestG1A1(MachineRuntime.rfidCodeG1A1);
                if (ret == DBRetCode.RFID_NOT_READ)
                {
                    MachineRuntime.rfidReadingResultG1A1 = -1;
                    MachineRuntime.rfidReadingResultStringG1A1 = "rfid reading error";
                }
                else if (ret == DBRetCode.RFID_OLD)
                {
                    MachineRuntime.rfidReadingResultG1A1 = 1;
                    //update RFID database table
                    myRSA.GetDBL().GetRFIDTable().UpdateRFIDAssociationInfoMN(myRSA.GetDBL().GetConnection(),
                        MachineRuntime.rfidCodeG1A1, MachineRuntime.rfidModelNameG1A1, MachineRuntime.rfidSizeG1A1);
                    MachineRuntime.rfidReadingResultStringG1A1 = "rfid succesfully read and associated";
                }
                else if (ret == DBRetCode.RFID_NEW)
                {
                    MachineRuntime.rfidReadingResultStringG1A1 = "rfid is NEW, association needed";
                    if (IsFormOpened())
                    {

                    }
                    else
                    {
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            MachineRuntime.rfidSizeG1A1 = frm.size;
                            MachineRuntime.rfidFootG1A1 = frm.foot;
                            myRSA.GetDBL().GetRFIDTable().AddNewRFIDAssociationInfo(myRSA.GetDBL().GetConnection(), MachineRuntime.rfidCodeG1A1,
                                MachineRuntime.rfidModelNameG1A1, "000", "XX");
                            MachineRuntime.rfidReadingResultStringG1A1 = "rfid succesfully read and associated";
                        }
                    }
                    MachineRuntime.rfidReadingResultG1A1 = 0;
                }
                else if (ret == DBRetCode.RFID_ERROR)
                {
                    MachineRuntime.rfidReadingResultG1A1 = 2;
                    MachineRuntime.rfidReadingResultStringG1A1 = "rfid reading error";
                }
            }
            else
            {
                //no tag
                MachineRuntime.rfidReadingResultG1A1 = 2;
                MachineRuntime.rfidReadingResultStringG1A1 = "rfid not present";
            }
        }

        private void tabPageT3_Paint(object sender, PaintEventArgs e)
        {
            Pen blackPen = new Pen(Color.Black, 2);
            Brush blackBrush = new SolidBrush(Color.Black);
            Pen rectBorderPen = new Pen(Color.FromArgb(80, 157, 187), 2);
            rectBorderPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

            Pen rectBorderOfflinePen = new Pen(Color.Black, 2);
            rectBorderOfflinePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

            Pen whitePen = new Pen(Color.WhiteSmoke, 2);
            Brush whiteSmokeBrush = new SolidBrush(Color.WhiteSmoke);
            Font whiteFont = new Font("Verdana", 14);

            Brush windowsTextBrush = new SolidBrush(Color.Black);
            Pen windowsTextPen = new Pen(Color.Black, 2);

            Pen redPen = new Pen(Color.Red, 2);
            Brush redBrush = new SolidBrush(Color.Red);

            Pen orangePen = new Pen(Color.Orange, 2);
            Brush orangeBrush = new SolidBrush(Color.Orange);

            Pen greenPen = new Pen(Color.LightGreen, 2);
            Brush greenBrush = new SolidBrush(Color.LightGreen);

            Pen darkGreenPen = new Pen(Color.DarkGreen, 2);
            Brush darkGreenBrush = new SolidBrush(Color.DarkGreen);

            Pen yellowPen = new Pen(Color.Yellow, 2);
            Brush yellowBrush = new SolidBrush(Color.Yellow);

            Brush darkGrayBrush = new SolidBrush(Color.DarkGray);

            Pen groupBoxLineOnTop = new Pen(Color.FromArgb(63, 124, 203), 4);
            Pen groupBoxLineInAlarmTop = new Pen(Color.Red, 4);
            Pen groupBoxLineOffTop = new Pen(Color.Black, 4);
            Pen runtimePen = new Pen(Color.Black);

            Brush groupBoxFillColorOn = new SolidBrush(Color.FromArgb(242, 243, 245));

            Graphics myGraphics = e.Graphics;
            Rectangle rectBox = new Rectangle(0, 0, 1680, 560);

            myGraphics.ResetTransform();
            myGraphics.TranslateTransform(20, 20);
            myGraphics.DrawRectangle(rectBorderPen, rectBox);
            myGraphics.FillRectangle(groupBoxFillColorOn, rectBox);
            myGraphics.DrawString("recipe settings", whiteFont, windowsTextBrush, new Point(20, 20));
            myGraphics.DrawLine(groupBoxLineOnTop, new Point(0, 0), new Point(rectBox.Width, 0));
            myGraphics.DrawLine(windowsTextPen, new Point(20, 45), new Point(1380, 45));
            //sole training
            Rectangle rectTrainingBox = new Rectangle(0, 0, 880, 420);
            myGraphics.ResetTransform();
            myGraphics.TranslateTransform(820, 600);
            myGraphics.DrawRectangle(rectBorderPen, rectTrainingBox);
            myGraphics.FillRectangle(groupBoxFillColorOn, rectTrainingBox);
            myGraphics.DrawString("sole training", whiteFont, windowsTextBrush, new Point(20, 20));
            myGraphics.DrawLine(groupBoxLineOnTop, new Point(0, 0), new Point(rectTrainingBox.Width, 0));
            myGraphics.DrawLine(windowsTextPen, new Point(20, 45), new Point(860, 45));

            Rectangle rectGCBox = new Rectangle(0, 0, 380, 420);
            //myGraphics.ResetTransform();
            //myGraphics.TranslateTransform(20, 600);
            //myGraphics.DrawRectangle(rectBorderPen, rectGCBox);
            //myGraphics.FillRectangle(groupBoxFillColorOn, rectGCBox);
            //myGraphics.DrawString("scanner device status", whiteFont, windowsTextBrush, new Point(20, 20));
            //myGraphics.DrawLine(groupBoxLineOnTop, new Point(0, 0), new Point(rectGCBox.Width, 0));
            //myGraphics.DrawLine(windowsTextPen, new Point(20, 45), new Point(640, 45));

            rectGCBox = new Rectangle(0, 0, 380, 420);
            myGraphics.ResetTransform();
            myGraphics.TranslateTransform(420, 600);
            myGraphics.DrawRectangle(rectBorderPen, rectGCBox);
            myGraphics.FillRectangle(groupBoxFillColorOn, rectGCBox);
            myGraphics.DrawString("scanner calibration", whiteFont, windowsTextBrush, new Point(20, 20));
            myGraphics.DrawLine(groupBoxLineOnTop, new Point(0, 0), new Point(rectGCBox.Width, 0));
            myGraphics.DrawLine(windowsTextPen, new Point(20, 45), new Point(360, 45));
        }

        private void buttonLineTimeoutAlarms_Click(object sender, EventArgs e)
        {
            if (MachineRuntime.lineInTimeout) tabControlMain.SelectedPage = tabPageT3;
            else MessageBox.Show("no active timeout alarms", "timeout alarms", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonResetCounter_Click(object sender, EventArgs e)
        {
            textBoxLastCounter.Text = "0";
        }

        private async void checkBoxR1Cleaning_CheckStateChanged(object sender, EventArgs e)
        {
            if (ccService == null) return;
            if (ccService.ClientIsConnected)
            {
                string keyToSend = null;
                bool chkValue = false;

                keyToSend = "pcR1CleaningInclusion";
                chkValue = (checkBoxR1InclusionCleaning.CheckState == CheckState.Checked) ? true : false;
                MachineRuntime.r1.SetRobotInclusionCleaning(chkValue);
                var sendResult = await ccService.Send(keyToSend, chkValue);

                if (sendResult.OpcResult)
                {
                    checkBoxR1InclusionCleaning.ImageIndex = (chkValue) ? 0 : 1;
                }
                else
                {
                    checkBoxR1InclusionCleaning.ImageIndex = 2;
                }
            }
            else
            {

            }
        }

        private async void numericUpDownGD1Param1_ValueChanged(object sender, EventArgs e)
        {
            if (ccService == null) return;
            if (ccService.ClientIsConnected)
            {
                MachineRuntime.gdevice1.SetDeviceParam1((numericUpDownGD1Param1.Value).ToString());
                string keyToSend = "pcGD1Param1";
                var sendResult = await ccService.Send(keyToSend, Convert.ToInt32((numericUpDownGD1Param1.Value / 10)));

                if (sendResult.OpcResult)
                {
                }
                else
                {

                }
            }
        }

        private async void numericUpDownGD1Param2_ValueChanged(object sender, EventArgs e)
        {
            if (ccService == null) return;
            if (ccService.ClientIsConnected)
            {
                MachineRuntime.gdevice1.SetDeviceParam2((numericUpDownGD1Param2.Value).ToString());
                string keyToSend = "pcGD1Param2";
                var sendResult = await ccService.Send(keyToSend, Convert.ToInt32((numericUpDownGD1Param2.Value / 10)));

                if (sendResult.OpcResult)
                {
                }
                else
                {

                }
            }
        }

        private async void buttonStartTareScale_Click(object sender, EventArgs e)
        {
            if (ccService == null) return;
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcGD1CommandTare";
                var sendResult = await ccService.Send(keyToSend, true);

                if (sendResult.OpcResult)
                {
                    Thread.Sleep(100);
                    sendResult = await ccService.Send(keyToSend, false);
                    if (sendResult.OpcResult)
                    {
                        MessageBox.Show("PL", GDevice1.Default.GD1Name + " tare done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else MessageBox.Show("PL", GDevice1.Default.GD1Name + " error doing tare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("PL", GDevice1.Default.GD1Name + " error doing tare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else MessageBox.Show("PL", "OPCUA: client not connected", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private async void buttonStartZeroScale_Click(object sender, EventArgs e)
        {
            if (ccService == null) return;
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcGD1CommandZero";
                var sendResult = await ccService.Send(keyToSend, true);

                if (sendResult.OpcResult)
                {
                    Thread.Sleep(100);
                    sendResult = await ccService.Send(keyToSend, false);
                    if (sendResult.OpcResult)
                    {
                        MessageBox.Show("PL", GDevice1.Default.GD1Name + " zero done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else MessageBox.Show("PL", GDevice1.Default.GD1Name + " error doing zero", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("PL", GDevice1.Default.GD1Name + " error doing zero", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else MessageBox.Show("PL", "OPCUA: client not connected", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private async void buttonPanelLockScale_Click(object sender, EventArgs e)
        {
            if (ccService == null) return;
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcGD1CommandLockHMI";
                var sendResult = await ccService.Send(keyToSend, true);

                if (sendResult.OpcResult)
                {
                    Thread.Sleep(100);
                    sendResult = await ccService.Send(keyToSend, false);
                    if (sendResult.OpcResult)
                    {
                        MessageBox.Show("PL", GDevice1.Default.GD1Name + " HMI locked", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else MessageBox.Show("PL", GDevice1.Default.GD1Name + " error doing HMI locking", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("PL", GDevice1.Default.GD1Name + " error doing HMI locking", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else MessageBox.Show("PL", "OPCUA: client not connected", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private async void buttonPanelUnlockScale_Click(object sender, EventArgs e)
        {
            if (ccService == null) return;
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcGD1CommandUnlockHMI";
                var sendResult = await ccService.Send(keyToSend, true);

                if (sendResult.OpcResult)
                {
                    Thread.Sleep(100);
                    sendResult = await ccService.Send(keyToSend, false);
                    if (sendResult.OpcResult)
                    {
                        MessageBox.Show("PL", GDevice1.Default.GD1Name + " HMI unlocked", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else MessageBox.Show("PL", GDevice1.Default.GD1Name + " error doing HMI unlocking", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("PL", GDevice1.Default.GD1Name + " error doing HMI unlocking", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else MessageBox.Show("PL", "OPCUA: client not connected", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void tabPageT4_6_Paint(object sender, PaintEventArgs e)
        {
            Pen blackPen = new Pen(Color.Black, 2);
            Brush blackBrush = new SolidBrush(Color.Black);
            Pen rectBorderPen = new Pen(Color.FromArgb(80, 157, 187), 2);
            rectBorderPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            Pen whitePen = new Pen(Color.WhiteSmoke, 2);
            Brush whiteSmokeBrush = new SolidBrush(Color.WhiteSmoke);
            Font whiteFont = new Font("Verdana", 14);

            Brush windowsTextBrush = new SolidBrush(Color.Black);
            Pen windowsTextPen = new Pen(Color.Black, 2);

            Pen redPen = new Pen(Color.Red, 2);
            Brush redBrush = new SolidBrush(Color.Red);

            Pen orangePen = new Pen(Color.Orange, 2);
            Brush orangeBrush = new SolidBrush(Color.Orange);

            Pen greenPen = new Pen(Color.LightGreen, 2);
            Brush greenBrush = new SolidBrush(Color.LightGreen);

            Pen darkGreenPen = new Pen(Color.DarkGreen, 2);
            Brush darkGreenBrush = new SolidBrush(Color.DarkGreen);

            Pen yellowPen = new Pen(Color.Yellow, 2);
            Brush yellowBrush = new SolidBrush(Color.Yellow);

            Graphics myGraphics = e.Graphics;
            Rectangle rectBox = new Rectangle(0, 0, 940, 120);
            Rectangle rectControlBox = new Rectangle(0, 0, 520, 160);
            myGraphics.ResetTransform();
            myGraphics.TranslateTransform(20, 20);
            //rfid 
            myGraphics.DrawRectangle(rectBorderPen, rectBox);
            myGraphics.DrawString("emergency buttons", whiteFont, windowsTextBrush, new Point(20, 20));
            myGraphics.DrawLine(windowsTextPen, new Point(20, 45), new Point(920, 45));

            myGraphics.TranslateTransform(0, 140);
            //rfid 
            myGraphics.DrawRectangle(rectBorderPen, rectBox);
            myGraphics.DrawString("timeout stop pallet NODE 1", whiteFont, windowsTextBrush, new Point(20, 20));
            myGraphics.DrawLine(windowsTextPen, new Point(20, 45), new Point(920, 45));

            myGraphics.TranslateTransform(0, 140);
            //rfid 
            myGraphics.DrawRectangle(rectBorderPen, rectBox);
            myGraphics.DrawString("timeout stop pallet NODE 2", whiteFont, windowsTextBrush, new Point(20, 20));
            myGraphics.DrawLine(windowsTextPen, new Point(20, 45), new Point(920, 45));

            myGraphics.TranslateTransform(0, 140);
            //rfid 
            myGraphics.DrawRectangle(rectBorderPen, rectBox);
            myGraphics.DrawString("timeout stop pallet NODE 3", whiteFont, windowsTextBrush, new Point(20, 20));
            myGraphics.DrawLine(windowsTextPen, new Point(20, 45), new Point(920, 45));

            myGraphics.TranslateTransform(0, 140);
            //rfid 
            myGraphics.DrawRectangle(rectBorderPen, rectBox);
            myGraphics.DrawString("timeout stop pallet NODE 4", whiteFont, windowsTextBrush, new Point(20, 20));
            myGraphics.DrawLine(windowsTextPen, new Point(20, 45), new Point(920, 45));
        }

        private async void buttonStartTraining_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcScannerStartTraining";
            var sendResult = await ccService.Send(keyToSend, true);
            //Thread.Sleep(300);
            //sendResult = await ccService.Send(keyToSend, false);

            if (sendResult.OpcResult)
            {
                //send training mode to scanner
                tcp.Write("MODE," + 1 + ",2,0,0,0,0,0,0,0,0,");
                Thread.Sleep(100);
                lbLedTrainingStarted.State = LBLed.LedState.On;
            }
            else lbLedTrainingStarted.State = LBLed.LedState.Off;
            //reset result led
            lbLedScannerResult.State = LBLed.LedState.Off;
        }

        private void buttonTRecipeUpdate_Click(object sender, EventArgs e)
        {
            //send mode to the scanner
            if (tcp == null)
            {
                MessageBox.Show("FMSS PL", "missing connection with scanner", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    if (tcp.IsConnected() == false)
                    {
                        MessageBox.Show("FMSS PL", "missing connection with scanner", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {

                        //send recipe to scanner
                        tcp.Write(
                            "RECIPE,1," + numericUpDownTParam2.Value.ToString() + "," + numericUpDownTParam3.Value.ToString() + "," +
                            numericUpDownTParam4.Value.ToString() + "," + numericUpDownTParam5.Value.ToString() + "," + "3," +
                            numericUpDownTParam6.Value.ToString() + "," + numericUpDownTParam7.Value.ToString() + "," +
                            numericUpDownTParam8.Value.ToString() + "," + numericUpDownTParam1.Value.ToString() + ",");
                        //update recipe on db
                        string fillingType = "3";
                        myRSA.GetDBL().GetModelTable().UpdateModelNameRParams(myRSA.GetDBL().GetConnection(), comboBoxTModelName.Text,
                            numericUpDownTParam1.Value.ToString(), numericUpDownTParam2.Value.ToString(), numericUpDownTParam3.Value.ToString(),
                            numericUpDownTParam4.Value.ToString(), fillingType, numericUpDownTParam5.Value.ToString(),
                            numericUpDownTParam6.Value.ToString(), numericUpDownTParam7.Value.ToString(), numericUpDownTParam8.Value.ToString());
                        MessageBox.Show("FMSS PL", "model name succesfully updated", MessageBoxButtons.OK);
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void buttonTSave_Click(object sender, EventArgs e)
        {
            if (comboBoxTModelName.Text == "")
            {
                MessageBox.Show("missing model name selection", "FMSS PL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //get program name
            string programName = "PR" + comboBoxTModelName.Text + "-" + numericUpDownTSize.Value.ToString() + "-" + comboBoxTType.Text + "00";

            try
            {
                if (!Properties.Simulation.Default.R1Simulation)
                {
                    //save parameters
                    DBRetCode ret = myRSA.GetDBR1().GetParameterTable().FindParameter(myRSA.GetDBR1().GetConnection(), programName);

                    if (ret != DBRetCode.PARAMETER_ERROR)
                    {
                        if (ret == DBRetCode.PARAMETER_NEW)
                        {
                            myRSA.GetDBR1().GetParameterTable().AddNewParameter(myRSA.GetDBR1().GetConnection(), programName,
                                textBoxXT.Text, textBoxYT.Text, textBoxZT.Text, textBoxRxT.Text, textBoxRyT.Text, textBoxRzT.Text, textBoxTLenght.Text
                                );
                        }
                        else if (ret == DBRetCode.PARAMETER_OLD)
                        {
                            myRSA.GetDBR1().GetParameterTable().UpdateParameter(myRSA.GetDBR1().GetConnection(), programName,
                                textBoxXT.Text, textBoxYT.Text, textBoxZT.Text, textBoxRxT.Text, textBoxRyT.Text, textBoxRzT.Text, textBoxTLenght.Text
                                );
                        }
                        //save file
                        if (checkBoxMasterPointR1.Checked)
                        {
                            //File.Copy("\\\\172.31.10.126\\PRODUCTION\\Scanner\\JOB1" + ".LAV", "C:\\PRODUCTION\\Programs\\R1\\" + programName + ".LAV", true);
                            //File.Copy("\\\\172.31.10.126\\PRODUCTION\\Scanner\\JOB1" + ".AU1", "C:\\PRODUCTION\\Programs\\R1\\" + programName + ".AU1", true);
                            //File.Copy("\\\\172.31.10.126\\PRODUCTION\\Scanner\\JOB1" + ".AU2", "C:\\PRODUCTION\\Programs\\R1\\" + programName + ".AU2", true);
                        }
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception ex)
            {

            }

            try
            {
                if (!Properties.Simulation.Default.R2Simulation)
                {
                    DBRetCode ret = myRSA.GetDBR2().GetParameterTable().FindParameter(myRSA.GetDBR2().GetConnection(), programName);

                    if (ret != DBRetCode.PARAMETER_ERROR)
                    {
                        if (ret == DBRetCode.PARAMETER_NEW)
                        {
                            myRSA.GetDBR2().GetParameterTable().AddNewParameter(myRSA.GetDBR2().GetConnection(), programName,
                                textBoxXT.Text, textBoxYT.Text, textBoxZT.Text, textBoxRxT.Text, textBoxRyT.Text, textBoxRzT.Text, textBoxTLenght.Text
                                );
                        }
                        else if (ret == DBRetCode.PARAMETER_OLD)
                        {
                            myRSA.GetDBR2().GetParameterTable().UpdateParameter(myRSA.GetDBR2().GetConnection(), programName,
                                textBoxXT.Text, textBoxYT.Text, textBoxZT.Text, textBoxRxT.Text, textBoxRyT.Text, textBoxRzT.Text, textBoxTLenght.Text
                                );
                        }
                        if (checkBoxMasterPointR2.Checked)
                        {
                            //File.Copy("\\\\172.31.10.126\\PRODUCTION\\Scanner\\JOB1" + ".LAV", "C:\\PRODUCTION\\Programs\\R2\\" + programName + ".LAV", true);
                            //File.Copy("\\\\172.31.10.126\\PRODUCTION\\Scanner\\JOB1" + ".AU1", "C:\\PRODUCTION\\Programs\\R2\\" + programName + ".AU1", true);
                            //File.Copy("\\\\172.31.10.126\\PRODUCTION\\Scanner\\JOB1" + ".AU2", "C:\\PRODUCTION\\Programs\\R2\\" + programName + ".AU2", true);
                        }
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception ex)
            {

            }

            //save a copy on master prog folder
            try
            {
                File.Copy("\\\\172.31.10.126\\PRODUCTION\\Scanner\\JOB1" + ".LAV", "C:\\PRODUCTION\\Programs\\master_prog\\" + programName + ".LAV", true);
                File.Copy("\\\\172.31.10.126\\PRODUCTION\\Scanner\\JOB1" + ".AU1", "C:\\PRODUCTION\\Programs\\master_prog\\" + programName + ".AU1", true);
                File.Copy("\\\\172.31.10.126\\PRODUCTION\\Scanner\\JOB1" + ".AU2", "C:\\PRODUCTION\\Programs\\master_prog\\" + programName + ".AU2", true);
            }
            catch (Exception ex)
            {

            }

            MessageBox.Show("programs saved succesfully", "FMSS PL", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void comboBoxTModelName_SelectedIndexChanged(object sender, EventArgs e)
        {
            //check robot code
            if (comboBoxTModelName.Text == "")
            {
                return;
            }

            List<string> flags = new List<string>();

            try
            {
                numericUpDownTParam1.Value = Convert.ToDecimal(myRSA.GetDBL().GetModelTable().GetModelNameRecordParam0(myRSA.GetDBL().GetConnection(), comboBoxTModelName.Text));
                numericUpDownTParam2.Value = Convert.ToDecimal(myRSA.GetDBL().GetModelTable().GetModelNameRecordParam1(myRSA.GetDBL().GetConnection(), comboBoxTModelName.Text));
                numericUpDownTParam3.Value = Convert.ToDecimal(myRSA.GetDBL().GetModelTable().GetModelNameRecordParam2(myRSA.GetDBL().GetConnection(), comboBoxTModelName.Text));
                numericUpDownTParam4.Value = Convert.ToDecimal(myRSA.GetDBL().GetModelTable().GetModelNameRecordParam3(myRSA.GetDBL().GetConnection(), comboBoxTModelName.Text));
                numericUpDownTParam5.Value = Convert.ToDecimal(myRSA.GetDBL().GetModelTable().GetModelNameRecordParam5(myRSA.GetDBL().GetConnection(), comboBoxTModelName.Text));
                numericUpDownTParam6.Value = Convert.ToDecimal(myRSA.GetDBL().GetModelTable().GetModelNameRecordParam6(myRSA.GetDBL().GetConnection(), comboBoxTModelName.Text));
                numericUpDownTParam7.Value = Convert.ToDecimal(myRSA.GetDBL().GetModelTable().GetModelNameRecordParam7(myRSA.GetDBL().GetConnection(), comboBoxTModelName.Text));
                numericUpDownTParam8.Value = Convert.ToDecimal(myRSA.GetDBL().GetModelTable().GetModelNameRecordParam8(myRSA.GetDBL().GetConnection(), comboBoxTModelName.Text));
            }
            catch (Exception Ex)
            {

            }

            if (flags.Count == 0)
            {
                //todo message to user
                return;
            }

            //

        }

        private void SendRecipeInAutomatic(string recipeName)
        {

            string par1 = "";
            string par2 = "";
            string par3 = "";
            string par4 = "";
            string par5 = "";
            string par6 = "";
            string par7 = "";
            string par8 = "";
            try
            {
                par1 = myRSA.GetDBL().GetModelTable().GetModelNameRecordParam0(myRSA.GetDBL().GetConnection(), recipeName);
                par2 = myRSA.GetDBL().GetModelTable().GetModelNameRecordParam1(myRSA.GetDBL().GetConnection(), recipeName);
                par3 = myRSA.GetDBL().GetModelTable().GetModelNameRecordParam2(myRSA.GetDBL().GetConnection(), recipeName);
                par4 = myRSA.GetDBL().GetModelTable().GetModelNameRecordParam3(myRSA.GetDBL().GetConnection(), recipeName);
                par5 = myRSA.GetDBL().GetModelTable().GetModelNameRecordParam5(myRSA.GetDBL().GetConnection(), recipeName);
                par6 = myRSA.GetDBL().GetModelTable().GetModelNameRecordParam6(myRSA.GetDBL().GetConnection(), recipeName);
                par7 = myRSA.GetDBL().GetModelTable().GetModelNameRecordParam7(myRSA.GetDBL().GetConnection(), recipeName);
                par8 = myRSA.GetDBL().GetModelTable().GetModelNameRecordParam8(myRSA.GetDBL().GetConnection(), recipeName);
            }
            catch (Exception Ex)
            {

            }

            //send mode to the scanner
            if (tcp == null)
            {
                MessageBox.Show("FMSS PL", "missing connection with scanner", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    if (tcp.IsConnected() == false)
                    {
                        MessageBox.Show("FMSS PL", "missing connection with scanner", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {

                        //send recipe to scanner
                        tcp.Write(
                            "RECIPE,1," + par2 + "," + par3 + "," +
                            par4 + "," + par5 + "," + "3," +
                            par6 + "," + par7 + "," +
                            par8 + "," + par1 + ",");
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void buttonG1A2ManualRead_Click(object sender, EventArgs e)
        {




            ////only in stop auto
            //if (PLCLineOutputDictionary[8] == true)
            //{
            //    MessageBox.Show("manual reading permitted when stop is pressed", "read RFID", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            ////go on read
            //int antenna = 0;
            //if (Properties.Settings.Default.readerType == "BALLUFF") antenna = myRSA.GetG1Balluff().IPort2;
            //else antenna = 1;
            //string tmpG1A1 = "";
            //bool retG1A1 = false;
            //try
            //{
            //    retG1A1 = ReadRequestG1(MachineRuntime.RFIDReaderType, antenna, ref tmpG1A1);
            //}
            //catch (Exception ex)
            //{
            //    retG1A1 = false;
            //}
            //if (retG1A1)
            //{
            //    //rfid code
            //    MachineRuntime.rfidCodeG1A1 = tmpG1A1;
            //    //find rfid association
            //    DBRetCode ret = ManageRequestG1A1(MachineRuntime.rfidCodeG1A1);
            //    if (ret == DBRetCode.RFID_NOT_READ)
            //    {
            //        MachineRuntime.rfidReadingResultG1A1 = -1;
            //        MachineRuntime.rfidReadingResultStringG1A1 = "rfid reading error";
            //    }
            //    else if (ret == DBRetCode.RFID_OLD)
            //    {
            //        MachineRuntime.rfidReadingResultG1A1 = 1;
            //        //update RFID database table
            //        myRSA.GetDBL().GetRFIDTable().UpdateRFIDAssociationInfoMN(myRSA.GetDBL().GetConnection(),
            //            MachineRuntime.rfidCodeG1A1, MachineRuntime.rfidModelNameG1A1, MachineRuntime.rfidSizeG1A1);
            //        MachineRuntime.rfidReadingResultStringG1A1 = "rfid succesfully read and associated";
            //    }
            //    else if (ret == DBRetCode.RFID_NEW)
            //    {
            //        MachineRuntime.rfidReadingResultStringG1A1 = "rfid is NEW, association needed";
            //        if (IsFormOpened())
            //        {

            //        }
            //        else
            //        {
            //            if (frm.ShowDialog() == DialogResult.OK)
            //            {
            //                MachineRuntime.rfidSizeG1A1 = frm.size;
            //                MachineRuntime.rfidFootG1A1 = frm.foot;
            //                myRSA.GetDBL().GetRFIDTable().AddNewRFIDAssociationInfo(myRSA.GetDBL().GetConnection(), MachineRuntime.rfidCodeG1A1,
            //                    MachineRuntime.rfidModelNameG1A1, "000", "XX");
            //                MachineRuntime.rfidReadingResultStringG1A1 = "rfid succesfully read and associated";
            //            }
            //        }
            //        MachineRuntime.rfidReadingResultG1A1 = 0;
            //    }
            //    else if (ret == DBRetCode.RFID_ERROR)
            //    {
            //        MachineRuntime.rfidReadingResultG1A1 = 2;
            //        MachineRuntime.rfidReadingResultStringG1A1 = "rfid reading error";
            //    }
            //}
            //else
            //{
            //    //no tag
            //    MachineRuntime.rfidReadingResultG1A1 = 2;
            //    MachineRuntime.rfidReadingResultStringG1A1 = "rfid not present";
            //}
        }

        private void buttonTRecipeDelete_Click(object sender, EventArgs e)
        {
            DialogResult res = xDialog.MsgBox.Show("are you sure you want to delete " + comboBoxTModelName.Text + " from database?", "FMSS PL", xDialog.MsgBox.Buttons.YesNo);
            if (res == DialogResult.Yes)
            {
                try
                {
                    myRSA.GetDBL().GetModelTable().DeleteModelNameRecordByName(comboBoxTModelName.Text, myRSA.GetDBL().GetConnection());
                    MessageBox.Show("FMSS PL", "model name " + comboBoxTModelName.Text + " succesfully deleted", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("FMSS PL", "error deleting model name " + comboBoxTModelName.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void buttonStartCalib_Click(object sender, EventArgs e)
        {
            //send settings to scanner
            //send mode to the scanner
            if (tcp == null)
            {
                MessageBox.Show("FMSS PL", "missing connection with scanner", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    if (tcp.IsConnected() == false)
                    {
                        MessageBox.Show("FMSS PL", "missing connection with scanner", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        //send training mode to scanner
                        tcp.Write("SETTINGS," + 1 + "," + numericUpDownCExposure.Value.ToString() + ",0,0,0,0,0,0,0,0,");
                        Thread.Sleep(100);
                        tcp.Write("MODE," + 1 + ",1,0,0,0,0,0,0,0,0,");

                        string keyToSend = "pcScannerStartCalib";
                        var sendResult = await ccService.Send(keyToSend, true);
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void numericUpDownSize_T0_ValueChanged(object sender, EventArgs e)
        {
            MachineRuntime.rfidSizeG1A1 = numericUpDownSize_T0.Value.ToString();
        }

        private async void buttonResetScanner_Click(object sender, EventArgs e)
        {
            if (ccService == null) return;
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcScannerAxisReset";
                var sendResult = await ccService.Send(keyToSend, true);

                if (sendResult.OpcResult)
                {
                    Thread.Sleep(100);
                    sendResult = await ccService.Send(keyToSend, false);
                    if (sendResult.OpcResult)
                    {
                        MessageBox.Show("PL", "scanner axis reset done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else MessageBox.Show("PL", "scanner axis error doing reset", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("PL", "scanner axis error doing reset", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else MessageBox.Show("PL", "OPCUA: client not connected", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private async void buttonScannerAxisHome_Click(object sender, EventArgs e)
        {
            if (ccService == null) return;
            if (ccService.ClientIsConnected)
            {
                string keyToSend = "pcScannerAxisHome";
                var sendResult = await ccService.Send(keyToSend, true);

                if (sendResult.OpcResult)
                {
                    Thread.Sleep(100);
                    sendResult = await ccService.Send(keyToSend, false);
                    if (sendResult.OpcResult)
                    {
                        MessageBox.Show("PL", "scanner axis zero done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else MessageBox.Show("PL", "scanner axis error doing zero", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("PL", "scanner axis error doing zero", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else MessageBox.Show("PL", "OPCUA: client not connected", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private async void checkBoxLockUnlock_CheckedChanged(object sender, EventArgs e)
        {
            string keyToRead = "pcLockUnlockPallets";
            var readResult = await ccService.Read(keyToRead);

            if (readResult.OpcResult)
            {
                bool value = bool.Parse(readResult.Value.ToString());
                string keyToSend = "pcLockUnlockPallets";
                if (value)
                {
                    var sendResult = await ccService.Send(keyToSend, false);

                    if (sendResult.OpcResult)
                    {
                        checkBoxLockUnlock.Text = "pallets unlocked";
                    }
                }
                else
                {
                    var sendResult = await ccService.Send(keyToSend, true);

                    if (sendResult.OpcResult)
                    {
                        checkBoxLockUnlock.Text = "pallets locked";
                    }
                }
            }
        }

        private void tabPageT4_2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tabPageT8_1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Set background color (light blue page)
            Brush backgroundBrush = new SolidBrush(Color.FromArgb(226, 233, 243)); // light blue
            g.FillRectangle(backgroundBrush, tabPageT8_1.ClientRectangle);

            Font font = new Font("Verdana", 12);
            Font boldFont = new Font("Verdana", 14, FontStyle.Bold);
            Brush textBrush = Brushes.Black;

            Pen borderPen = new Pen(Color.FromArgb(80, 157, 187), 2)
            {
                DashStyle = System.Drawing.Drawing2D.DashStyle.Dash
            };

            Brush onBrush = new SolidBrush(Color.LightGreen);
            Brush offBrush = new SolidBrush(Color.DarkGreen);

            int blockWidth = 500;
            int blockHeight = 550;
            int startX = 20;
            int startY = 20;
            int rowHeight = 30;

            List<IoSignal> inputSignals = GetInputSignals();
            List<IoSignal> outputSignals = GetOutputSignals();

            // Draw Input block with white background
            Rectangle inputBlock = new Rectangle(startX, startY, blockWidth, blockHeight);
            g.FillRectangle(Brushes.White, inputBlock); // White background for the box
            g.DrawRectangle(borderPen, inputBlock);
            g.DrawString("Input", boldFont, textBrush, startX + 10, startY + 5);

            for (int i = 0; i < inputSignals.Count; i++)
            {
                var sig = inputSignals[i];
                int y = startY + 40 + i * rowHeight;
                g.DrawString(sig.Variable.Replace("_", " "), font, textBrush, startX + 10, y);
                Brush statusBrush = sig.IsOn ? onBrush : offBrush;
                g.FillEllipse(statusBrush, startX + 450, y, 25, 25);
            }

            // Draw Output block to the right of Input block with white background
            int outStartX = startX + blockWidth + 50; // horizontal position after input block + spacing
            Rectangle outputBlock = new Rectangle(outStartX, startY, blockWidth, blockHeight);
            g.FillRectangle(Brushes.White, outputBlock);
            g.DrawRectangle(borderPen, outputBlock);
            g.DrawString("Output", boldFont, textBrush, outStartX + 10, startY + 5);

            for (int i = 0; i < outputSignals.Count; i++)
            {
                var sig = outputSignals[i];
                int y = startY + 40 + i * rowHeight;
                g.DrawString(sig.Variable.Replace("_", " "), font, textBrush, outStartX + 10, y);
                Brush statusBrush = sig.IsOn ? onBrush : offBrush;
                g.FillEllipse(statusBrush, outStartX + 450, y, 25, 25);
            }
        }

        List<IoSignal> GetInputSignals()
        {
            return new List<IoSignal>
            {
                new IoSignal { Variable = "Termico_motore_1", Address = "I0.0", Comment = "Ethernet 0 IN 1", ElectricAddress = "DI0", IsOn = false },
                new IoSignal { Variable = "Termico_motore_2", Address = "I0.1", Comment = "Ethernet 0 IN 2", ElectricAddress = "DI1", IsOn = false },
                new IoSignal { Variable = "Emergenza_OK", Address = "I0.2", Comment = "Ethernet 0 IN 3", ElectricAddress = "DI2", IsOn = false },
                new IoSignal { Variable = "Pressostato", Address = "I0.3", Comment = "Ethernet 0 IN 4", ElectricAddress = "DI3", IsOn = false },
                new IoSignal { Variable = "Manuale_Automatico_pulsantiera_lancio_produzione", Address = "I0.4", Comment = "Ethernet 0 IN 5", ElectricAddress = "DI4", IsOn = false },
                new IoSignal { Variable = "Pulsante_via_pallet_pulsantiera_lancio_produzione", Address = "I0.5", Comment = "Ethernet 0 IN 6", ElectricAddress = "DI5", IsOn = false },
                new IoSignal { Variable = "Start_stop_linea", Address = "I0.6", Comment = "Ethernet 0 IN 7", ElectricAddress = "DI6", IsOn = false },
                new IoSignal { Variable = "Pulsante_emergenza_lancio_produzione_premuto", Address = "I0.7", Comment = "Ethernet 0 IN 8", ElectricAddress = "DI7", IsOn = false },
                new IoSignal { Variable = "Linea_in_spare_9", Address = "I1.0", Comment = "Ethernet 0 IN 9", ElectricAddress = "DI8", IsOn = false },
                new IoSignal { Variable = "Linea_in_spare_10", Address = "I1.1", Comment = "Ethernet 0 IN 10", ElectricAddress = "DI9", IsOn = false },
                new IoSignal { Variable = "Linea_in_spare_11", Address = "I1.2", Comment = "Ethernet 0 IN 11", ElectricAddress = "DI10", IsOn = false },
                new IoSignal { Variable = "Linea_in_spare_12", Address = "I1.3", Comment = "Ethernet 0 IN 12", ElectricAddress = "DI11", IsOn = false },
                new IoSignal { Variable = "Linea_in_spare_13", Address = "I1.4", Comment = "Ethernet 0 IN 13", ElectricAddress = "DI12", IsOn = false },
                new IoSignal { Variable = "Linea_in_spare_14", Address = "I1.5", Comment = "Ethernet 0 IN 14", ElectricAddress = "DI13", IsOn = false },
                new IoSignal { Variable = "Linea_in_spare_15", Address = "I1.6", Comment = "Ethernet 0 IN 15", ElectricAddress = "DI14", IsOn = false },
                new IoSignal { Variable = "Linea_in_spare_16", Address = "I1.7", Comment = "Ethernet 0 IN 16", ElectricAddress = "DI15", IsOn = false }
            };
        }

        List<IoSignal> GetOutputSignals()
        {
            return new List<IoSignal>
            {
                new IoSignal { Variable = "Start_stop_nastri", Address = "Q0.0", Comment = "Ethernet 0 OUT 1", ElectricAddress = "DO0", IsOn = false },
                new IoSignal { Variable = "Linea_out_spare_2", Address = "Q0.1", Comment = "Ethernet 0 OUT 2", ElectricAddress = "DO1", IsOn = false },
                new IoSignal { Variable = "Reset_generale_motore_scanner", Address = "Q0.2", Comment = "Ethernet 0 OUT 3", ElectricAddress = "DO2", IsOn = false },
                new IoSignal { Variable = "Semaforo_sirena", Address = "Q0.3", Comment = "Ethernet 0 OUT 4", ElectricAddress = "DO3", IsOn = false },
                new IoSignal { Variable = "Semaforo_rosso", Address = "Q0.4", Comment = "Ethernet 0 OUT 5", ElectricAddress = "DO4", IsOn = false },
                new IoSignal { Variable = "Semaforo_giallo", Address = "Q0.5", Comment = "Ethernet 0 OUT 6", ElectricAddress = "DO5", IsOn = false },
                new IoSignal { Variable = "Semaforo_verde", Address = "Q0.6", Comment = "Ethernet 0 OUT 7", ElectricAddress = "DO6", IsOn = false },
                new IoSignal { Variable = "Luce_pulsante_start_linea", Address = "Q0.7", Comment = "Ethernet 0 OUT 8", ElectricAddress = "DO7", IsOn = false },
                new IoSignal { Variable = "Lettura_ok_lancio_produzione", Address = "Q1.0", Comment = "Ethernet 0 OUT 9", ElectricAddress = "DO8", IsOn = false },
                new IoSignal { Variable = "Linea_out_spare_10", Address = "Q1.1", Comment = "Ethernet 0 OUT 10", ElectricAddress = "DO9", IsOn = false },
                new IoSignal { Variable = "Trigger_scanner", Address = "Q1.2", Comment = "Ethernet 0 OUT 11", ElectricAddress = "DO10", IsOn = false },
                new IoSignal { Variable = "Linea_out_spare_12", Address = "Q1.3", Comment = "Ethernet 0 OUT 12", ElectricAddress = "DO11", IsOn = false },
                new IoSignal { Variable = "Linea_out_spare_13", Address = "Q1.4", Comment = "Ethernet 0 OUT 13", ElectricAddress = "DO12", IsOn = false },
                new IoSignal { Variable = "Linea_out_spare_14", Address = "Q1.5", Comment = "Ethernet 0 OUT 14", ElectricAddress = "DO13", IsOn = false },
                new IoSignal { Variable = "Linea_out_spare_15", Address = "Q1.6", Comment = "Ethernet 0 OUT 15", ElectricAddress = "DO14", IsOn = false },
                new IoSignal { Variable = "Linea_out_spare_16", Address = "Q1.7", Comment = "Ethernet 0 OUT 16", ElectricAddress = "DO15", IsOn = false }
            };
        }

        public class IoSignal
        {
            public string Variable { get; set; }
            public string Address { get; set; }
            public string Comment { get; set; }
            public string ElectricAddress { get; set; }
            public string RobotAddress { get; set; }
            public bool IsOn { get; set; }
            public string DescriptionENG { get; set; }
        }

        private void tabPageT8_Paint(object sender, PaintEventArgs e)
        {
            //Graphics g = e.Graphics;
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            //// Clear the background with light blue color
            //g.Clear(Color.FromArgb(226, 233, 243));

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Set background color (light blue page)
            Brush backgroundBrush = new SolidBrush(Color.FromArgb(226, 233, 243)); // light blue
            g.FillRectangle(backgroundBrush, tabPageT8_1.ClientRectangle);

            Font font = new Font("Verdana", 10);
            Font boldFont = new Font("Verdana", 12, FontStyle.Bold);
            Brush textBrush = Brushes.Black;

            Pen borderPen = new Pen(Color.FromArgb(80, 157, 187), 2)
            {
                DashStyle = System.Drawing.Drawing2D.DashStyle.Dash
            };

            Brush onBrush = new SolidBrush(Color.LightGreen);
            Brush offBrush = new SolidBrush(Color.DarkGreen);
        }

        private void tabPage8_2_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Background
            Brush backgroundBrush = new SolidBrush(Color.FromArgb(226, 233, 243));
            g.FillRectangle(backgroundBrush, tabPage8_2.ClientRectangle);

            Font font = new Font("Verdana", 12);
            Font boldFont = new Font("Verdana", 14, FontStyle.Bold);
            Brush textBrush = Brushes.Black;

            Pen borderPen = new Pen(Color.FromArgb(80, 157, 187), 2)
            {
                DashStyle = System.Drawing.Drawing2D.DashStyle.Dash
            };

            Brush onBrush = new SolidBrush(Color.LightGreen);
            Brush offBrush = new SolidBrush(Color.DarkGreen);

            int blockWidth = 500;
            int blockHeight = 550;
            int startX = 20;
            int startY = 20;
            int rowHeight = 30;

            List<IoSignal> inputSignals = GetInputSignalsForTab2();
            List<IoSignal> outputSignals = GetOutputSignalsForTab2();

            // INPUT block
            Rectangle inputBlock = new Rectangle(startX, startY, blockWidth, blockHeight);
            g.FillRectangle(Brushes.White, inputBlock);
            g.DrawRectangle(borderPen, inputBlock);
            g.DrawString("Input", boldFont, textBrush, startX + 10, startY + 5);

            for (int i = 0; i < inputSignals.Count; i++)
            {
                var sig = inputSignals[i];
                int y = startY + 40 + i * rowHeight;
                g.DrawString(sig.Variable.Replace("_", " "), font, textBrush, startX + 10, y);
                Brush statusBrush = sig.IsOn ? onBrush : offBrush;
                g.FillEllipse(statusBrush, startX + 450, y, 25, 25);
            }

            // OUTPUT block
            int outStartX = startX + blockWidth + 50;
            Rectangle outputBlock = new Rectangle(outStartX, startY, blockWidth, blockHeight);
            g.FillRectangle(Brushes.White, outputBlock);
            g.DrawRectangle(borderPen, outputBlock);
            g.DrawString("Output", boldFont, textBrush, outStartX + 10, startY + 5);

            for (int i = 0; i < outputSignals.Count; i++)
            {
                var sig = outputSignals[i];
                int y = startY + 40 + i * rowHeight;
                g.DrawString(sig.Variable.Replace("_", " "), font, textBrush, outStartX + 10, y);
                Brush statusBrush = sig.IsOn ? onBrush : offBrush;
                g.FillEllipse(statusBrush, outStartX + 450, y, 25, 25);
            }
        }

        List<IoSignal> GetInputSignalsForTab2()
        {
            return new List<IoSignal>
            {
                new IoSignal { Variable = "Cardatura_cambio_pressione_robot", Address = "I51.0", DescriptionENG="Carding change pressure robot", Comment = "Ethernet 5 IN Espansione 9", RobotAddress = "9", IsOn = false },
                new IoSignal { Variable = "Cardatura_reset_centraggio_SX_robot", Address = "I53.3", DescriptionENG="Carding reset centering left robot", Comment = "Ethernet 5 IN Scambio 28", RobotAddress = "28", IsOn = false },
                new IoSignal { Variable = "Cardatura_reset_centraggio_DX_robot", Address = "I53.4", DescriptionENG="Carding reset  centering right robot", Comment = "Ethernet 5 IN Scambio 29", RobotAddress = "29", IsOn = false },
                new IoSignal { Variable = "Cardatura_salta_copia_terminata_SX_robot", Address = "I53.5", DescriptionENG="Carding jump copy completed left robot", Comment = "Ethernet 5 IN Scambio 30", RobotAddress = "30", IsOn = false },
                new IoSignal { Variable = "Cardatura_salta_copia_terminata_DX_robot", Address = "I53.6", DescriptionENG="Carding jump copy completed right robot", Comment = "Ethernet 5 IN Scambio 31", RobotAddress = "31", IsOn = false },
                new IoSignal { Variable = "Cardatura_pulsante_emergenza_TP_robot", Address = "I53.7", DescriptionENG="Carding emergency button TP robot", Comment = "Ethernet 5 IN Scambio 32", RobotAddress = "32", IsOn = false },
                new IoSignal { Variable = "Cardatura_pulsante_emergenza_controllo_robot", Address = "I54.0", DescriptionENG="Carding emergency button controller robot", Comment = "Ethernet 5 IN Scambio 33", RobotAddress = "33", IsOn = false },
                new IoSignal { Variable = "Cardatura_porte_aperte_robot", Address = "I54.1", DescriptionENG="Carding door open robot", Comment = "Ethernet 5 IN Scambio 34", RobotAddress = "34", IsOn = false }
            };
        }

        List<IoSignal> GetOutputSignalsForTab2()
        {
            return new List<IoSignal>
            {
                new IoSignal { Variable = "Cardatura_salta_copia_SX_robot", Address = "Q54.5", DescriptionENG="Carding jump copy left robot", Comment = "Ethernet 5 OUT Scambio 38", RobotAddress = "1038", IsOn = false },
                new IoSignal { Variable = "Cardatura_salta_copia_DX_robot", Address = "Q54.6", DescriptionENG="Carding jump copy right robot", Comment = "Ethernet 5 OUT Scambio 39", RobotAddress = "1039", IsOn = false },
            };
        }

        private void tabPage8_3_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Background
            Brush backgroundBrush = new SolidBrush(Color.FromArgb(226, 233, 243));
            g.FillRectangle(backgroundBrush, tabPage8_2.ClientRectangle);

            Font font = new Font("Verdana", 12);
            Font boldFont = new Font("Verdana", 14, FontStyle.Bold);
            Brush textBrush = Brushes.Black;

            Pen borderPen = new Pen(Color.FromArgb(80, 157, 187), 2)
            {
                DashStyle = System.Drawing.Drawing2D.DashStyle.Dash
            };

            Brush onBrush = new SolidBrush(Color.LightGreen);
            Brush offBrush = new SolidBrush(Color.DarkGreen);

            int blockWidth = 500;
            int blockHeight = 550;
            int startX = 20;
            int startY = 20;
            int rowHeight = 30;

            List<IoSignal> inputSignals = GetInputSignalsForTab3();
            List<IoSignal> outputSignals = GetOutputSignalsForTab3();

            // INPUT block
            Rectangle inputBlock = new Rectangle(startX, startY, blockWidth, blockHeight);
            g.FillRectangle(Brushes.White, inputBlock);
            g.DrawRectangle(borderPen, inputBlock);
            g.DrawString("Input", boldFont, textBrush, startX + 10, startY + 5);

            for (int i = 0; i < inputSignals.Count; i++)
            {
                var sig = inputSignals[i];
                int y = startY + 40 + i * rowHeight;
                g.DrawString(sig.Variable.Replace("_", " "), font, textBrush, startX + 10, y);
                Brush statusBrush = sig.IsOn ? onBrush : offBrush;
                g.FillEllipse(statusBrush, startX + 450, y, 25, 25);
            }

            // OUTPUT block
            int outStartX = startX + blockWidth + 50;
            Rectangle outputBlock = new Rectangle(outStartX, startY, blockWidth, blockHeight);
            g.FillRectangle(Brushes.White, outputBlock);
            g.DrawRectangle(borderPen, outputBlock);
            g.DrawString("Output", boldFont, textBrush, outStartX + 10, startY + 5);

            for (int i = 0; i < outputSignals.Count; i++)
            {
                var sig = outputSignals[i];
                int y = startY + 40 + i * rowHeight;
                g.DrawString(sig.Variable.Replace("_", " "), font, textBrush, outStartX + 10, y);
                Brush statusBrush = sig.IsOn ? onBrush : offBrush;
                g.FillEllipse(statusBrush, outStartX + 450, y, 25, 25);
            }
        }

        List<IoSignal> GetInputSignalsForTab3()
        {
            return new List<IoSignal>
            {
                new IoSignal { Variable = "Cardatura_pulsante_emergenza_premuto", Address = "I90.0", ElectricAddress = "DI0" },
                new IoSignal { Variable = "Cardatura_in_spare_2", Address = "I90.1", ElectricAddress = "DI1" },
                new IoSignal { Variable = "Cardatura_pressostato", Address = "I90.2", ElectricAddress = "DI2" },
                new IoSignal { Variable = "Cardatura_drive_pronto", Address = "I90.3", ElectricAddress = "DI3" },
                new IoSignal { Variable = "Cardatura_velocita_zero", Address = "I90.4", ElectricAddress = "DI4" },
                new IoSignal { Variable = "Cardatura_allarme_drive", Address = "I90.5", ElectricAddress = "DI5" },
                new IoSignal { Variable = "Cardatura_in_spare_7", Address = "I90.6", ElectricAddress = "DI6" },
                new IoSignal { Variable = "Cardatura_in_spare_8", Address = "I90.7", ElectricAddress = "DI7" },
                new IoSignal { Variable = "Cardatura_in_spare_9", Address = "I91.0", ElectricAddress = "DI8" },
                new IoSignal { Variable = "Cardatura_in_spare_10", Address = "I91.1", ElectricAddress = "DI9" },
                new IoSignal { Variable = "Cardatura_in_spare_11", Address = "I91.2", ElectricAddress = "DI10" },
                new IoSignal { Variable = "Cardatura_in_spare_12", Address = "I91.3", ElectricAddress = "DI11" },
                new IoSignal { Variable = "Cardatura_in_spare_13", Address = "I91.4", ElectricAddress = "DI12" },
                new IoSignal { Variable = "Cardatura_in_spare_14", Address = "I91.5", ElectricAddress = "DI13" },
                new IoSignal { Variable = "Cardatura_in_spare_15", Address = "I91.6", ElectricAddress = "DI14" },
                new IoSignal { Variable = "Cardatura_in_spare_16", Address = "I91.7", ElectricAddress = "DI15" },

            };
        }

        List<IoSignal> GetOutputSignalsForTab3()
        {
            return new List<IoSignal>
            {
                new IoSignal { Variable = "Cardatura_servo_on", Address = "Q90.0", ElectricAddress = "DO0" },
                new IoSignal { Variable = "Cardatura_start_velocita", Address = "Q90.1", ElectricAddress = "DO1" },
                new IoSignal { Variable = "Cardatura_velocita_bit_0", Address = "Q90.2", ElectricAddress = "DO2" },
                new IoSignal { Variable = "Cardatura_velocita_bit_1", Address = "Q90.3", ElectricAddress = "DO3" },
                new IoSignal { Variable = "Cardatura_reset_allarmi_drive", Address = "Q90.4", ElectricAddress = "DO4" },
                new IoSignal { Variable = "Cardatura_stop_motore", Address = "Q90.5", ElectricAddress = "DO5" },
                new IoSignal { Variable = "Cardatura_compensazione_indietro_1", Address = "Q90.6", ElectricAddress = "DO6" },
                new IoSignal { Variable = "Cardatura_compensazione_indietro_2", Address = "Q90.7", ElectricAddress = "DO7" },
                new IoSignal { Variable = "Cardatura_cambio_pressione", Address = "Q91.0", ElectricAddress = "DO8" },
                new IoSignal { Variable = "Cardatura_reset_drive", Address = "Q91.1", ElectricAddress = "DO9" },
                new IoSignal { Variable = "Cardatura_velocita_bit_2", Address = "Q91.2", ElectricAddress = "DO10" },
                new IoSignal { Variable = "Cardatura_out_spare_12", Address = "Q91.3", ElectricAddress = "DO11" },
                new IoSignal { Variable = "Cardatura_out_spare_13", Address = "Q91.4", ElectricAddress = "DO12" },
                new IoSignal { Variable = "Cardatura_spia_verde", Address = "Q91.5", ElectricAddress = "DO13" },
                new IoSignal { Variable = "Cardatura_spia_gialla", Address = "Q91.6", ElectricAddress = "DO14" },
                new IoSignal { Variable = "Cardatura_spia_rossa", Address = "Q91.7", ElectricAddress = "DO15" },

            };
        }
    }
}
