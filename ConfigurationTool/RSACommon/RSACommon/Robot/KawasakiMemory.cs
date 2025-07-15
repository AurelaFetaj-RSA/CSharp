using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using log4net;
using Newtonsoft.Json;
using RSACommon;


namespace Robot
{
    public class KawasakiMemoryVariable: BaseRobotVariable
    {
        public KawasakiCommand CommandType { get; set; }

        public string CommandString { get => MakeCommand(CommandType, Name); }

        public static string MakeCommand(Enum command, string variable)
        {
            return $"{command} {variable}";
        }
    }

    public enum KawasakiCommand
    {
        ty,
        po,
    }

    public class KawasakiMemory : IRobotMemory<KawasakiMemoryVariable>
    {
        public IDictionary<string, IRobotVariable> VirtualMemory = new Dictionary<string, IRobotVariable>();

        [JsonIgnore]
        IRobot<IRobotVariable> _robot { get; set; } = null;
        [JsonIgnore]
        public IList<IRobotVariable> Memory => VirtualMemory.Values.ToList();

        [JsonIgnore]
        ILog Log { get; set; } = null;

        [JsonConstructor]
        public KawasakiMemory(Kawasaki robot) 
        {
            _robot = robot;

            SetRobot(robot);
        }

        public void SetRobot(IRobot<IRobotVariable> robot)
        {
            _robot = robot;

            if (_robot != null)
            {
                //_robot.MemoryTimer.Elapsed += OnUpdateMemory;
                Log = _robot.Log;
            }

        }

        /// <summary>
        /// Update memory for the Kawasaki
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected async void OnUpdateMemory(object o, ElapsedEventArgs e)
        {
            /*
            if(!_robot.IsConnected)
            {
                Log?.Warn($"{_robot.CommunicationString} is not connected");
                return;
            }

            foreach (var memory in VirtualMemory)
            {
                //Very heavy
                var toFill = Activator.CreateInstance(memory.Value.Type);

                if (memory.Value is KawasakiMemoryVariable kwM)
                {
                    _robot.ReadVariable(kwM.CommandString, ref toFill);
                    kwM.Value = toFill;
                }
            }
            */

            long loadingTime = await LoadMemory();
        }

        public async Task<long> LoadMemory(RefreshPriority priority)
        {
            Stopwatch t = new Stopwatch();

            if (!_robot.IsConnected)
            {
                Log?.Warn($"{_robot.CommunicationString} is not connected");
                return long.MaxValue;
            }

            await Task.Run(() =>
            {
                t.Start();
                foreach (var memory in VirtualMemory)
                {
                    if (priority != memory.Value.Priority)
                        continue;
                    //Very heavy
                    var toFill = Activator.CreateInstance(memory.Value.Type);

                    if (memory.Value is KawasakiMemoryVariable kwM)
                    {
                        _robot.ReadCommand(kwM.CommandString, ref toFill);
                        kwM.Value = toFill;
                        kwM.LastTimeRefreshed = DateTime.Now;
                    }
                }
                t.Stop();
                return Task.FromResult(t.ElapsedMilliseconds);
            });

            return t.ElapsedMilliseconds;
        }

        public async Task<long> LoadMemory()
        {
            Stopwatch t = new Stopwatch();

            if (!_robot.IsConnected)
            {
                Log?.Warn($"{_robot.CommunicationString} is not connected");
                return long.MaxValue;
            }

            await Task.Run(() =>
            {
                t.Start();
                foreach (var memory in VirtualMemory)
                {
                    //Very heavy
                    var toFill = Activator.CreateInstance(memory.Value.Type);

                    if (memory.Value is KawasakiMemoryVariable kwM)
                    {
                        _robot.ReadCommand(kwM.CommandString, ref toFill);
                        kwM.Value = toFill;
                    }
                }
                t.Stop();
                return Task.FromResult(t.ElapsedMilliseconds);
            });

            return t.ElapsedMilliseconds;
        }

        /*
        private Task LoadMemoryFromRobot()
        {
            return Task.Factory.StartNew(() =>
            {
               
            });
        }

        public Task ExecuteAsync()
        {
            foreach (var memory in VirtualMemory)
            {
                //Very heavy
                var toFill = Activator.CreateInstance(memory.Value.Type);

                if (memory.Value is KawasakiMemoryVariable kwM)
                {
                    _robot.ReadVariable(kwM.CommandString, ref toFill);
                    kwM.Value = toFill;
                }
            }
        }

        protected async Task<bool> LoadMemory()
        {
            if (!_robot.IsConnected)
            {
                Log?.Warn($"{_robot.CommunicationString} is not connected");
                return false;
            }

            Task LoadMemoryTask = await LoadMemoryFromRobot();

            if (LoadMemoryTask.IsCompleted)
                return true;
            


        }
        */
        public KawasakiMemory(IRobotMemory<KawasakiMemoryVariable> memory)
        {
            if (memory == null)
                return;

            foreach(var itemInMemory in memory.Memory)
            {
                VirtualMemory[itemInMemory.Name] = itemInMemory;
            }
        }

        public bool AddMemory(string name, IRobotVariable varToAdd)
        {
            if (VirtualMemory.ContainsKey(name) || varToAdd == null)
                return false;

            VirtualMemory.Add(name, varToAdd);

            return true;
        }


        /// <summary>
        /// this function will fix when the dictionary string key is different from variable name
        /// </summary>
        public IRobotMemory<KawasakiMemoryVariable> FixVirtualizedMemory()
        {
            Dictionary<string, IRobotVariable> VirtualMemoryNew = new Dictionary<string, IRobotVariable>();

            foreach(var itemInMemory in VirtualMemory)
            {
                VirtualMemoryNew[itemInMemory.Value.Name] = itemInMemory.Value;
            }

            VirtualMemory = VirtualMemoryNew;

            return this;
        }

        public async Task<T> GetMemoryValueAsync<T>(string value)
        {
            if(!VirtualMemory.ContainsKey(value) || VirtualMemory[value].Value == null)
                return default(T);

            try
            {
                if (VirtualMemory[value].Type == typeof(int))
                {
                    return await Task.Run(() =>
                    {
                        return Task.FromResult((T)Convert.ChangeType(VirtualMemory[value].Value, VirtualMemory[value].Type));
                    });
                }
                else if(VirtualMemory[value].Type == typeof(string))
                {
                    return await Task.Run(() =>
                    {
                        return Task.FromResult((T)Convert.ChangeType(VirtualMemory[value].Value, VirtualMemory[value].Type));
                    });
                }
                else if (VirtualMemory[value].Type == typeof(double))
                {
                    return await Task.Run(() =>
                    {
                        return Task.FromResult((T)Convert.ChangeType(VirtualMemory[value].Value, VirtualMemory[value].Type));
                    });
                }
                else
                    return (T)VirtualMemory[value].Value;

            }
            catch
            {
                return default(T);
            }
        }
        public void SetDummyMemory()
        {
            KawasakiMemoryVariable testMemory = new KawasakiMemoryVariable()
            {
                Name = "test",
                Type = typeof(int),
            };

            AddMemory(testMemory.Name, testMemory);


            KawasakiMemoryVariable testMemory2 = new KawasakiMemoryVariable()
            {
                Name = "$stringTest",
                Type = typeof(string),
            };

            AddMemory(testMemory2.Name, testMemory2);
        }

        public void FreeVirtualMemory()
        {
            VirtualMemory.Clear();
        }

        public bool SetMemoryValue<T>(string name, T value)
        {
            return _robot.SetVariable(name, value);
        }

        public void SetLogger(ILog logger)
        {
            Log = logger;
        }


    }
}
