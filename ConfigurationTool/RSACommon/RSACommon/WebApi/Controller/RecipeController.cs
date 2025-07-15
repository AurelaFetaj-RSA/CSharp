using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using WebApi;
using RSACommon.WebApiDefinitions;
using RSACommon;
using System.Windows.Input;
using RSWareCommands;

namespace WebApi.Controllers
{
    public class RecipeController : ApiController
    {
        private ISharedList SharedList { get; set; }

        public RecipeController(ISharedList sharedInfo)
        {
            SharedList = sharedInfo;
        }

        [HttpGet]
        public HttpResponseMessage GetAck(int userID)
        {
            IServerShared sharedInfo = SharedList?.GetWebSharedUserInstance(userID);

            sharedInfo.OnAckRequest(new AckRequestEventArgs(sharedInfo.UserInfo));

            if (sharedInfo == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, sharedInfo.AckStatus);
            return response;
        }


        [HttpGet]
        public HttpResponseMessage GetError(int userID)
        {
            IServerShared sharedInfo = SharedList.GetWebSharedUserInstance(userID);

            if (sharedInfo == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            sharedInfo.Increment();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, sharedInfo.Error);
            return response;

        }

        [HttpGet]
        public HttpResponseMessage GetRecipe(int userID)
        {
            IServerShared sharedInfo = SharedList.GetWebSharedUserInstance(userID);

            if (sharedInfo == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            HttpResponseMessage response = null;
            if (sharedInfo is WebApiRSWareSharedClass rsWareRecipe)
            {
                response = Request.CreateResponse(HttpStatusCode.OK, rsWareRecipe.Recipe);
            }


            response = Request.CreateResponse(HttpStatusCode.BadRequest);
            return response;
        }

        [HttpGet]
        public HttpResponseMessage GetAllCommands(int userID)
        {
            IServerShared sharedInfo = SharedList.GetWebSharedUserInstance(userID);

            if (sharedInfo == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            string result = "";

            if(sharedInfo.UserInfo.Commands.Keys.Count == 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Empty command list");
            }

            foreach (var dictKV in sharedInfo.UserInfo.Commands)
            {
                foreach(RSWareCommands.ICommand cmd in dictKV.Value)
                {
                    result += $"\"{dictKV.Key} - {cmd.CommandString}\"\n";
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }


        [HttpGet]
        public HttpResponseMessage GetCommand(int userID)
        {
            IServerShared sharedInfo = SharedList.GetWebSharedUserInstance(userID);

            if (sharedInfo == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            sharedInfo.Increment();
            RSWareCommands.ICommand cmdToCheck;

            if(sharedInfo.SelectCommand(out cmdToCheck) == Error.OK)
            {
                sharedInfo.OnReadCommand(new CommandRequestedEventArgs(cmdToCheck, sharedInfo.UserInfo));
                return Request.CreateResponse(HttpStatusCode.OK, cmdToCheck.CommandString);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest, "no command");
        }

        //https://localhost:4600/recipe/UpdateID/0?value=1
        [HttpPut]
        public HttpResponseMessage UpdateId(int userID, int id, int value)
        {
            User user;

            IServerShared sharedInfo = SharedList.GetWebSharedUserInstance(userID);

            if (sharedInfo == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            user = sharedInfo.UserInfo;

            switch ((RSWareIdMessage)id)
            {
                case RSWareIdMessage.ACK:
                    sharedInfo.AckStatus = (ACK)value;
                    sharedInfo.OnAckChangedValue(new AckChangeEventArgs((ACK)value, user));

                    return Request.CreateResponse(HttpStatusCode.OK, sharedInfo.AckStatus);

                case RSWareIdMessage.Error:
                    sharedInfo.OnSetError(new ErrorEventArgs((Error)value, user));
                        

                    return Request.CreateResponse(HttpStatusCode.OK, sharedInfo.Error);
                default:
                    sharedInfo.OnSetError(new ErrorEventArgs((Error)value, user));
                    return Request.CreateResponse(HttpStatusCode.NotImplemented, "Trying to configure not implemented RSWare message ID");
            }
        }
    }
}
