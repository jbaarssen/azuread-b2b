using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RubiCool.Common.Utils
{
    public class JsonHelper
    {
        public static string Serialize(object jsonData)
        {
            JsonConvert.DefaultSettings = () =>
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                return settings;
            };

            return JsonConvert.SerializeObject(jsonData);
        }

    }
}
