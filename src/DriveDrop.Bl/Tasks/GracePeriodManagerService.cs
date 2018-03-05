using DriveDrop.Bl.Services;
using DriveDrop.Bl.Tasks.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;


namespace DriveDrop.Bl.Tasks
{
    public class GracePeriodManagerService
        : BackgroundService
    {
        private readonly ILogger<GracePeriodManagerService> _logger;
        private readonly IBackGroundEmailService _sendEmail; 

        public GracePeriodManagerService(
           IBackGroundEmailService sendEmail, 
                                         ILogger<GracePeriodManagerService> logger)
        {
           _sendEmail = sendEmail;
              _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug($"GracePeriodManagerService is starting.");

            stoppingToken.Register(() => _logger.LogDebug($"#1 GracePeriodManagerService background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug($"GracePeriodManagerService background task is doing background work.");

                CheckConfirmedGracePeriodOrders();

                await Task.Delay(1000, stoppingToken);
            }

            _logger.LogDebug($"GracePeriodManagerService background task is stopping.");

            await Task.CompletedTask;
        }

        private void CheckConfirmedGracePeriodOrders()
        {
            _logger.LogDebug($"Checking confirmed grace period orders");

              _sendEmail.SendBatchEmailFromQueueAsync().GetAwaiter();
        }

       
    }
}
