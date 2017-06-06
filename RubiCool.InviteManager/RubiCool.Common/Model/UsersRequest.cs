using System;

namespace RubiCool.Common.Model
{
    public class UsersRequest
    {
        public string EmailAddress { get; set; }
        public ActionType RequestType { get; set; }
    }

    public enum ActionType
    {
        Block,
        Unblock
    }
}
