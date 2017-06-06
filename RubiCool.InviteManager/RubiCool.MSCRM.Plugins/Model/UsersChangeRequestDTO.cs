using System;
using System.Runtime.Serialization;

namespace RubiCool.MSCRM.Plugins.Model
{
    [Serializable]
    [DataContract]
    public class UserChangeRequestDTO
    {
        [DataMember]
        public string EmailAddress { get; set; }
        [DataMember]
        public string RequestType { get; set; }
    }

    [DataContract]
    public enum RequestType
    {
        Block,
        Unblock,
    }
}
