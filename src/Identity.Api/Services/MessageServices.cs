using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace Identity.Api.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IHostingEnvironment _env;
        public   AuthMessageSender(IHostingEnvironment env, IOptionsSnapshot<AppSettings> settings) {
            _env = env;
            _settings = settings;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {

           // email = "richardrcruzc@gmail.com";
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(_settings.Value.EmailSenderName,_settings.Value.EmailSenderEmail));
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

                    await client.ConnectAsync(_settings.Value.EmailLocalDomain,_settings.Value.EmailLocalPort, SecureSocketOptions.None).ConfigureAwait(false);
                    await client.AuthenticateAsync(_settings.Value.EmailUser,_settings.Value.EmailPassword);
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
