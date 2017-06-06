using System;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using Microsoft.Xrm.Sdk;
using RubiCool.MSCRM.Plugins.Model;

namespace RubiCool.MSCRM.Plugins.Helpers
{
    public static class ContactPluginContextHelper
    {
        public static void SendInvitation(IOrganizationService service, ITracingService trace, Entity entity,
            Entity image)
        {
            try
            {
                // Check if the entity contains status and email attributes
                if (!image.Contains(CrmContext.Contact.EmailAddress1) ||
                    !image.Contains(CrmContext.Base.StateCode))
                    return;

                trace.Trace("Start processing contact invitation");

                var status = entity.GetAttributeValue<OptionSetValue>(CrmContext.Base.StateCode);

                // Check if status is active
                if (status.Value == CrmContext.StateCodes.Active)
                {
                    trace.Trace("Status of contact is active");

                    trace.Trace("Retrieve contact information");
                    var emailAddress = image.GetAttributeValue<string>(CrmContext.Contact.EmailAddress1);
                    var firstName = image.GetAttributeValue<string>(CrmContext.Contact.FirstName);
                    var lastName = image.GetAttributeValue<string>(CrmContext.Contact.LastName);
                    var accountId = image.GetAttributeValue<EntityReference>(CrmContext.Contact.AccountId);

                    if (accountId == null || accountId.Id == Guid.Empty)
                    { 
                        trace.Trace("No account information found for this contact.");
                        return;  
                    }

                    var account = AccountHelper.RetrieveAccount(service, trace, accountId);
                    var accountName = account.GetAttributeValue<string>(CrmContext.Account.NameNl);

                    trace.Trace("Sending invite to the API");

                    // Create invite
                    var invite = new InviteRequestDTO
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        EmailAddress = emailAddress,
                        RequestDate = DateTime.Now,
                        AccountId = accountId.Id.ToString(),
                        AccountName = accountName
                    };

                    SendInvitationRequest(service, trace, invite);
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

        public static void UpdateContact(IOrganizationService service, ITracingService trace, Entity entity,
            Entity image)
        {
            try
            {
                // Check if the entity contains status and email attributes
                if (!image.Contains(CrmContext.Contact.EmailAddress1) ||
                    !image.Contains(CrmContext.Base.StateCode))
                    return;

                trace.Trace("Start processing update on contact information");

                var newStatus = entity.GetAttributeValue<OptionSetValue>(CrmContext.Base.StateCode);
                var oldStatus = image.GetAttributeValue<OptionSetValue>(CrmContext.Base.StateCode);
                
                trace.Trace($"Old status: {oldStatus.Value}");
                trace.Trace($"New status: {newStatus.Value}");

                // Check for change in status
                if (!oldStatus.Value.Equals(newStatus.Value))
                {
                    trace.Trace("The account status has been changed");

                    var emailAddress = image.GetAttributeValue<string>(CrmContext.Contact.EmailAddress1);

                    // Create user change request
                    var changeRequest = new UserChangeRequestDTO()
                    {
                        EmailAddress = emailAddress,
                        RequestType = newStatus.Value == CrmContext.StateCodes.Active ? "Unblock" : "Block"
                    };

                   // Send user request to Azure Function API
                    UpdateUser(service, trace, changeRequest);
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

        private static void SendInvitationRequest(IOrganizationService service, ITracingService trace, InviteRequestDTO request)
        {
            //Get the application settings
            var xFunctionKey = SettingHelper.RetrieveSetting(trace, SettingHelper.SecureConfigKeys.InviteFunctionKey);
            var xFunctionEndPoint = SettingHelper.RetrieveSetting(trace, SettingHelper.SecureConfigKeys.InviteEndPoint);

            trace.Trace($"Sending request to Invite Azure Function: {xFunctionEndPoint}");
            trace.Trace($"Function key - {xFunctionKey}");

            // Call mail API via APIM.
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("x-functions-key", xFunctionKey);

                var content = new StringContent(GlobalHelper.JsonSerializer<InviteRequestDTO>(request), Encoding.UTF8, "application/json");

                var response = httpClient.PostAsync(
                    xFunctionEndPoint, content).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    trace.Trace($"Success: invitation request is successfully send.");
                }
                else
                {
                    var errorContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    trace.Trace($"Error: invitation request failed - {errorContent}");
                }
            }

        }

        private static void UpdateUser(IOrganizationService service, ITracingService trace, UserChangeRequestDTO request)
        {
            //Get the application settings
            var xFunctionKey = SettingHelper.RetrieveSetting(trace, SettingHelper.SecureConfigKeys.UsersFunctionKey);
            var xFunctionEndPoint = SettingHelper.RetrieveSetting(trace, SettingHelper.SecureConfigKeys.UsersEndPoint);

            trace.Trace($"Sending request to users Azure Function: {xFunctionEndPoint}");
            trace.Trace($"Function key - {xFunctionKey}");

            // Call mail API via APIM.
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("x-functions-key", xFunctionKey);

                trace.Trace($"Content: {GlobalHelper.JsonSerializer<UserChangeRequestDTO>(request)}");

                var content = new StringContent(GlobalHelper.JsonSerializer<UserChangeRequestDTO>(request), Encoding.UTF8, "application/json");

                var response = httpClient.PostAsync(
                    xFunctionEndPoint, content).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    trace.Trace($"Success: user change request is successfully send.");
                }
                else
                {
                    var errorContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    trace.Trace($"Error: user change request failed - {errorContent}");
                }
            }

        }
    }
}
