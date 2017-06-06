using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RubiCool.Common.Model
{
    public class GraphInvitation
    {
        [JsonProperty(PropertyName = "@odata.context")]
        public string Context { get; set; }
        public string InviteRedeemUrl { get; set; }
        public string InvitedUserDisplayName { get; set; }
        public string InvitedUserType { get; set; }
        public string InvitedUserEmailAddress { get; set; }
        public bool SendInvitationMessage { get; set; }
        public InvitedUserMessageInfo InvitedUserMessageInfo { get; set; }
        public string InviteRedirectUrl { get; set; }
        public string Status { get; set; }
        public InvitedUser InvitedUser { get; set; }
    }

    public class InvitedUser
    {
        public string Id { get; set; }
    }
    public class EmailAddress
    {
        public string Name { get; set; }       
        public string Address { get; set; }
    }
    
    public class CcRecipient
    {
        public EmailAddress EmailAddress { get; set; }
        public string Email { get; set; }
        public string Alias { get; set; }
        public string ObjectId { get; set; }
        public string PermissionIdentityType { get; set; }
    }

    public class InvitedUserMessageInfo
    {
        public string MessageLanguage { get; set; }
        public List<CcRecipient> CcRecipients { get; set; }
        public string CustomizedMessageBody { get; set; }
    }
}
