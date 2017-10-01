using ApplicationCore.Entities.Helpers;
using DriveDrop.Api.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.Services
{
    
    public interface IQueueService
    {
         
        Task<bool> InsertQ(string email, string subject, string message);
    }

    public class QueueService: IQueueService
    {
        private readonly DriveDropContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IOptionsSnapshot<AppSettings> _settings;

        public QueueService(IHostingEnvironment env, DriveDropContext context, IOptionsSnapshot<AppSettings> settings)
        {
            _context = context;
            _env = env;
            _settings = settings;
        }

        public async Task<bool> InsertQ(string email, string subject, string message)
        {

            var q = new QueuedEmail
            {
                Body = message,
                From = _settings.Value.EmailSenderEmail,
                Subject = message,
                To=email,


            };

            _context.Add(q);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
