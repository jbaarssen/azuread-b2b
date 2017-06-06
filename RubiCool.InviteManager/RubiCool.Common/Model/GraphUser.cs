using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RubiCool.Common.Model
{
    public class GraphFilteredUsers
    {
        [JsonProperty("value")]
        public IList<GraphUser> Users { get; set; }

        public GraphFilteredUsers()
        {
            Users = new List<GraphUser>();
        }
    }
    public class GraphUser
    {
        public string Id { get; set; }
        public DateTime? DeletedDateTime { get; set; }
        public bool AccountEnabled { get; set; }
        public string CompanyName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Department { get; set; }
        public string DisplayName { get; set; }
        public string GivenName { get; set; }
        public string JobTitle { get; set; }
        public string Mail { get; set; }
        public string MailNickname { get; set; }
        public string MobilePhone { get; set; }
        public string OfficeLocation { get; set; }
        public string PostalCode { get; set; }
        public string State { get; set; }
        public string StreetAddress { get; set; }
        public string SurName { get; set; }
        public string UsageLocation { get; set; }
        public string UserPrincipalName { get; set; }
        public string UserType { get; set; }
    }
}
