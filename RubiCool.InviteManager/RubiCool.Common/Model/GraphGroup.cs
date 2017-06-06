using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RubiCool.Common.Model
{
    public class GraphFilteredGroups
    {
        [JsonProperty("value")]
        public IList<GraphGroup> Groups { get; set; }

        public GraphFilteredGroups()
        {
            Groups = new List<GraphGroup>();
        }
    }

    public class GraphGroup
    {
        [JsonProperty("@odata.context")]
        public string Context { get; set; }
        public string Id { get; set; }
        public DateTime? DeletedDateTime { get; set; }
        public string Classification { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public List<string> GroupTypes { get; set; }
        public string Mail { get; set; }
        public bool MailEnabled { get; set; }
        public string MailNickname { get; set; }
        public List<string> ProxyAddresses { get; set; }
        public DateTime? RenewedDateTime { get; set; }
        public bool SecurityEnabled { get; set; }
        public string Visibility { get; set; }
    }
}
