using System;
using System.Net.NetworkInformation;
using RSACommon;
using RSWareCommands;

namespace RSACommon.WebApiDefinitions
{
    public interface ISharedList
    {
        void AddUser<T>(User userToAdd) where T : IServerShared;
        void SetAllACKto(ACK value);
        void CleanUserList();
        User GetUserInstance(int id);
        IServerShared GetWebSharedUserInstance(int id);
        IServerShared GetWebSharedUserInstance(User userToAdd);
    }
    public enum RSWareIdMessage
    {
        ACK = 0,
        Error = 1
    }
    public enum RSWareCommand
    {
        CSTAA,
        CSTOA,
        SET_RECIPE,
        CPARK,
        CSTRT
    }

    public class AckRequestEventArgs : EventArgs
    {
        public DateTime GeneratedTime;
        public User User;
        public AckRequestEventArgs(User user)
        {
            GeneratedTime = DateTime.Now;
            User = user;
            User.KeepAlive();
        }
    }
    public class AckChangeEventArgs : EventArgs
    {
        public ACK AckValue;
        public DateTime GeneratedTime;
        public User User;
        public AckChangeEventArgs(ACK value, User user)
        {
            GeneratedTime = DateTime.Now;
            AckValue = value;
            User = user;
        }
    }
    public class ErrorEventArgs : EventArgs
    {
        public Error EventError;
        public DateTime GeneratedTime;
        public User User;
        public ErrorEventArgs(Error value, User user)
        {
            GeneratedTime = DateTime.Now;
            EventError = value;
            User = user;
        }
    }

    public class CreateCommandEventArgs : EventArgs
    {
        public ICommand Cmd;
        public string[] Parameters;
        public DateTime GeneratedTime;
        public CreateCommandEventArgs(ICommand cmd, params string[] par)
        {
            Parameters = par;
            Cmd = cmd;
            GeneratedTime = DateTime.Now;
        }
    }

    public class CommandRequestedEventArgs : EventArgs
    {
        public ICommand Command;
        public DateTime GeneratedTime;
        public User User;
        public CommandRequestedEventArgs(ICommand cmd, User user)
        {
            Command = cmd;
            GeneratedTime = DateTime.Now;
            User = user;
        }
    }


}
