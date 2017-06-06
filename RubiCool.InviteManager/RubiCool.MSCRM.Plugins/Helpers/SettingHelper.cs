using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Xrm.Sdk;

namespace RubiCool.MSCRM.Plugins.Helpers
{
    public static class SettingHelper
    {
        private static Dictionary<string, string> SecureConfig { get; set; }

        public static class SecureConfigKeys
        {
            public const string InviteFunctionKey = "InviteFunctionKey";
            public const string InviteEndPoint = "InviteEndPoint";
            public const string GroupFunctionKey = "GroupFunctionKey";
            public const string GroupEndPoint = "GroupEndPoint";
            public const string UsersFunctionKey = "UsersFunctionKey";
            public const string UsersEndPoint = "UsersEndPoint";
        }

        public static void InitSecureConfig(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                try
                {
                    var doc = XElement.Parse(content);
                    SecureConfig = doc.Descendants().Select(x => new { x.Name, x.Value }).ToDictionary(x => x.Name.LocalName, x => x.Value);
                }
                catch (Exception)
                {
                    // Throwing an exception here will kill the plugin
                }
            }
        }

        public static string RetrieveSetting(ITracingService trace, string key)
        {
            trace.Trace($"Setting {key} from Secure Config");
            return SecureConfig.ContainsKey(key) ? SecureConfig[key] : default(string);
        }
    }
}
