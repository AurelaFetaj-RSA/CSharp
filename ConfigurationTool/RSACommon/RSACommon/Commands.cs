using RSACommon.OpcServerDefinitions;
using RSACommon.WebApiDefinitions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSWareCommands
{
    public interface ICommand
    {
        string CommandString { get; set; }
        ICommand BuildCommand(params string[] value);
        RSWareCommand CommandType { get; set; }
        DateTime Created { get; }
    }

    public abstract class BaseCommand: ICommand
    {
        protected string _command = ""; //the last command built
        public string CommandString { get; set; } = "";
        public RSWareCommand CommandType { get; set; }
        public DateTime Created { get => _created;}
        private DateTime _created;

        protected readonly string separator = ",";
        public BaseCommand(RSWareCommand cmd, params string[] p)
        {
            CommandType = cmd;
            _command = MakeCommand(cmd, p);
            _created = DateTime.Now;
        }

        public BaseCommand(BaseCommand toCopy)
        {
            CommandType = toCopy.CommandType;
            _command = toCopy._command;
            CommandString = toCopy.CommandString;
            _created = toCopy._created; 
        }

        /// <summary>
        /// This function will fill value in string similar to "Test;{0};{1}". The parameters must be ordered in ascend way
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ICommand BuildCommand(params string[] value)
        {
            CommandString = String.Format(_command, value);
            return this;
        }

        public virtual string MakeCommand(RSWareCommand commandId, params string[] value)
        {
            string parametersString = "";

            foreach (string p in value)
            {
                parametersString += $"{p}{separator}";
            }

            _command = $"{commandId}{separator}{parametersString}";
            return _command;
        }
    }
}
