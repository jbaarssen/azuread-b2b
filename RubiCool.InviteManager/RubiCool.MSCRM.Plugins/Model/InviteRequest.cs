using System;
using System.Runtime.Serialization;

namespace RubiCool.MSCRM.Plugins.Model
{
    [Serializable]
    [DataContract]
    public class InviteRequestDTO
    {
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string EmailAddress { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public DateTime RequestDate { get; set; }
        [DataMember]
        public string AccountId { get; set; }
        [DataMember]
        public string AccountName { get; set; }
    }
}
