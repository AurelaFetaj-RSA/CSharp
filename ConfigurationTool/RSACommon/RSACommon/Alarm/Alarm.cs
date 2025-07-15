using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSACommon.Alarm.Core
{
    public enum Severity
    {
        Debug = 0,
        Info = 1,
        Warn = 2,
        Alarm = 3,
    }

    public class AlarmStatus
    {
        public string Message { get; set; } = string.Empty;
        public int Value { get; set;}
        public int BitMask { get; set; }
        public Severity Severity { get; set; }

        public AlarmStatus(int bitPosition, string message = "", Severity severity = Severity.Alarm)
        {
            Value = AlarmParser.GetBitMask(bitPosition);
            BitMask = Value;
            Message = message;
            Severity = severity;
        }
    }

    public class AlarmParser
    {
        public List<string> AlarmFiles { get; set; } = new List<string>();
        private Dictionary<int, AlarmStatus> AlarmDictionary { get; set; } = new Dictionary<int, AlarmStatus>();
        public AlarmParser(List<string> filename)
        {
            AlarmFiles = filename;
        }
        public AlarmParser(string filename)
        {
            AlarmFiles = new List<string>() { filename };
        }
        public AlarmParser Load()
        {
            return Load(AlarmFiles);
        }

        public static int GetBitMask(int bitePosition)
        {
            return (int)Math.Pow(2,bitePosition - 1);
        }

        public AlarmParser Load(List<string> filenames)
        {

            foreach (string filename in filenames)
            {

                if (!File.Exists(filename) || string.IsNullOrEmpty(filename))
                    continue;

                using (var sr = new StreamReader(filename))
                {
                    string text = sr.ReadToEnd();
                    string[] splittedLine = text.Split(new char[] { '\n', '\r' });

                    try
                    {
                        foreach (string line in splittedLine)
                        {
                            string[] dataInLine = line.Split(';');

                            if (dataInLine.Length > 1 && int.TryParse(dataInLine[0], out int bitMask))
                            {
                                if (!string.IsNullOrEmpty(dataInLine[1]))
                                {
                                    AlarmDictionary.Add(AlarmParser.GetBitMask(bitMask), new AlarmStatus(bitMask, dataInLine[1]));

                                }
                            }
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Error on parsing files");
                    }
                }
            }

            return this;
        }

        public List<AlarmStatus> GetAlarms(int value)
        {
            List<AlarmStatus> alarms = new List<AlarmStatus>();

            foreach (var bitmask in AlarmDictionary.Keys)
            {
                bool alarmIsReal = (bitmask & value) == bitmask? true : false;

                if (alarmIsReal)
                {
                    alarms.Add(AlarmDictionary[bitmask]);
                }
            }

            return alarms;
        }
    }
}
