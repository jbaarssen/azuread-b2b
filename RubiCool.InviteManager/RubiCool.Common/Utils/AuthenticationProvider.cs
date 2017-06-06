using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Threading.Tasks;

namespace RubiCool.Common.Utils
{
    public class AuthenticationProvider
    { 
        public static string Token;

        public static GraphClient GetGraphClient()
        {
            var servicePointUri = new Uri(Constants.ResourceUrl);
            var serviceRoot = new Uri(servicePointUri, Constants.TenantId);

            var graphClient = new GraphClient(
                serviceRoot, async () => await AcquireTokenAsync());

            return graphClient;
        }

        public static async Task<string> AcquireTokenAsync()
        {
            if(Token == null)
            {
                var authenticationContext = new AuthenticationContext(Constants.AuthString, false);

                // Config for OAuth client credentials
                var credentials = new ClientCredential(Constants.ClientId, Constants.ClientSecret);
                var authenticationResult = await authenticationContext.AcquireTokenAsync(Constants.ResourceUrl, credentials);

                Token = authenticationResult.AccessToken;
            }
            return Token;
        }
    }
}
