using Newtonsoft.Json;

namespace RubiCool.Common.Model
{
    public class GraphAddMember
    {
        private string _id;

        [JsonProperty("@odata.id")]
        public string Id
        {
            get => _id;
            set => _id = $"https://graph.microsoft.com/v1.0/directoryObjects/{value}";
        }

        public GraphAddMember(string userId)
        {
            Id = userId;
        }
    }
}
