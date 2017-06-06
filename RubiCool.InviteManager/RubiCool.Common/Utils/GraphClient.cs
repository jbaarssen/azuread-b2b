using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using RubiCool.Common.Model;

namespace RubiCool.Common.Utils
{
    public class GraphClient
    {
        private const string API_INVITATION_PATH = "/beta/invitations";

        private object _syncLock = new Object();
        private string _accessToken;
        private Func<Task<string>> _accessTokenGetter;
        private Func<Task> _accessTokenSetter;
        private Uri _serviceRoot;

        public GraphClient(Uri serviceRoot, Func<Task<string>> accessTokenGetter)
        {
            _accessTokenGetter = accessTokenGetter;
            _accessTokenSetter = this.SetToken;
            _serviceRoot = serviceRoot;
        }

        private async Task SetToken()
        {
            var accessToken = await _accessTokenGetter();
            lock (_syncLock)
            {
                _accessToken = accessToken;
            }
        }
        
        public async Task<GraphResponse<T>> GetData<T>(HttpMethod type, string uri, dynamic postContent = null)
        {
            var result = new GraphResponse<T>();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept
                    .Add(new MediaTypeWithQualityHeaderValue("*/*"));
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", await _accessTokenGetter());

                HttpResponseMessage response = null;
                if (type == HttpMethod.Get)
                {
                    response = await httpClient.GetAsync(uri).ConfigureAwait(false);
                }
                else if (type == HttpMethod.Post)
                {
                    var content = new StringContent(JsonHelper.Serialize(postContent), Encoding.UTF8, "application/json");

                    response = await httpClient.PostAsync(uri, content).ConfigureAwait(false);
                } 
                else if (type.Method == "PATCH")
                {
                    using (var httpRequestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), uri))
                    {
                        httpRequestMessage.Content = new StringContent(JsonHelper.Serialize(postContent), Encoding.UTF8, "application/json");

                        response = await httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
                    }
                }

                if (response != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    // Update result information
                    result.StatusCode = response.StatusCode;
                    result.Message = response.ReasonPhrase;
                    
                    if (response.IsSuccessStatusCode)
                    {
                        result.Data = JsonConvert.DeserializeObject<T>(responseContent);
                    }
                    else
                    {
                        var error = JsonConvert.DeserializeObject<GraphError>(responseContent);

                        result.IsSuccessfull = false;
                        var reason = response.ReasonPhrase;
                        var errorMessage = (error.Error == null) ? "N/A" : error.Error.Message;
                        result.Message = $"Server response: {reason}. Server detail: {errorMessage}";
                        return result;
                    }
                }
            }
            return result;
        }
    }
}
