using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace RubiCool.Common
{
    public static class Config
    {
        public static string GetKeyVaultSecret(string secretNode)
        {
            var keyVaultUri = ConfigurationManager.AppSettings["KeyVaultUri"];
            var secretUri = $"{keyVaultUri}{secretNode}";

            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken));
            return keyVaultClient.GetSecretAsync(secretUri).Result.Value;
        }

        private static async Task<string> GetAccessToken(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, GetCert());

            if (result == null)
                throw new InvalidOperationException("Failed to obtain the JWT token");

            return result.AccessToken;
        }

        private static ClientAssertionCertificate GetCert()
        {
            var certificateThumbPrint = ConfigurationManager.AppSettings["SecretsCertificateThumbPrint"];
            var servicePrincipalId = ConfigurationManager.AppSettings["ServicePrincipalId"];

            var clientAssertionCertPfx = FindCertificateByThumbprint(certificateThumbPrint);
            // the left-hand GUID here is the output of $adapp.ApplicationId in our Service Principal setup script
            return new ClientAssertionCertificate(servicePrincipalId, clientAssertionCertPfx);
        }

        private static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, System.EnvironmentVariableTarget.Process);
        }

        private static X509Certificate2 FindCertificateByThumbprint(string findValue)
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            try
            {
                store.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection col = store.Certificates.Find(X509FindType.FindByThumbprint, findValue, false);
                if (col == null || col.Count == 0)
                {
                    return null;
                }
                return col[0];
            }
            finally
            {
                store.Close();
            }
        }
    }
}
