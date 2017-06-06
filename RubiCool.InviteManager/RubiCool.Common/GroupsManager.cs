using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using RubiCool.Common.Model;
using RubiCool.Common.Utils;

namespace RubiCool.Common
{
    public static class GroupsManager
    {
        private static readonly GraphClient Client;

        static GroupsManager()
        {
            Client = AuthenticationProvider.GetGraphClient();
        }

        public static async Task<GraphGroup> GetGroupByAccountIdAsync(string accountId)
        {
            var uriEndPoint =
                $"{Constants.ResourceUrl}/{Constants.GraphApiVersion}/groups?$filter=startswith(mailNickname, '{accountId}')";

            // Retrieve possible groups from Graph API
            var groups = await Client.GetData<GraphFilteredGroups>(HttpMethod.Get, Uri.EscapeUriString(uriEndPoint))
                .ConfigureAwait(false);

            // If request failed return nothing and log error
            if (!groups.IsSuccessfull)
            {
                throw new Exception($"Error: unable to retrieve group. API error {groups.Message}");
            }

            // Check if there is any group returned by the Graph API
            if (groups.Data.Groups.Any())
            {
                return groups.Data.Groups.First();
            }

            return null;
        }

        public static async Task<GraphGroup> CreateGroup(GroupRequest request)
        {
            var uriEndPoint = $"{Constants.ResourceUrl}/{Constants.GraphApiVersion}/groups";

            GraphResponse<GraphGroup> result = null;
            var group = new GraphGroup()
            {
                Description = request.AccountId,
                DisplayName = request.AccountName,
                GroupTypes = new List<string>() {"Unified"},
                MailEnabled = true,
                MailNickname = request.AccountId,
                SecurityEnabled = false
            };

            // Create group with Graph API
            result = await Client.GetData<GraphGroup>(HttpMethod.Post, uriEndPoint, group).ConfigureAwait(false);

            if (!result.IsSuccessfull)
            {
                throw new Exception($"Error: group not created - API error: {result.Message}");
            }

            return result.Data;
        }
    }
}
