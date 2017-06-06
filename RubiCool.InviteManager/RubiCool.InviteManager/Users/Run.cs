using System;
using Microsoft.Azure.WebJobs.Host;
using RubiCool.Common.Model;
using RubiCool.Common.Utils;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RubiCool.Common;

namespace RubiCool.InviteManager
{
    public class UsersFunction
    {
        public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
        {
            log.Info("UsersFunction http trigger started.");
            
            try
            {
                // Deserialize request object to model
                var request = await req.RetrieveRequestObject<UsersRequest>().ConfigureAwait(false);

                // Check if user exists in Azure AD
                var user = await UserManager.GetUserByMailAddress(request.EmailAddress).ConfigureAwait(false);
                if (user == null)
                {
                    return req.CreateErrorResponse(HttpStatusCode.BadRequest, "User doesn't exists in directory.");
                }

                // Check for actions
                switch (request.RequestType)
                {
                    case ActionType.Block:
                        user = await UserManager.DisableUser(user);
                        break;
                    case ActionType.Unblock:
                        user = await UserManager.EnableUser(user);
                        break;
                }
                
                return req.CreateResponse(HttpStatusCode.Created, user);
            }
            catch (Exception e)
            {
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }
    }
}