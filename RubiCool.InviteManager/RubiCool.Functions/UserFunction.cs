using System;
using RubiCool.Common.Model;
using RubiCool.Common.Utils;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using RubiCool.Common;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace RubiCool.Functions
{
    public static class UserFunction
    {
        [FunctionName("Users")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("User function processed a request.");

            try
            {
                // Deserialize request object to model
                var request = await req.RetrieveRequestObject<UsersRequest>().ConfigureAwait(false);

                log.Info($"Check if user {request.EmailAddress} exists in Azure AD.");

                // Check if user exists in Azure AD
                var user = await UserManager.GetUserByMailAddress(request.EmailAddress).ConfigureAwait(false);
                if (user == null)
                {
                    log.Warning($"User {request.EmailAddress} doesn't exist in Azure AD.");
                    return req.CreateErrorResponse(HttpStatusCode.BadRequest, "User doesn't exists in directory.");
                }

                // Check for actions
                switch (request.RequestType)
                {
                    case ActionType.Block:
                        log.Info($"Disabling user {request.EmailAddress}");
                        user = await UserManager.DisableUser(user);
                        break;
                    case ActionType.Unblock:
                        log.Info($"Enabling user {request.EmailAddress}");
                        user = await UserManager.EnableUser(user);
                        break;
                }

                log.Info("User function executed succesfully");

                return req.CreateResponse(HttpStatusCode.Created, user);
            }
            catch (Exception e)
            {
                log.Error($"Unexpected error {e.Message}");
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }
    }
}