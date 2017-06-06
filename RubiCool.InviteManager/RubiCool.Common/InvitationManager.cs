using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using RubiCool.Common.Model;
using RubiCool.Common.Utils;

namespace RubiCool.Common
{
    public static class InvitationManager
    {
        private static readonly GraphClient Client;

        static InvitationManager()
        {
            Client = AuthenticationProvider.GetGraphClient();
        }

        public static async Task<GraphInvitation> SendInvitationAsync(GuestRequest request, string groupUrl, string groupId)
        {
            var displayName = $"{request.FirstName} {request.LastName}";
            var memberType = GraphMemberType.Guest;

            GraphResponse<GraphInvitation> response = null;
            // Setup invitation
            var inviteEndPoint = $"{Constants.ResourceUrl}/{Constants.GraphApiBetaVersion}/invitations";
            var invitation = new GraphInvitation()
            {
                InvitedUserDisplayName = displayName,
                InvitedUserEmailAddress = request.EmailAddress,
                InviteRedirectUrl = groupUrl,
                SendInvitationMessage = false,
                InvitedUserType = memberType.ToString()
            };

            // Invite user via Graph API
            response = await Client.GetData<GraphInvitation>(HttpMethod.Post, inviteEndPoint, invitation)
                .ConfigureAwait(false);

            if (!response.IsSuccessfull)
            {
                throw new Exception($"Error: invite not sent - API error: {response.Message}");
            }

            // Add user to group
            var groupAdded = await AddUserToGroupAsync(response.Data.InvitedUser.Id,
                groupId).ConfigureAwait(false);

            if (!groupAdded)
            {
                throw new Exception($"Error: could not add user {response.Data.InvitedUserEmailAddress} to group {groupId}");   
            }

            // Send email to user
            if (!await Mailer.SendInvitationEmailAsync(request.EmailAddress, response.Data.InviteRedeemUrl))
            {
                throw new Exception($"Error: invite not send to {request.EmailAddress}.");
            }

            return response.Data;
        }

        private static async Task<bool> AddUserToGroupAsync(string userId, string groupId)
        {
            var body = new GraphAddMember(userId);
            
            var uriEndPoint = $"{Constants.ResourceUrl}/{Constants.GraphApiVersion}/groups/{groupId}/members/$ref";
            var response = await Client.GetData<string>(HttpMethod.Post, uriEndPoint, body).ConfigureAwait(false);

            if (!response.IsSuccessfull)
            {
                throw new Exception($"Error: Adding user to group failed - API erro: {response.Message}");
            }

            return response.IsSuccessfull;
        }
    }
}
