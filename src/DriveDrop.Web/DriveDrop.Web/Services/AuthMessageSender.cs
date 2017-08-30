using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace DriveDrop.Web.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {

            // email = "richardrcruzc@gmail.com";
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("DriveDrop", "info@DriveDrop.com"));
            emailMessage.To.Add(new MailboxAddress(email, email));
            emailMessage.Subject = subject;
            // emailMessage.Body = new TextPart("plain") { Text = message };
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = message;
            emailMessage.Body = bodyBuilder.ToMessageBody();



            try
            {
                using (var client = new SmtpClient())
                {

                    client.LocalDomain = "smtp.sendgrid.net";

                    await client.ConnectAsync("smtp.sendgrid.net", 587, SecureSocketOptions.None).ConfigureAwait(false);
                    await client.AuthenticateAsync("azure_d5dbc42617b3c0dbb2559dc0342b495e@azure.com", "Q!w2e3r4");
                    await client.SendAsync(emailMessage).ConfigureAwait(false);
                    await client.DisconnectAsync(true).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {

                var m = ex.Message;
            }

            // return Task.FromResult(0);
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
