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
    public class GroupsFunction
    {
        public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
        {
            log.Info("CreateGroup http trigger started.");

            try
            {
                // Deserialize request object to model
                var request = await req.RetrieveRequestObject<GroupRequest>().ConfigureAwait(false);

                // Create group
                var response = await GroupsManager.CreateGroup(request);
                
                return req.CreateResponse(HttpStatusCode.Created, response);
            }
            catch (Exception e)
            {
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }
    }
}