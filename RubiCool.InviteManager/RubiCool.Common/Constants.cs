using System.Configuration;

namespace RubiCool.Common
{
    public static class Constants
    {
        public static string TenantId = Config.GetKeyVaultSecret("ActiveDirectoryTenantId");
        public static string TenantName = ConfigurationManager.AppSettings["ActiveDirectoryTenant"];
        public static string ClientId = Config.GetKeyVaultSecret("IdaClientId");
        public static string ClientSecret = Config.GetKeyVaultSecret("IdaClientSecret");
        public static string AuthString = $"https://login.microsoftonline.com/{TenantId}";
        public const string ResourceUrl = "https://graph.microsoft.com";
        public const string GraphServiceObjectId = "00000002-0000-0000-c000-000000000000";
        public const string GraphApiVersion = "v1.0";
        public const string GraphApiBetaVersion = "beta";
        public static string SendGridAPIKey = Config.GetKeyVaultSecret("SendGridAPIKey");
        public const string SendGridTemplateId = "8b895add-ad5a-4dce-8475-997af0bbaf5e";
        public const string SendGridSubject = "You are invited to the RubiChill portal";
        public const string SendGridNoReploy = "no-reply@rubichill.com";

    }
}
