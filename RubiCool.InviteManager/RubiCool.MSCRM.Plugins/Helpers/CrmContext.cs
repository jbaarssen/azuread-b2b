using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiCool.MSCRM.Plugins.Helpers
{
    public static class CrmContext
    {
        public static class EntityNames
        {
            public const string Account = "account";
            public const string Contact = "contact";
        }

        public static class StateCodes
        {
            public const int Active = 0;
            public const int Inactive = 1;
        }

        public static class Account
        {
            public const string City = "city";
            public const string Email1 = "emailaddress1";
            public const string Fax = "fax";
            public const string Id = "accountid";
            public const string NameNl = "name";
            public const string PostalCode = "address1_postalcode";
            public const string TelephoneNo = "telephone1";
            public const string Web = "websiteurl";
        }

        public static class Base
        {
            public const string Createdon = "createdon";
            public const string StateCode = "statecode";
            public const string Owner = "ownerid";
        }

        public static class Contact
        {
            public const string Id = "contactid";
            public const string BirthDate = "birthdate";
            public const string City = "address1_city";
            public const string FullName = "fullname";
            public const string Address1Line1 = "address1_line1";
            public const string Address1Country = "address1_country";
            public const string ParentCustomer = "parentcustomerid";
            public const string PostalCode = "address1_postalcode";
            public const string FirstName = "firstname";
            public const string LastName = "lastname";
            public const string Telephone1 = "telephone1";
            public const string Telephone2 = "telephone2";
            public const string EmailAddress1 = "emailaddress1";
            public const string MobilePhone = "mobilephone";
            public const string AccountId = "parentcustomerid";

        }
        
    }
}
