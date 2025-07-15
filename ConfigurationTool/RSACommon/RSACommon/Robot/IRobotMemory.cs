using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot
{
    public interface IRobotMemory<out V> where V: class, IRobotVariable
    {
        bool AddMemory(string name, IRobotVariable varToAdd);
        //T GetMemoryValue<T>(string value);
        bool SetMemoryValue<T>(string name, T value);
        void SetDummyMemory();
        IList<IRobotVariable> Memory { get; }
        void FreeVirtualMemory();
        void SetRobot(IRobot<IRobotVariable> robot);
        IRobotMemory<V> FixVirtualizedMemory();
        void SetLogger(ILog logger);
        Task<long> LoadMemory();
        Task<long> LoadMemory(RefreshPriority prio);
        Task<T> GetMemoryValueAsync<T>(string value);

    }

    public interface IRobotVariable
    {
        Type Type { get; set; }
        string Name { get; set; }
        int Key { get; set; }
        int RefreshTimeMS { get; set; }
        object Value { get; set; }
        DateTime LastTimeRefreshed { get; set; }
        RefreshPriority Priority { get; set; }
    }

    public enum RefreshPriority
    {
        High = 0,
        Mid = 1,
        Low = 2
    }

    public abstract class BaseRobotVariable: IRobotVariable
    {
        public static int REFRESH_TIME_DEFAULT = -1;
        public Type Type { get; set; }
        public string Name { get; set; }
        public int Key { get; set; }
        public int RefreshTimeMS { get; set; } = REFRESH_TIME_DEFAULT;
        public DateTime LastTimeRefreshed { get; set; } = DateTime.Now;
        public object Value { get; set; }
        public RefreshPriority Priority { get; set; } = RefreshPriority.Low;
    }

}
