using System.IO;
using System;

namespace ProductionLaunch
{
    public static class Common
    {
        public static string file_log_hmi = "C:\\Robotsys\\";

        public static void WritelogFile(string logFile, string strLine)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logFile, true))
                {
                    writer.WriteLine(DateTime.Now.ToString("HH: mm:ss.fff") + " " + strLine);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }

    
}