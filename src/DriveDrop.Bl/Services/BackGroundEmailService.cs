using DriveDrop.Bl.Data;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;

namespace DriveDrop.Bl.Services
{
    public class BackGroundEmailService: IBackGroundEmailService
    {
        private readonly AppSettings _settings;

        private readonly IHostingEnvironment _env;

        private readonly DriveDropContext _context;
        public BackGroundEmailService(DriveDropContext context, IHostingEnvironment env, IOptions<AppSettings> settings)
        {
            _context = context;
            _env = env;
            _settings = settings.Value;
        }

        public async Task SendBatchEmailFromQueueAsync()
        {
            var maxTries = 3;
            var toSends = _context.QueuedEmails.Where(x => x.SentTries < maxTries && !x.SentOnUtc.HasValue).ToList();

            foreach (var toSend in toSends)
            {

                if (string.IsNullOrEmpty(toSend.FromName) || string.IsNullOrEmpty(toSend.From) || string.IsNullOrEmpty(toSend.ToName) || string.IsNullOrEmpty(toSend.To))
                    continue;

                // email = "richardrcruzc@gmail.com";
                var emailMessage = new MimeMessage();

                emailMessage.From.Add(new MailboxAddress(toSend.FromName, toSend.From));
                emailMessage.To.Add(new MailboxAddress(toSend.ToName, toSend.To));
                emailMessage.Subject = toSend.Subject;
                // emailMessage.Body = new TextPart("plain") { Text = message };
                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = toSend.Body;
                emailMessage.Body = bodyBuilder.ToMessageBody();

                try
                {
                    using (var client = new SmtpClient())
                    {

                        client.LocalDomain = _settings.EmailLocalDomain;

                        await client.ConnectAsync(_settings.EmailLocalDomain, _settings.EmailLocalPort, SecureSocketOptions.Auto).ConfigureAwait(false);
                        await client.AuthenticateAsync(_settings.EmailUser, _settings.EmailPassword);
                        await client.SendAsync(emailMessage).ConfigureAwait(false);
                        await client.DisconnectAsync(true).ConfigureAwait(false);

                        toSend.SentOnUtc = DateTime.Now;
                        toSend.SentTries++;
                        _context.Update(toSend);
                        await _context.SaveChangesAsync();

                    }
                }
                catch (Exception ex)
                {
                    toSend.AttachmentFileName = ex.Message;
                    toSend.SentTries++;
                    _context.Update(toSend);
                    await _context.SaveChangesAsync();
                    var m = ex.Message;
                }
            }

            // return Task.FromResult(0);
        }
    }
}
