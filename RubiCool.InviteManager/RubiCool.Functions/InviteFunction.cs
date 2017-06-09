using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using RubiCool.Common;
using RubiCool.Common.Model;
using RubiCool.Common.Utils;
using System;

namespace RubiCool.Functions
{
    public static class InviteFunction
    {
        [FunctionName("Invite")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Invite function processed a request.");

            GraphInvitation result = null;
            try
            {
                // Deserialize request object to model
                var request = await req.RetrieveRequestObject<GuestRequest>().ConfigureAwait(false);

                log.Info($"Check if user {request.EmailAddress} already exists in Azure AD");

                // Check if user exists in Azure AD
                var user = await UserManager.GetUserByMailAddress(request.EmailAddress).ConfigureAwait(false);
                if (user != null)
                {
                    log.Warning($"User {request.EmailAddress} exists in Azure AD.");
                    return req.CreateErrorResponse(HttpStatusCode.BadRequest, "User already exists in directory.");
                }

                // Get group by account id
                log.Info($"Check if group {request.AccountName} exists in Azure AD");
                var group = await GroupsManager.GetGroupByAccountIdAsync(request.AccountId).ConfigureAwait(false);

                // Check if the group is found
                if (group != null)
                {
                    // Send invitation to user
                    result = await InvitationManager
                        .SendInvitationAsync(request,
                            $"https://rubichillsps.sharepoint.com/sites/{request.AccountId}",
                            group.Id)
                        .ConfigureAwait(false);
                }
                else
                {
                    log.Warning($"Azure AD group {request.AccountName} not found.");
                    return req.CreateErrorResponse(HttpStatusCode.BadRequest, "Azure AD group not found.");
                }

                log.Info("Invite function executed succesfully");

                return req.CreateResponse(HttpStatusCode.Created, result);
            }
            catch (Exception e)
            {
                log.Error($"Unexpected error {e.Message}");
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }
    }
}