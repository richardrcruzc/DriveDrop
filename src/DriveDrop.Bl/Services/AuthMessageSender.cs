using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using DriveDrop.Bl.Infrastructure;
using ApplicationCore.Entities.Helpers;
using DriveDrop.Bl.Data;

namespace DriveDrop.Bl.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        //private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly AppSettings _settings;

        private readonly IHostingEnvironment _env;

        private readonly DriveDropContext _context; 
        public AuthMessageSender(DriveDropContext context, IHostingEnvironment env, IOptions<AppSettings> settings)
        {
            _context = context;
            _env = env;
            _settings = settings.Value;
        }

        
        public async Task SendEmailAsync(string email, string subject, string message)
            {

            var customer = _context.Customers.OrderBy(u=>u.UserName).Where(x => x.UserName == email).FirstOrDefault();
            var q = new QueuedEmail
            {
                Body = message,
                From = _settings.EmailSenderEmail,
                FromName = _settings.EmailSenderName,
                Subject = subject,
                To = email,
                ToName= customer.FullName,
                PriorityId= 1,
                 SentTries=0,
                 CreatedOnUtc =DateTime.Now,
                 
            };

            _context.Add(q);            
            await _context.SaveChangesAsync();
           
            //await  SendBatchEmailFromQueueAsync();
            

            //RecurringJob.AddOrUpdate(() => SendBatchEmailFromQueueAsync(), Cron.Minutely);
            //var jobId = BackgroundJob.Enqueue(() => SendEmailFromQueueAsync(q.Id));
            //var succeeded = BackgroundJob.Requeue(jobId);

        }
        public async Task SendEmailFromQueueAsync(int id)
        { 
            var maxTries = 3;
            var toSend = _context.QueuedEmails.OrderBy(u => u.Id).Where(x =>x.Id == id&& x.SentTries < maxTries && !x.SentOnUtc.HasValue).FirstOrDefault();
             
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
             
            // return Task.FromResult(0);
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

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
