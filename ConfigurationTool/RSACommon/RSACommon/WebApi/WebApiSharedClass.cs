using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using RSWareCommands;
using RSACommon;
using RSACommon.OpcServerDefinitions;
using RSACommon.WebApiDefinitions;
using RSACommon.RecipeParser;
using System.Timers;

namespace WebApi
{
    public class WebApiSharedList: ISharedList
    {
        private Dictionary<User, IServerShared> _sharedInfoDictionary = new Dictionary<User, IServerShared>();
        public string ObjectName { get; set; } = "InitName";

        /// <summary>
        /// Will create user class instance of type T and will associate userToAdd to shared class, the T must be IServerShared
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="userToAdd"></param>
        public void AddUser<T>(User userToAdd) where T: IServerShared
        {
            if(!_sharedInfoDictionary.ContainsKey(userToAdd)) 
            {
                int lastId = LastId();

                userToAdd.SetId(++lastId);
                _sharedInfoDictionary[userToAdd] = (T)Activator.CreateInstance(typeof(T), (userToAdd));
            }
        }

        public void SetAllACKto(ACK value)
        {
            foreach(var instance in _sharedInfoDictionary)
            {
                instance.Value.AckStatus = value;
            }
        }

        public void CleanUserList()
        {
            _sharedInfoDictionary.Clear();
        }

        private int LastId()
        {
            if (_sharedInfoDictionary.Count == 0)
                return -1;

            return _sharedInfoDictionary.Keys.OrderBy(x => x.Id).Last().Id;
        }

        public User GetUserInstance(int id)
        { 
            return _sharedInfoDictionary.Keys.FirstOrDefault(x => x.Id == id);
        }

        public IServerShared GetWebSharedUserInstance(int id)
        {
            User obj = _sharedInfoDictionary.Keys.FirstOrDefault(x => x.Id == id);
                
            return GetWebSharedUserInstance(obj);
        }

        public IServerShared GetWebSharedUserInstance(User userToAdd)
        {
            IServerShared toShow = null;

            if(userToAdd == null)
                return toShow; //Null return

            _sharedInfoDictionary.TryGetValue(userToAdd, out toShow);
            
            return toShow;
        }
    }

    public class WebApiRSWareSharedClass : IServerShared
    {
        public User UserInfo { get; }
        public ACK AckStatus { get => _status; set => _status = value; }
        private ACK _status = ACK.NotOk;
        private string _recipe = "AMA33";
        public string Recipe { get => _recipe; }
        private int _callCount = 0;
        public int CallCount { get => _callCount; set => _callCount = value; }

        private Error _rSWareError = Error.OK;
        public Error Error { get => _rSWareError; }
        public string LastCommand { get; private set; }

        private string _lastCommand = string.Empty;

        public event EventHandler OnIncrementalCallCountEvent;
        public event EventHandler<ErrorEventArgs> OnErrorChangeValueEvent;
        public event EventHandler<CreateCommandEventArgs> OnCreateCommandEvent;
        public event EventHandler<CommandRequestedEventArgs> OnCommandReadEvent;
        public event EventHandler<AckChangeEventArgs> OnAckChangeValueEvent;
        public event EventHandler<AckRequestEventArgs> OnAckRequestEvent;


        public WebApiRSWareSharedClass(User info)
        {
            UserInfo = info;
            UserInfo.OnCreateCommandEvent += UserInfo_OnCreateCommandEvent;
        }

        /// <summary>
        /// will set recipe if the command recipe is in folder
        /// </summary>
        /// <param name="commandString"></param>
        /// <returns></returns>
        public bool SetRecipe(string commandString)
        {
            Recipe commandRecipe = RecipeFinder.ExtractRecipeFromCommands(commandString);

            if (UserInfo is RSWareUser rsWareUser)
            {
                var fileList = RecipeFinder.FindRecipeInFolder(rsWareUser.FolderPath);

                foreach(Recipe f in fileList)
                {
                    if (RSACommon.RecipeParser.Recipe.Comparer(f, commandRecipe))
                    {
                        _recipe = f.Model;
                        return true;
                    }
                }
            }

            return false;
        }


        private void UserInfo_OnCreateCommandEvent(object sender, CreateCommandEventArgs e)
        {
            // Console.WriteLine($"User: {e.Cmd.CommandType} cmd value: {e.Cmd.CommandString}  date: {e.GeneratedTime}");

            if(e.Cmd is RecipeCommand recipeCommand)
            {
                if(!SetRecipe(recipeCommand.CommandString))
                {
                    AckStatus = ACK.NotOk;
                }
                else
                    AckStatus = ACK.Ok;
            }
            else
            {
                AckStatus = ACK.Ok;
            }


        }

        public void Increment()
        {
            CallCount++;
            OnCall();
        }

        private void Status_OnAckChangeValue(object sender, AckChangeEventArgs e)
        {
            Console.WriteLine($"User: {e.User.Name}, Ack value: {e.AckValue}, date: {e.GeneratedTime}");
        }

        public virtual void OnAckRequest(AckRequestEventArgs e)
        {
            if (OnAckRequestEvent != null)
                OnAckRequestEvent(this, e);
        }
        public virtual void OnRsWareCommandCreated(CreateCommandEventArgs e)
        {
            if (OnCreateCommandEvent != null)
                OnCreateCommandEvent(this, e);
        }
        public virtual void OnAckChangedValue(AckChangeEventArgs e)
        {
            if (OnAckChangeValueEvent != null)
                OnAckChangeValueEvent(this, e);
        }
        public virtual void OnCall()
        {
            if (OnIncrementalCallCountEvent != null)
                OnIncrementalCallCountEvent(this, EventArgs.Empty);
        }

        public virtual void OnSetError(ErrorEventArgs events)
        {
            if (OnErrorChangeValueEvent != null)
                OnErrorChangeValueEvent(this, events);
        }

        public virtual void OnReadCommand(CommandRequestedEventArgs events)
        {
            if (OnCommandReadEvent != null)
                OnCommandReadEvent(this, events);
        }

        public Error SelectCommand(out ICommand cmd)
        {
            var result = UserInfo.SelectFirstCommand(out cmd);

            if(cmd != null)
                _lastCommand = cmd.CommandString;

            return result;
        }

        //public Error MakeCommand(RSWareCommand cmdType, out string cmd, params string[] p)
        //{

        //    if(UserInfo.Commands.TryGetValue(cmdType, out ICommand command))
        //    {
        //        cmd = Command = command.BuildCommand(p).Cmd;

        //        CommandQueue.Add(command);

        //        return Error.OK;
        //    }

        //    cmd = Command = null;

        //    return Error.COMMAND_NOT_FOUND;
        //}
    }
}
