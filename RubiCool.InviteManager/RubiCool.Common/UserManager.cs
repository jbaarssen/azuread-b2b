using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using RubiCool.Common.Model;
using RubiCool.Common.Utils;

namespace RubiCool.Common
{
    public static class UserManager
    {
        private static readonly GraphClient Client;

        static UserManager()
        {
            Client = AuthenticationProvider.GetGraphClient();
        }

        public static async Task<GraphUser> GetUserByMailAddress(string mailAddress)
        {
            var uriEndPoint =
                $"{Constants.ResourceUrl}/{Constants.GraphApiVersion}/users?$filter=mail eq '{mailAddress}' and userType eq 'Guest'";

            // Retrieve possible users from Graph API
            var users = await Client.GetData<GraphFilteredUsers>(HttpMethod.Get, Uri.EscapeUriString(uriEndPoint))
                .ConfigureAwait(false);

            // If request failed return nothing and log error
            if (!users.IsSuccessfull)
            {
                throw new Exception($"Error: unable to retrieve user. API error {users.Message}");
            }

            // Check if there is any user returned by the Graph API
            if (users.Data.Users.Any())
            {
                return users.Data.Users.First();
            }

            return null;
        }

        public static async Task<GraphUser> DisableUser(GraphUser user)
        {
            var uriEndPoint =
                $"{Constants.ResourceUrl}/{Constants.GraphApiVersion}/users/{user.Id}";

            // Set account on disabled
            user.AccountEnabled = false;

            // Patch user account
            var result = await Client.GetData<GraphUser>(new HttpMethod("PATCH"), uriEndPoint, user);

            // If request failed throw exception
            if (!result.IsSuccessfull)
            {
                throw new Exception($"Error: unable to disable user account. API error {result.Message}");
            }

            return user;
        }

        public static async Task<GraphUser> EnableUser(GraphUser user)
        {
            var uriEndPoint =
                $"{Constants.ResourceUrl}/{Constants.GraphApiVersion}/users/{user.Id}";

            // Set account on enabled
            user.AccountEnabled = true;

            // Patch user account
            var result = await Client.GetData<GraphUser>(new HttpMethod("PATCH"), uriEndPoint, user);

            // If request failed throw exception
            if (!result.IsSuccessfull)
            {
                throw new Exception($"Error: unable to enable user account. API error {result.Message}");
            }

            return user;
        }
    }
}
