using System;
using System.Runtime.Serialization;

namespace RubiCool.MSCRM.Plugins.Model
{
    [Serializable]
    [DataContract]
    public class GroupRequestDTO
    {
        [DataMember]
        public string AccountId { get; set; }
        [DataMember]
        public string AccountName { get; set; }
    }
}
