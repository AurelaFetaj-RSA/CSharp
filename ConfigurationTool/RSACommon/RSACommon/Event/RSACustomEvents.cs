using Robot;
using RSACommon.WebApiDefinitions;
using RSAInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace RSACommon.Event
{
    public static class RSACustomEvents
    {

        public abstract class AbstractRSAEventArgs: EventArgs
        {
            public DateTime CreationTime { get; private set; }
            public IService Service { get; private set; }
            public AbstractRSAEventArgs(IService whichService)
            {
                Service = whichService;
                CreationTime = DateTime.Now;
            }
        }

        public class ProgramsReadEndedEventArgs: AbstractRSAEventArgs
        {
            public int Loaded { get; private set; }
            public long ElapsedTimeMS { get; private set; }
            public ProgramsReadEndedEventArgs(IService whichService, int count, long elapsedTimeMS) : base(whichService)
            {
                Loaded = count;
                ElapsedTimeMS = elapsedTimeMS;
            }
        }

        public class RobotJobFinishedEventArgs : AbstractRSAEventArgs
        {
            public int StatusValue { get; private set; }
            public RobotJobFinishedEventArgs(IRobot<IRobotVariable> whichService, int statusValue) : base(whichService)
            {
                StatusValue = statusValue;
            }
        }

        public class KawasakiCheckVariableWithTimeoutEventArgs<T> : AbstractRSAEventArgs
        {

            public Dictionary<string, T> Commands { get; set; } = new Dictionary<string, T>();
            //public string[] Commands { get; private set; }
            //public T[] ExpectedValue { get; private set; }
            public int TimeOut { get; private set; }
            public KawasakiCheckVariableWithTimeoutEventArgs(IService whichService, string variableToRead, T expectedValue, int timeout) : base(whichService)
            {
                Commands[variableToRead] = expectedValue;
                TimeOut = timeout;
            }

            public KawasakiCheckVariableWithTimeoutEventArgs(IService whichService, string[] variablesToRead, T[] expectedsValue, int timeout) : base(whichService)
            {
                for(int i = 0; i < expectedsValue.Length; i++)
                {
                    if (i < expectedsValue.Length)
                    {
                        Commands[variablesToRead[i]] = expectedsValue[i];
                    }
                }

                TimeOut = timeout;
            }
        }

        public class KawasakiTimeoutResultEventArgs: AbstractRSAEventArgs
        {
            public Dictionary<string, bool> Commands { get; private set; }
            public MessageResponse Result { get; private set; }
            public string Message { get; private set; } = string.Empty;
            public KawasakiTimeoutResultEventArgs(IService whichService, Dictionary<string, bool> cmdToExecute, MessageResponse result, string message = "") : base(whichService)
            {
                Commands = cmdToExecute;
                Result = result;
                Message = message;
            }
        }

        public class KawasakiCommandDelayedEventArgs : KawasakiCommandEventArgs
        {
            public int Milliseconds { get; private set; } = 500;
            public bool AutoReset { get; private set; } = false;
            public KawasakiCommandDelayedEventArgs(IService whichService, string command, int milliseconds = 500, bool autoReset = false) : base(whichService, command)
            {
                Milliseconds = milliseconds;
                AutoReset = autoReset;
            }
        }
        public class KawasakiCommandEventArgs: AbstractRSAEventArgs
        {
            public string Command { get; private set; }
            public KawasakiCommandEventArgs(IService whichService, string command) : base(whichService)
            {
                Command = command;
            }
        }

        public class ServiceConnectionEventArgs : AbstractRSAEventArgs
        {
            public ServiceConnectionEventArgs(IService whichService, string whoClient): base(whichService)
            {
                WhoConnected = whoClient;
                ConnectionTime = DateTime.Now;
            }

            public DateTime ConnectionTime { get; private set; }
            public string WhoConnected { get; private set; }
        }

        public class ServiceStopEventArgs : ServiceConnectionEventArgs
        {
            public ServiceStopEventArgs(IService whichService, string whoClient) : base(whichService, whoClient)
            {
            }
        }

        public class KeepAliveTimeOutEventArgs : AbstractRSAEventArgs
        {
            public KeepAliveTimeOutEventArgs(IService whoHasTimeout): base(whoHasTimeout)
            {
            }
        }

        public class RobotToolUpdateEventArgs : AbstractRSAEventArgs
        {
            public string RobotToolStatus { get; set; } = "NO";
            public RobotToolUpdateEventArgs(IService whoHasTimeout, string status) : base(whoHasTimeout)
            {
                RobotToolStatus = status;
            }
        }

        public class SpeedIsChangedEventArgs : AbstractRSAEventArgs
        {
            public double RobotSpeedRead { get; set; } = 0;
            public SpeedIsChangedEventArgs(IService whoHasTimeout, double speed) : base(whoHasTimeout)
            {
                RobotSpeedRead = speed;
            }
        }

        public class WorkingTableIsChangedEventArgs : AbstractRSAEventArgs
        {
            public int WorkingTableRead { get; set; } = 0;
            public WorkingTableIsChangedEventArgs(IService whoHasTimeout, int station) : base(whoHasTimeout)
            {
                WorkingTableRead = station;
            }
        }

        public class KeepAliveOkEventArgs : AbstractRSAEventArgs
        {
            public KeepAliveOkEventArgs(IService keepAlive, string client): base(keepAlive)
            {
                Client = client;
            }
            public string Client { get; private set; }
        }

        public class OPCSubscriptionEventArgs : AbstractRSAEventArgs
        {
            public OPCSubscriptionEventArgs(IService whoHasTimeout, string clientName, string variableName): base(whoHasTimeout)
            {
                Client = clientName;
                VariableSubscripted = variableName;
            }

            public string Client { get; private set; }
            public string VariableSubscripted { get; private set; }
        }

        public class TimeoutEventArgs : AbstractRSAEventArgs
        {
            public MessageResponse Result { get; private set; }
            public ACK Ack { get; private set; }
            public TimeoutEventArgs(IService whichService, MessageResponse response, ACK value) : base(whichService)
            {
                Result = response;
                Ack = value;
            }
        }

        public class RecipeSelectedByUserEventArgs : AbstractRSAEventArgs
        {
            User User { get; set; } = null;
            public RecipeSelectedByUserEventArgs(IService whichService, User user) : base(whichService)
            {
                User = user;
            }
        }

        public class RecipeSelectedByMesEventArgs : AbstractRSAEventArgs
        {
            User User { get; set; } = null;
            public RecipeSelectedByMesEventArgs(IService whichService, User user) : base(whichService)
            {
                User = user;
            }
        }

        public static event EventHandler<TimeoutEventArgs> AckTimeoutEvent;
        public static event EventHandler<KeepAliveTimeOutEventArgs> KeepAliveTimeoutEvent;
        public static event EventHandler<KeepAliveOkEventArgs> KeepAliveOkEvent;
        public static event EventHandler<OPCSubscriptionEventArgs> OPCServerSubscriptionEvent;
        public static event EventHandler<ServiceConnectionEventArgs> ServiceConnectionEvent;
        public static event EventHandler<ServiceConnectionEventArgs> ServiceDisconnectionEvent;
        public static event EventHandler<ServiceStopEventArgs> ServiceStopEvent;
        public static event EventHandler<KawasakiCommandEventArgs> KawasakiCommandEvent;
        public static event EventHandler<KawasakiCommandDelayedEventArgs> KawasakiCommandEventDelayed;
        public static event EventHandler<KawasakiCheckVariableWithTimeoutEventArgs<int>> KawasakiCheckVariableIntWithTimeoutEvent;
        public static event EventHandler<KawasakiTimeoutResultEventArgs> KawasakiResultTimeoutEvent;
        public static event EventHandler<ProgramsReadEndedEventArgs> ServiceHasLoadProgramEvent;
        public static event EventHandler<RecipeSelectedByMesEventArgs> RecipeIsSelectedByMesEvent;
        public static event EventHandler<RecipeSelectedByUserEventArgs> RecipeIsSelectedByUserEvent;
        public static event EventHandler<SpeedIsChangedEventArgs> SpeedIsChangedEvent;
        public static event EventHandler<WorkingTableIsChangedEventArgs> WorkingTableIsChangedEvent;
        public static event EventHandler<RobotToolUpdateEventArgs> RobotToolUpdateEvent;
        public static void OnSetRecipeByMesEvent(RecipeSelectedByMesEventArgs e)
        {
            EventHandler<RecipeSelectedByMesEventArgs> raiseEvent = RecipeIsSelectedByMesEvent;

            if (raiseEvent != null)
            {
                raiseEvent(null, e);
            }
        }
        public static void OnSetRecipeByUserEvent(RecipeSelectedByUserEventArgs e)
        {
            EventHandler<RecipeSelectedByUserEventArgs> raiseEvent = RecipeIsSelectedByUserEvent;

            if (raiseEvent != null)
            {
                raiseEvent(null, e);
            }
        }

        // Wrap event invocations inside a protected virtual method
        // to allow derived classes to override the event invocation behavior
        public static void OnServiceLoadedProgramEndEvent(ProgramsReadEndedEventArgs e)
        {
            EventHandler<ProgramsReadEndedEventArgs> raiseEvent = ServiceHasLoadProgramEvent;

            if (raiseEvent != null)
            {
                raiseEvent(null, e);
            }
        }
        public static void OnKawasakiCommandEvent(KawasakiCommandEventArgs e)
        {
            EventHandler<KawasakiCommandEventArgs> raiseEvent = KawasakiCommandEvent;

            if (raiseEvent != null)
            {
                raiseEvent(null, e);
            }
        }

        public static void OnKawasakiCommandEventDelayed(KawasakiCommandDelayedEventArgs e)
        {
            EventHandler<KawasakiCommandDelayedEventArgs> raiseEvent = KawasakiCommandEventDelayed;

            if (raiseEvent != null)
            {
                raiseEvent(null, e);
            }
        }

        public static void OnAckTimeout(TimeoutEventArgs e)
        {
            EventHandler<TimeoutEventArgs> raiseEvent = AckTimeoutEvent;

            if (raiseEvent != null)
            {
                raiseEvent(null, e);
            }
        }

        public static void OnKawasakiCheckVariableWithTimeout(KawasakiCheckVariableWithTimeoutEventArgs<int> e)
        {
            EventHandler<KawasakiCheckVariableWithTimeoutEventArgs<int>> raiseEvent = KawasakiCheckVariableIntWithTimeoutEvent;

            if (raiseEvent != null)
            {
                raiseEvent(null, e);
            }
        }

        public static void OnKawasakiTimeout(KawasakiTimeoutResultEventArgs e)
        {
            EventHandler<KawasakiTimeoutResultEventArgs> raiseEvent = KawasakiResultTimeoutEvent;

            if (raiseEvent != null)
            {
                raiseEvent(null, e);
            }
        }

        // Wrap event invocations inside a protected virtual method
        // to allow derived classes to override the event invocation behavior
        public static void OnKeepAliveTimeotEvent(KeepAliveTimeOutEventArgs e)
        {
            EventHandler<KeepAliveTimeOutEventArgs> raiseEvent = KeepAliveTimeoutEvent;

            if (raiseEvent != null)
            {
                raiseEvent(null, e);
            }
        }

        public static void OnKeepAliveOkEvent(KeepAliveOkEventArgs e)
        {
            EventHandler<KeepAliveOkEventArgs> raiseEvent = KeepAliveOkEvent;

            if (raiseEvent != null)
            {
                raiseEvent(null, e);
            }
        }


        public static void OnOPCServerSubscriptionEvent(OPCSubscriptionEventArgs e)
        {
            EventHandler<OPCSubscriptionEventArgs> raiseEvent = OPCServerSubscriptionEvent;

            if (raiseEvent != null)
            {
                raiseEvent(null, e);
            }
        }

        public static void OnServiceConnection(ServiceConnectionEventArgs e)
        {
            EventHandler<ServiceConnectionEventArgs> raiseEvent = ServiceConnectionEvent;

            if (raiseEvent != null)
            {
                raiseEvent(null, e);
            }
        }

        public static void OnServiceDisconnected(ServiceConnectionEventArgs e)
        {
            EventHandler<ServiceConnectionEventArgs> raiseEvent = ServiceDisconnectionEvent;

            if (raiseEvent != null)
            {
                raiseEvent(null, e);
            }
        }

        public static void OnServiceStop(ServiceStopEventArgs e)
        {
            EventHandler<ServiceStopEventArgs> raiseEvent = ServiceStopEvent;

            if (raiseEvent != null)
            {
                raiseEvent(null, e);
            }
        }

        public static void OnSpeedUpdate(SpeedIsChangedEventArgs e)
        {
            EventHandler<SpeedIsChangedEventArgs> raiseEvent = SpeedIsChangedEvent;

            if (raiseEvent != null)
            {
                raiseEvent(null, e);
            }
        }

        public static void OnWorkingTableUpdate(WorkingTableIsChangedEventArgs e)
        {
            EventHandler<WorkingTableIsChangedEventArgs> raiseEvent = WorkingTableIsChangedEvent;

            if (raiseEvent != null)
            {
                raiseEvent(null, e);
            }
        }

        public static void OnRobotToolUpdate(RobotToolUpdateEventArgs e)
        {
            EventHandler<RobotToolUpdateEventArgs> raiseEvent = RobotToolUpdateEvent;

            if (raiseEvent != null)
            {
                raiseEvent(null, e);
            }
        }
    }
}
