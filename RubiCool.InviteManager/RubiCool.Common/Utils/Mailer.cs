using System.Net;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;

namespace RubiCool.Common.Utils
{
    public static class Mailer
    {
        public static async Task<bool> SendInvitationEmailAsync(string userEmailAddress, string redeemUrl)
        {
            var mailClient = new SendGrid.SendGridClient(Constants.SendGridAPIKey);
            
            var message = new SendGridMessage()
            {
                From = new EmailAddress(Constants.SendGridNoReploy),
                TemplateId = Constants.SendGridTemplateId,
                Subject = Constants.SendGridSubject,
            };
            message.AddTo(new EmailAddress(userEmailAddress));
            message.AddSubstitution("%invitationRedeemUrl%", redeemUrl);

            // Send e-mail
            var sendGridWebTransport = await mailClient.SendEmailAsync(message);

            if (sendGridWebTransport.StatusCode == HttpStatusCode.Accepted)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
