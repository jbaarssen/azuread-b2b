using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace RubiCool.MSCRM.Plugins.Helpers
{
    public static class AccountHelper
    {
        public static Entity RetrieveAccount(IOrganizationService service, ITracingService tracing, EntityReference accountId)
        {
            return service.Retrieve(CrmContext.EntityNames.Account, accountId.Id,
                new ColumnSet(CrmContext.Account.NameNl));
        }
    }
}
