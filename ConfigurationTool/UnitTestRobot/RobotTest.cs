using Microsoft.VisualStudio.TestTools.UnitTesting;
using Robot;
using System;
using KRcc;
using RSACommon;

namespace UnitTestRobot
{
    [TestClass]
    public class RobotUnitTest
    {
        [TestMethod]
        public void Communication()
        {
            RobotConfiguration robotConfigurator = new RobotConfiguration()
            {
                Host = "127.0.0.1",
                Port = 9105,
                ServiceName = "RDaneelOlivaw"
            };

            IRobot<IRobotVariable> robotTest = new Kawasaki(robotConfigurator, null);
            //robotTest.Start();

        }

        delegate void WriteText(int x);

        [TestMethod]
        public void CommunicationLoad()
        {
            RobotConfiguration robotConfigurator = new RobotConfiguration()
            {
                Host = "127.0.0.1",
                Port = 9105,
                ServiceName = "RDaneelOlivaw"
            };

            var robotTest = new Kawasaki(robotConfigurator, null);

            if (robotTest is Kawasaki kwService)
            {
                if (kwService.RobotIsConnected())
                {
                }

                int test = kwService.krccCommmunication.load(@"C:\Work\Github\FUTURE_SERVER\ConfigurationTool\UnitTestRobot\TX1.LC");

                if (test != 0)
                {
                    Assert.Fail();
                }

                string retu = string.Empty;
                kwService.ReadCommand<string>("po ciclo[1,100,1]", ref retu);

                Console.WriteLine(retu);

            }
        }

        [TestMethod]
        public void CommunicationLoadRSA()
        {
            RobotConfiguration robotConfigurator = new RobotConfiguration()
            {
                Host = "127.0.0.1",
                Port = 9105,
                ServiceName = "RDaneelOlivaw"
            };

            var robotTest = new Kawasaki(robotConfigurator, null);

            if (robotTest is Kawasaki kwService)
            {
                if (kwService.RobotIsConnected())
                {
                }

                int test = (int)kwService.Load(@"C:\Work\Github\FUTURE_SERVER\ConfigurationTool\UnitTestRobot\TX2.LC");

                if (test != 0)
                {
                    Assert.Fail();
                }

                string retu = string.Empty;
                kwService.ReadCommand<string>("po ciclo2[1,100,1]", ref retu);

                Console.WriteLine(retu);

            }
        }
    }
}
