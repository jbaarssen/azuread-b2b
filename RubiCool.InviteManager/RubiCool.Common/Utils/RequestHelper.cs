using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RubiCool.Common.Utils
{
    public static class RequestHelper
    {
        public static async Task<T> RetrieveRequestObject<T>(this HttpRequestMessage req)
        {
            var result = await req.Content.ReadAsStringAsync();
            return result != null ? JsonConvert.DeserializeObject<T>(result) : default(T);
        }
    }
}
