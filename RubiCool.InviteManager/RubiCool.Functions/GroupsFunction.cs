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
    public static class GroupsFunction
    {
        [FunctionName("Groups")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Group function processed a request.");

            try
            {
                // Deserialize request object to model
                var request = await req.RetrieveRequestObject<GroupRequest>().ConfigureAwait(false);

                log.Info($"Create group {request.AccountName}");

                // Create group
                var response = await GroupsManager.CreateGroup(request);

                log.Info("Invite function executed succesfully");

                return req.CreateResponse(HttpStatusCode.Created, response);
            }
            catch (Exception e)
            {
                log.Error($"Unexpected error {e.Message}");
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }
    }
}