using KRcc;
using RSACommon;
using RSACommon.Event;
using RSAFile;
using RSAFile.Json;
using RSAInterface;
using RSAInterface.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IdentityModel.Protocols.WSTrust;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace Robot
{
    public enum KawasakiFlag
    {
        ON = -1,
        OFF = 0
    }

    public class KawasakiConfiguration : RobotConfiguration
    {
        public int HigPALLETPRIORITYMemoryTimerMS { get; set; } = Kawasaki.HIGH_PRIORITY_TIMER_MS;
        public int MidPriorityMemoryTimerMS { get; set; } = Kawasaki.MID_PRIORITY_TIMER_MS;
        public int LowPriorityMemoryTimerMS { get; set; } = Kawasaki.LOW_PRIORITY_TIMER_MS;
        public int TI { get; set; } = Kawasaki.LOW_PRIORITY_TIMER_MS;
    }


    public class Kawasaki: BaseRobot<KawasakiMemoryVariable>
    {
        public static  readonly int MEMORY_TIME_ELAPSED_TIME_MS = 250; //ms
        public static readonly int HIGH_PRIORITY_TIMER_MS = 1000; //ms
        public static readonly int MID_PRIORITY_TIMER_MS = 5000; //ms
        public static readonly int LOW_PRIORITY_TIMER_MS = 10000; //ms
        Timer CheckConnectionTimer { get; set; } = null;

        public Commu krccCommmunication;
        string communicationStringDefault = "TCP as@{0} {1}";
        public override bool IsConnected
        {
            get
            {
                try
                {
                    if (krccCommmunication == null)
                    {
                        return false;
                    }

                    else return krccCommmunication.IsConnected;
                }
                catch
                {
                    
                    return false;
                }
            }
        }

        public Kawasaki(string host, int port): base()
        {
            _host = host;
            _port = port;
            ServiceURI = Helper.BuildUri("http", host, port);

            memorySaverConfig = new JsonConfiguration(MemoryConfiguratorFilename, MemoryConfiguratorPath);

            //CreateNewVirtualMemory();
        }

        public Kawasaki(RobotConfiguration configuration, IRobotMemory<BaseRobotVariable> virtualizedMemory)
        {
            if(virtualizedMemory is KawasakiMemory mem)
            {
                VirtualizedMemory = mem;
            }

            _host = configuration.Host;
            _port = configuration.Port;
            ServiceURI = Helper.BuildUri("http", _host, _port);

            Name = configuration.ServiceName;
            IsActive = configuration.Active;

            memorySaverConfig = new JsonConfiguration(MemoryConfiguratorFilename, MemoryConfiguratorPath);

        }

        private async void CheckConnectionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (IsConnected)
                return;

            if(RobotConnectionStatus.Connected != await ConnectAsync())
            {
                Deactivating();
                RSACustomEvents.OnServiceDisconnected(new RSACustomEvents.ServiceConnectionEventArgs(this, "FutureServer"));
            }
        }

        public Kawasaki(RobotConfiguration configuration) : this(configuration.Host, configuration.Port)
        {
            Name = configuration.ServiceName;
            IsActive = configuration.Active;

            if(configuration is KawasakiConfiguration konfig)
                ConfigTimer(konfig);

            RSACustomEvents.KawasakiCommandEvent += RSACustomEvents_KawasakiCommandEvent;
            RSACustomEvents.KawasakiCheckVariableIntWithTimeoutEvent += RSACustomEvents_KawasakiCheckVariableIntWithTimeoutEvent;
            RSACustomEvents.KawasakiCommandEventDelayed += RSACustomEvents_KawasakiCommandEventDelayed;

            CheckConnectionTimer = new Timer(TimeSpan.FromSeconds(5).TotalMilliseconds);
            CheckConnectionTimer.Elapsed += CheckConnectionTimer_Elapsed;
        }

        private void RSACustomEvents_KawasakiCommandEventDelayed(object sender, RSACustomEvents.KawasakiCommandDelayedEventArgs e)
        {
            string result = "";

            if (e == null)
                return;

            Timer newTimer = new Timer(e.Milliseconds);
            newTimer.AutoReset = e.AutoReset;

            newTimer.Start();
            newTimer.Elapsed += (object timerSender, ElapsedEventArgs elapsedEvent) =>
            {
                if (SetCommand(e.Command, ref result))
                {
                    Log?.Info($"{e.Service.Name} call: delayed set command {e.Command}");
                }
                else
                    Log?.Info($"{e.Service.Name} call: Error on delayed set command {e.Command}");
            };
        }

        private async void RSACustomEvents_KawasakiCheckVariableIntWithTimeoutEvent(object sender, RSACustomEvents.KawasakiCheckVariableWithTimeoutEventArgs<int> e)
        {

            Stopwatch sw = new Stopwatch();
            sw.Start();

            Dictionary<string, bool> ValueToReach = new Dictionary<string, bool>();

            string test = string.Empty;
            foreach (string cmd in e.Commands.Keys)
            {
                ValueToReach[cmd] = false;
                test += cmd +"   ";
            }

            Log?.Info($"VALUE TO SET: {test}");
            try
            {
                while (true)
                {
                    if (sw.ElapsedMilliseconds > e.TimeOut)
                        throw new TimeoutException();

                    //MessageResponse resultValue = MessageResponse.NoPrompt;

                    foreach (string command in e.Commands.Keys.ToList())
                    {
                        int readValue = await ReadCommandAsync<int>(command);
                        if(readValue == e.Commands[command])
                        {

                            Log?.Info($"{command} correct value, sent the success");
                            ValueToReach[command] = true;
                            e.Commands.Remove(command);
                        }
                    }

                    bool status = true;
                    foreach (string cmd in ValueToReach.Keys)
                    {
                        status &= ValueToReach[cmd];
                    }
                    
                    if (status)
                    {
                        RSACustomEvents.OnKawasakiTimeout(new RSACustomEvents.KawasakiTimeoutResultEventArgs(this, ValueToReach, MessageResponse.Success));
                        break;
                    }

                    await Task.Delay(1000);
                }
            }
            catch(TimeoutException)
            {
                RSACustomEvents.OnKawasakiTimeout(new RSACustomEvents.KawasakiTimeoutResultEventArgs(this, ValueToReach, MessageResponse.Timeout));
                Log?.Info($"Timeout for: {e.Commands.Keys.ToList()}, sent the timeout value");
            }
        }

        private void RSACustomEvents_KawasakiCommandEvent(object sender, RSACustomEvents.KawasakiCommandEventArgs e)
        {
            string result = "";
            if(e != null)
            {
                if (SetCommand(e.Command, ref result))
                {
                    Log?.Info($"{e.Service.Name} call: set command {e.Command}");
                }
                else
                    Log?.Info($"{e.Service.Name} call: Error on delayed set command {e.Command}");
            }
        }

        public bool LoadProgram()
        {

            return true;
        }


        #region MEMORY TIMER KAWASAI

        public System.Timers.Timer HigPALLETPRIORITYMemoryTimer { get; set; }
        public System.Timers.Timer MidPriorityMemoryTimer { get; set; }
        public System.Timers.Timer LowPriorityMemoryTimer { get; set; }
        public int HigPALLETPRIORITYMemoryTimerMS { get; set; } = HIGH_PRIORITY_TIMER_MS;
        public int MidPriorityMemoryTimerMS { get; set; } = MID_PRIORITY_TIMER_MS;
        public int LowPriorityMemoryTimerMS { get; set; } = LOW_PRIORITY_TIMER_MS;

        public List<System.Timers.Timer> ListOfTimer = new List<System.Timers.Timer>();

        public void ConfigTimer(KawasakiConfiguration configuration)
        {
            HigPALLETPRIORITYMemoryTimerMS = configuration.HigPALLETPRIORITYMemoryTimerMS;
            MidPriorityMemoryTimerMS = configuration.LowPriorityMemoryTimerMS;
            LowPriorityMemoryTimerMS = configuration.MidPriorityMemoryTimerMS;

            HigPALLETPRIORITYMemoryTimer = new System.Timers.Timer(HigPALLETPRIORITYMemoryTimerMS);
            MidPriorityMemoryTimer = new System.Timers.Timer(MidPriorityMemoryTimerMS);
            LowPriorityMemoryTimer = new System.Timers.Timer(LowPriorityMemoryTimerMS);

            HigPALLETPRIORITYMemoryTimer.Elapsed += HigPALLETPRIORITYMemoryTimer_Elapsed;
            MidPriorityMemoryTimer.Elapsed += MidPriorityMemoryTimer_Elapsed;
            LowPriorityMemoryTimer.Elapsed += LowPriorityMemoryTimer_Elapsed;

            ListOfTimer = new List<System.Timers.Timer>() { HigPALLETPRIORITYMemoryTimer, MidPriorityMemoryTimer, LowPriorityMemoryTimer };
        }

        public virtual async void HigPALLETPRIORITYMemoryTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            long loadingTime = await VirtualizedMemory.LoadMemory(RefreshPriority.High);
            //throw new NotImplementedException();
        }

        private async void MidPriorityMemoryTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            long loadingTime = await VirtualizedMemory.LoadMemory(RefreshPriority.Mid);
        }

        private async void LowPriorityMemoryTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            long loadingTime = await VirtualizedMemory.LoadMemory(RefreshPriority.Low);
        }

        #endregion

        #region --- MEMORY LOADING ---
        /// <summary>
        /// This function instantiate another type of memory for the robot.
        /// </summary>
        public override void CreateNewVirtualMemory()
        {
            VirtualizedMemory = new KawasakiMemory(this);
        }

        #endregion

        protected override RobotConnectionStatus Connect()
        {
            _commString = BuildCommunicationString(_host, _port);

            Log?.Info($"Version {krccCommmunication}, Communication string {_commString}");

            Stopwatch sw = Stopwatch.StartNew();

            try
            {
                krccCommmunication = new Commu(_commString);
                krccCommmunication.progress = KawasakiProgressEventHandler;
                Log?.Warn($"Time elapsed in connecting Kawasaki {sw.ElapsedMilliseconds} ms");
            }
            catch
            {
                Log?.Warn($"Time elapsed in connecting Kawasaki {sw.ElapsedMilliseconds} ms");
                Log?.Info("Kawasaki Connection error");
                Deactivating();
                return RobotConnectionStatus.ConnectionError;
            }

            sw.Stop();

            //Log?.Warn($"Time elapsed in connecting Kawasaki {sw.ElapsedMilliseconds} ms");

            if (IsConnected)
            {
                RSACustomEvents.OnServiceConnection(new RSACustomEvents.ServiceConnectionEventArgs(this, "FutureServer"));
                Log?.Info("Kawasaki Connection is fine");
                Name = krccCommmunication.name();
                ResetReactivatingTry();
            }
            else
                RSACustomEvents.OnServiceDisconnected(new RSACustomEvents.ServiceConnectionEventArgs(this, "FutureServer"));

            return krccCommmunication.IsConnected ? RobotConnectionStatus.Connected : RobotConnectionStatus.ConnectionError;
        }

        protected async Task<RobotConnectionStatus> ConnectAsync()
        {
            _commString = BuildCommunicationString(_host, _port);
            //_commString = "TCP as@172.31.10.100";
            Log?.Info($"Version {krccCommmunication}, Communication string {_commString}");

            Stopwatch sw = Stopwatch.StartNew();

            krccCommmunication = await Task<Commu>.Run(async () =>
            {

                try
                {
                    return await Task.FromResult<Commu>(new Commu(_commString));
                }
                catch
                {
                    return null;
                }
            });

            if(krccCommmunication is null)
            {
                Log?.Warn($"Time elapsed in connecting Kawasaki {sw.ElapsedMilliseconds} ms");
                Log?.Info("Kawasaki Connection error");
                //Deactivating();
                return RobotConnectionStatus.ConnectionError;

            }
            else
            {
                krccCommmunication.progress = KawasakiProgressEventHandler;
                Log?.Warn($"Time elapsed in connecting Kawasaki {sw.ElapsedMilliseconds} ms");

                if (IsConnected)
                {
                    RSACustomEvents.OnServiceConnection(new RSACustomEvents.ServiceConnectionEventArgs(this, "FutureServer"));
                    Log?.Info("Kawasaki Connection is fine");
                    Name = krccCommmunication.name();
                    ResetReactivatingTry();
                }
                else
                    RSACustomEvents.OnServiceDisconnected(new RSACustomEvents.ServiceConnectionEventArgs(this, "FutureServer"));
            }

            sw.Stop();

            //Log?.Warn($"Time elapsed in connecting Kawasaki {sw.ElapsedMilliseconds} ms");

            return krccCommmunication.IsConnected ? RobotConnectionStatus.Connected : RobotConnectionStatus.ConnectionError;
        }

        private bool Disconnect()
        {
            if (krccCommmunication == null || !krccCommmunication.IsConnected)
                return true;

            if(krccCommmunication.disconnect())
            {
                krccCommmunication.Dispose();
                return true;
            }

            return false;
        }

        public override IRobotMemory<IRobotVariable> LoadMemoryConfiguration(string filename)
        {
            if (!File.Exists(filename))
            {
                CreateNewVirtualMemory();

                memorySaverConfig.Save(filename, VirtualizedMemory);
            }

            return memorySaverConfig.Load<KawasakiMemory>(filename);
        }

        //public virtual IRobotMemory<V> LoadMemoryConfiguration()
        //{
        //    string filename = Path.Combine(MemoryConfiguratorPath, MemoryConfiguratorFilename);
        //    return LoadMemoryConfiguration(filename);
        //}


        public override IRobotMemory<IRobotVariable> LoadMemoryConfiguration()
        {
            string filename = Path.Combine(MemoryConfiguratorPath, MemoryConfiguratorFilename);

            VirtualizedMemory = LoadMemoryConfiguration(filename).FixVirtualizedMemory();
            VirtualizedMemory.SetRobot(this);

            return VirtualizedMemory;
        }


        public override bool SaveMemoryConfiguration(string path)
        {
            if(VirtualizedMemory is IRobotMemory<KawasakiMemoryVariable> toSave)
                memorySaverConfig.Save(path, toSave);

            return true;
        }

        public bool GetControllerMessage(ref ArrayList ret)
        {
            if (!IsConnected)
                return false;

            ret = krccCommmunication.command();

            return true;
        }

        public string ParseString(string objToParse)
        {
            return objToParse.TrimEnd(new char[] { '\r', '\n' });
        }

        /// <summary>
        /// Alternative connection with custom host and custom port
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public RobotConnectionStatus Connect(string host, int port)
        {
            return (RobotConnectionStatus)krccCommmunication.connect(BuildCommunicationString(host, port));
        }

        public string BuildCommunicationString(string host, int port)
        {
            return string.Format(communicationStringDefault, host, port);
        }

        public void KawasakiProgressEventHandler(int value, int total)
        {
            Console.WriteLine($"{value}/{total}");
        }

        public override  async Task<bool> SetVariableAsync<T>(string key, T value)
        {
            return await Task.Run(() =>
            {

                return SetVariable<T>(key, value);

            });
        }

        public virtual MessageResponse Load(string path)
        {
            if (!IsConnected)
                return MessageResponse.Bad;

            if (!File.Exists(path))
            {
                return MessageResponse.Bad;
            }

            return (MessageResponse)krccCommmunication.load(path);      

        }

        public override bool SetVariable<T>(string VariableName, T value)
        {
            if (!IsConnected)
                return false;

            //Da gestire le stringhe
            string command;
            if (typeof(T) == typeof(string))
            {
                command = $"{VariableName} = \"{value}\"";
            }
            else
                command = $"{VariableName} = {value}";

            ArrayList returnedList = krccCommmunication.command(command);

            if ((MessageResponse)returnedList[0] == MessageResponse.Success)
            {
                return true;
            }

            return false;

        }

        public async override Task<T> ReadCommandAsync<T>(string command)
        {
            if(!IsConnected)
                return default(T);

            string[] charsToRemove = new string[] { "@", ",", ".", ";", "'" };

            return await Task.Run(() =>            
            {

                ArrayList returnedList = krccCommmunication.command(command);

                string replaced = Regex.Replace(((string)returnedList[1]), "[\r\n\\\"]", string.Empty);

                try
                {
                    if ((MessageResponse)returnedList[0] == MessageResponse.Success && !(returnedList[1] as string).Contains("E0102"))
                    {
                        return Task.FromResult((T)Convert.ChangeType(replaced, typeof(T)));
                    }
                    else if ((MessageResponse)returnedList[0] == MessageResponse.Timeout)
                    {
                        Log?.Warn($"{command} Timeout");
                        return Task.FromResult(default(T));
                    }
                    else if ((MessageResponse)returnedList[0] == MessageResponse.NoPrompt)
                    {
                        Log?.Warn($"{command} No prompt");
                        return Task.FromResult(default(T));
                    }
                    else
                    {
                        Log?.Warn($"{command} No info from kawasaki");
                        return Task.FromResult(default(T));
                    }
                }
                catch
                {
                    Log?.Warn($"{command} No variable setted from kawasaki");
                    return Task.FromResult(default(T));
                }

            });
        }

        public bool RobotIsConnected()
        {
            try
            {
                if (krccCommmunication == null || !krccCommmunication.IsConnected)
                {
                    Log?.Warn("Robot communication offline, trying the reconnection...");
                    RSACustomEvents.OnServiceDisconnected(new RSACustomEvents.ServiceConnectionEventArgs(this, "Future"));

                    if (Connect() != RobotConnectionStatus.Connected)
                        return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }


        private readonly object readlock = new object();
        /// <summary>
        /// return variable and the reading status, monitor command is the variable to read
        /// </summary>
        /// <param name="monitorCommand"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        public override bool ReadCommand<T>(string monitorCommand, ref T ret)
        {
            if (!IsConnected)
                return false;

            string[] charsToRemove = new string[] { "@", ",", ".", ";", "'" };             

            lock(readlock)
            {
                try
                {
                    ArrayList returnedList = krccCommmunication.command(monitorCommand);

                    string replaced = Regex.Replace(((string)returnedList[1]), "[\r\n\\\"]", string.Empty);

                    if ((MessageResponse)returnedList[0] == MessageResponse.Success)
                    {
                        ret = (T)Convert.ChangeType(replaced, typeof(T));
                        return true;
                    }
                }
                catch
                {
                    ret = default(T);
                    return false;
                }
            }

            return false;
        }

        public enum ProcessResult
        {
            /// <summary>
            /// Operazione conclusa correttamente.
            /// </summary>
            Ok,
            /// <summary>
            /// Operazione conclusa con errore.
            /// </summary>
            Error,
            /// <summary>
            /// Operazione non conclusa ma in attesa di conferma.
            /// </summary>
            WaitAnswer,
        }

        public ProcessResult ReadDoubleVariable(string variabile, out double valore)
        {
            int ret = -1;
            valore = 0;

            if (variabile == "" || variabile == null || variabile.StartsWith("$") || !IsConnected)
            {
                ret = -20;
                return ProcessResult.Error;
            }

            try
            {
                krccCommmunication.cmdInquiry = null;

                // return value of command() is ArrayList.
                //
                // resp(0) :  result code    0:  OK with response string
                //                          -1:  timeout
                //                          -2:  No prompt
                // resp(1) :  response string      

                ret = 99;
                string cmdAs = "List/R " + variabile;
                ArrayList returnedList = krccCommmunication.command(cmdAs);

                ret = 0;

                if (returnedList == null)
                    return ProcessResult.Error;
                if (returnedList.Count != 2)
                    return ProcessResult.Error;
                if (!(returnedList[0] is int) || (int)returnedList[0] != 0)
                    return ProcessResult.Error;

                if (returnedList[1] is string risp && risp != "")
                {
                    if (risp.Contains("="))
                    {
                        risp = risp.Remove(0, risp.IndexOf("=") + 2);
                        valore = Convert.ToDouble(risp);
                        return ProcessResult.Ok;
                    }
                    else
                    {
                        return ProcessResult.Error;
                    }
                }
                return ProcessResult.Error;
            }
            catch (Exception ex)
            {
                ret = 17;
                //Messaggi.Errore("Errore DLL KRCC" + " Robot " + id_robot.ToString() + " " + indirizzoRobotTcp, ex.Message);
                return ProcessResult.Error;
            }
        }



        public ProcessResult ReadSignalSwitch(string variabile, out bool valore)
        {
            valore = false;
            int ret = -1;

            if (variabile == "" || variabile == null || !IsConnected)
            {
                ret = -20;
                return ProcessResult.Error;
            }

            try
            {
                krccCommmunication.cmdInquiry = null;

                // return value of command() is ArrayList.
                //
                // resp(0) :  result code    0:  OK with response string
                //                          -1:  timeout
                //                          -2:  No prompt
                // resp(1) :  response string      

                ret = 99;
                string cmdAs = "type (" + variabile + ")"; // “type sig(“ per leggere un ingresso!!!

                ArrayList returnedList = krccCommmunication.command(cmdAs);
                ret = 0;

                if (returnedList == null)
                    return ProcessResult.Error;
                if (returnedList.Count != 2)
                    return ProcessResult.Error;
                if (!(returnedList[0] is int) || (int)returnedList[0] != 0)
                    return ProcessResult.Error;

                if (returnedList[1] is string risp && risp != "")
                {
                    if (int.TryParse(risp, out int valTemp))
                    {
                        if (valTemp == -1)
                        {
                            valore = true;
                            return ProcessResult.Ok;
                        }
                        if (valTemp == 0)
                        {
                            valore = false;
                            return ProcessResult.Ok;
                        }
                    }
                    return ProcessResult.Error;
                }
                return ProcessResult.Error;
            }
            catch (Exception ex)
            {
                ret = 17;
                //Messaggi.Errore("Errore DLL KRCC", ex.Message);
                return ProcessResult.Error;
            }
        }


        public ProcessResult ReadStringVariable(string variabile, out string valore)
        {
            valore = "";
            int ret = -1;
            if (variabile == "" || variabile == null || !variabile.StartsWith("$") || !IsConnected)
            {
                ret = -20;
                return ProcessResult.Error;
            }

            try
            {
                krccCommmunication.cmdInquiry = null;

                // return value of command() is ArrayList.
                //
                // resp(0) :  result code    0:  OK with response string
                //                          -1:  timeout
                //                          -2:  No prompt
                // resp(1) :  response string      

                ret = 99;
                string cmdAs = "List/S " + variabile;

                ArrayList returnedList = krccCommmunication.command(cmdAs);
                ret = 0;

                if (returnedList == null)
                    return ProcessResult.Error;
                if (returnedList.Count != 2)
                    return ProcessResult.Error;
                if (!(returnedList[0] is int) || (int)returnedList[0] != 0)
                    return ProcessResult.Error;

                if (returnedList[1] is string risp && risp != "")
                {
                    if (risp.Contains("="))
                    {
                        risp = risp.Remove(0, risp.IndexOf("=") + 2);
                        int start = risp.IndexOf('"');
                        int end = risp.IndexOf('"', start + 1);
                        if (start >= 0 && end > start && (end - start - 1) >= 0)
                        {
                            valore = risp.Substring(start + 1, end - start - 1);
                            return ProcessResult.Ok;
                        }
                    }
                }
                return ProcessResult.Error;
            }
            catch (Exception ex)
            {
                ret = 17;
                //Messaggi.Errore("Errore DLL KRCC" + " Robot " + id_robot.ToString() + " " + indirizzoRobotTcp, ex.Message);
                return ProcessResult.Error;
            }
        }




        public async override Task<IService> Start()
        {
            Log?.Info($"Starting service {Name}");

            try
            {
                await base.Start();
               
                //ListOfTimer.ForEach(x => x.Start());

                if(RobotConnectionStatus.Connected == await ConnectAsync())
                {
                    IsActive = true;
                }

                CheckConnectionTimer.Start();
            }
            catch (Exception e)
            {
                Log?.Warn($"Error on starting service {Name} {e.Message} {e.Source}");
                IsActive = false;
            }
  

            return this;
        }

        public override void Stop()
        {
            base.Stop();
            ListOfTimer.ForEach(x => x.Stop());

            Disconnect();

            CheckConnectionTimer.Stop();

        }

        public override bool SetCommand(string monitorCommand, ref string ret)
        {
            if (!IsConnected)
                return false;

            ArrayList returnedList = krccCommmunication.command(monitorCommand);

            if ((MessageResponse)returnedList[0] == MessageResponse.Success)
            {
                ret = Regex.Replace(((string)returnedList[1]), "[\r\n\\\"]", string.Empty);

                return true;
            }

            return false;
        }
    }
}
