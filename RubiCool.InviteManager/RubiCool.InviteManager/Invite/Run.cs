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
    public class InviteFunction
    {
        public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
        {
            log.Info("SendInvite http trigger started.");

            GraphInvitation result = null;
            try
            {
                // Deserialize request object to model
                var request = await req.RetrieveRequestObject<GuestRequest>().ConfigureAwait(false);

                // Check if user exists in Azure AD
                var user = await UserManager.GetUserByMailAddress(request.EmailAddress).ConfigureAwait(false);
                if (user != null)
                {
                    return req.CreateErrorResponse(HttpStatusCode.BadRequest, "User already exists in directory.");
                }

                // Get group by account id
                var group = await GroupsManager.GetGroupByAccountIdAsync(request.AccountId).ConfigureAwait(false);

                // Check if the group is found
                if (group != null)
                {
                    // Send invitation to user
                    result = await InvitationManager
                        .SendInvitationAsync(request,
                            $"https://rubichill.sharepoint.com/sites/{request.AccountId}",
                            group.Id)
                        .ConfigureAwait(false);
                }
                else
                {
                    return req.CreateErrorResponse(HttpStatusCode.BadRequest, "Azure AD group not found.");
                }

                return req.CreateResponse(HttpStatusCode.Created, result);
            }
            catch (Exception e)
            {
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }
    }
}