using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DriveDrop.Bl.Services
{
    public class GracePeriodManagerService
       : BackgroundService
    {
        private readonly ILogger<GracePeriodManagerService> _logger;
      //  private readonly IBackGroundEmailService _email;

        private readonly AppSettings _settings;

        public GracePeriodManagerService(
            //IBackGroundEmailService email,
            IOptionsSnapshot<AppSettings> settings,
                                         ILogger<GracePeriodManagerService> logger)
        {
            _logger = logger;
            //_email = email;
            _settings = settings.Value;     
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug($"GracePeriodManagerService is starting.");

            stoppingToken.Register(() =>
                    _logger.LogDebug($" GracePeriod background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug($"GracePeriod task doing background work.");
                
              //   await _email.SendBatchEmailFromQueueAsync();

              await Task.Delay(60000, stoppingToken);
            }

            _logger.LogDebug($"GracePeriod background task is stopping.");
        }

        //protected override async Task StopAsync(CancellationToken stoppingToken)
        //{
        //    // Run your graceful clean-up actions
        //}
    }
}
