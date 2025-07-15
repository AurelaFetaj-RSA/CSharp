using Robot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryTester
{
    public class Program
    {

        public static IRobot<IRobotVariable> Robot;
        public static int MAX_VARIABLE = 100;

        [STAThread]
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World");

            RobotConfiguration robotConfigurator = new RobotConfiguration()
            {
                Host = "172.31.10.147",
                Port = 9105,
                ServiceName = "RDaneelOlivaw",
                Active = true,
            };

            Robot = new Kawasaki(robotConfigurator, null);
            Robot.Start();

            //if (!Robot.IsActive)
            //    Assert.Fail();

            InitRobotMemory(Robot);

            Stopwatch t = new Stopwatch();

            Task.Run(async () => await Program.ReadRealMemory());
            Console.WriteLine("Exit from memory read");
            //Task.Run(async () => await Program.ReadVirtualizedMemory());            
            //Console.WriteLine("Exit from virtualized memory read");


            Console.ReadLine();
        }

        public static async Task ReadRealMemory()
        {
            long elapsedTime =  await Robot.VirtualizedMemory.LoadMemory();

            Console.WriteLine($"Elapsed Time {elapsedTime}");
        }

        public static async Task ReadVirtualizedMemory()
        {
            List<int> test = new List<int>();

            Stopwatch t = new Stopwatch();
            t.Start();

            //if (!Robot.IsActive)
            //    Assert.Fail();

            foreach (var memory in Robot.VirtualizedMemory.Memory)
            {
                test.Add(await Robot.VirtualizedMemory.GetMemoryValueAsync<int>(memory.Name));
            }

            foreach(var value in test)
            {
                Console.WriteLine($"{value}");
            }

            t.Stop();

            Console.WriteLine($"Time for reading: {t.ElapsedMilliseconds}");
        }


        public static void InitRobotMemory(IRobot<IRobotVariable> robot)
        {
            robot.CreateNewVirtualMemory();

            for (int i = 0; i < 10; i++)
            {
                robot.VirtualizedMemory.AddMemory($"test_{i}", new KawasakiMemoryVariable()
                {
                    Name = $"test_{i}",
                    Type = typeof(int),
                    Key = i,
                    Value = i.ToString()
                });

                string output = "";
                robot.SetCommand($"test_{i} = {i}",ref output);
            }



            //robot.MemoryTimer.Start();
        }

    }
}
