using System;

namespace RubiCool.Common.Model
{
    public class GuestRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Status { get; set; }
        public DateTime RequestDate { get; set; }
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public bool AccountEnabled { get; set; }
    }
}
