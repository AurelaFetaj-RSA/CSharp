using RSWareCommands;
using log4net;
using RSAInterface.Logger;
using RSACommon.WebApiDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace RSACommon
{
    public enum MessageResponse
    {
        Success = 0,
        Timeout = -1,
        NoPrompt = -2,
        Bad = -3
    }
    public enum ACK
    {
        Ok = 1,
        Free = 0,
        NotOk = -1,
        Initialize = 25
    }
    public abstract class User
    {
        protected string separator = ";";
        public int Id { get => _id; } //Serializer
        protected int _id = -1;
        public string Name { get; set; }
        public string Ip { get; set; }

        protected string _command = "";
        public virtual int PriorityMessage { get => 0; }
        public DateTime CreationalTime { get; set; }    
        public Dictionary<RSWareCommand, List<ICommand>> Commands { get; } = new Dictionary<RSWareCommand, List<ICommand>>();
        public List<ICommand> Hystory = new List<ICommand>();
        public volatile bool IsConnected = false;
        private System.Timers.Timer IsConnectedTimer { get; set; } = new System.Timers.Timer(20000);
        public event EventHandler<CreateCommandEventArgs> OnCreateCommandEvent;

        public virtual void OnRsWareCommandCreated(CreateCommandEventArgs e)
        {
            if (OnCreateCommandEvent != null)
                OnCreateCommandEvent(this, e);
        }

        public void SetId(int id)
        {
            _id = id;
        }

        public User()
        {
            CreationalTime = DateTime.Now;

            IsConnectedTimer.Elapsed += IsConnectedTimer_Elapsed;

            IsConnectedTimer.AutoReset = true;
            IsConnectedTimer.Start();
        }


        /// <summary>
        /// IF elapsed, the user is not connected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IsConnectedTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            IsConnected = false;
        }

        public void KeepAlive()
        {
            IsConnectedTimer.Stop();
            IsConnectedTimer.Start();

            IsConnected = true;
        }

        /// <summary>
        /// This function will add the commands to a dictionary, every dictionary has a queue
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public virtual bool AddCommandsType(ICommand commandToAdd)
        {
            if(!Commands.ContainsKey(commandToAdd.CommandType)) //Se non contiene il comando, vado avanti ed inserisco l'oggetto
            {
                Commands[commandToAdd.CommandType] = new List<ICommand>();
                return true;
            }

            return false;
        }

        public int CommandQueueSize
        {
            get => Commands.SelectMany(x => x.Value).Count();
        }

        public Error MakeCommand(ICommand newCommand, params string[] p)
        {
            if (newCommand == null)
                return Error.COMMAND_NOT_FOUND;

            if(Commands.ContainsKey(newCommand.CommandType) && Commands[newCommand.CommandType] != null)
            {
                Commands[newCommand.CommandType].Add(newCommand.BuildCommand(p));
                OnRsWareCommandCreated(new CreateCommandEventArgs(newCommand, p));
                return Error.OK;
            }

            return Error.COMMAND_NOT_FOUND;
        }

        public virtual Error SelectFirstCommand(out ICommand command)
        {
            List<ICommand> commandsList = Commands.SelectMany(x => x.Value).ToList();

            if(commandsList.Count != 0)
            {
                ICommand cmd = commandsList.Aggregate((curMin, x) => (x.Created < curMin.Created) ? x : curMin);

                command = cmd;

                //aggiungo alla storia dei miei comandi
                Hystory.Add(cmd);
                //Rimuovo dalla coda
                Commands[cmd.CommandType].Remove(cmd);

                return Error.OK;
            }

            command = null;

            return Error.COMMAND_NOT_FOUND;

        }


    }

    public interface IServerShared
    {
        //event EventHandler OnIncrementalCallCountEvent;
        //event EventHandler<EventArgs> OnAckChangeValueEvent;
        //event EventHandler<EventArgs> OnErrorChangeValueEvent;
        //event EventHandler<EventArgs> OnCreateCommandEvent;
        //event EventHandler<EventArgs> OnCommandReadEvent;
        User UserInfo { get; }
        void Increment();
        string LastCommand { get; }
        ACK AckStatus { get; set; }
        int CallCount { get; set; }
        Error Error { get; }
        void OnReadCommand(CommandRequestedEventArgs events);
        void OnAckChangedValue(AckChangeEventArgs events);
        void OnAckRequest(AckRequestEventArgs events);
        void OnSetError(RSACommon.WebApiDefinitions.ErrorEventArgs events);
        Error SelectCommand(out ICommand cmd);
    }
}
