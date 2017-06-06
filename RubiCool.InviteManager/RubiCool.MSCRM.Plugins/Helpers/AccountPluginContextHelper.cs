using System;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using Microsoft.Xrm.Sdk;
using RubiCool.MSCRM.Plugins.Model;

namespace RubiCool.MSCRM.Plugins.Helpers
{
    public static class AccountPluginContextHelper
    {
        public static void CreateGroup(IOrganizationService service, ITracingService trace, Entity entity,
            Entity image)
        {
            try
            {
                // Check if the entity contains status and email attributes
                if (!image.Contains(CrmContext.Account.NameNl) ||
                    !image.Contains(CrmContext.Base.StateCode))
                    return;

                trace.Trace("Start processing group creation");

                var status = entity.GetAttributeValue<OptionSetValue>(CrmContext.Base.StateCode);

                // Check if status is active
                if (status.Value == CrmContext.StateCodes.Active)
                {
                    trace.Trace("Status of account is active");

                    trace.Trace("Retrieve account information");
                    var accountId = image.GetAttributeValue<Guid>(CrmContext.Account.Id);
                    var accountName = image.GetAttributeValue<string>(CrmContext.Account.NameNl);
                    
                    trace.Trace("Sending group request to the API");

                    // Create invite
                    var groupRequest = new GroupRequestDTO
                    {
                        AccountId = accountId.ToString(),
                        AccountName = accountName
                    };

                    SendGroupRequest(service, trace, groupRequest);
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new InvalidPluginExecutionException(
                    $"An error occurrd in the organizationservice. {ex.Detail.Message}");
            }
            catch (TimeoutException ex)
            {
                throw new InvalidPluginExecutionException($"A timeout occurred. {ex.Message}");
            }
            catch (FaultException ex)
            {
                throw new InvalidPluginExecutionException($"An error occurrd. {ex.Message}");
            }
        }

        private static void SendGroupRequest(IOrganizationService service, ITracingService trace, GroupRequestDTO request)
        {
            //Get the application settings
            var xFunctionKey = SettingHelper.RetrieveSetting(trace, SettingHelper.SecureConfigKeys.GroupFunctionKey);
            var xFunctionEndPoint = SettingHelper.RetrieveSetting(trace, SettingHelper.SecureConfigKeys.GroupEndPoint);

            trace.Trace($"Sending request to Invite Azure Function: {xFunctionEndPoint}");

            // Call mail API via APIM.
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("x-functions-key", xFunctionKey);

                var content = new StringContent(GlobalHelper.JsonSerializer<GroupRequestDTO>(request), Encoding.UTF8, "application/json");

                var response = httpClient.PostAsync(
                    xFunctionEndPoint, content).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    trace.Trace($"Success: group request is successfully send.");
                }
                else
                {
                    var errorContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    trace.Trace($"Error: group request failed - {errorContent}");
                }
            }

        }
    }
}
