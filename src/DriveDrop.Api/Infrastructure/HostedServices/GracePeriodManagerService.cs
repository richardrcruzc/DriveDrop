 using Dapper;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using DriveDrop.Api;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DriveDrop.Api.Application.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace DriveDrop.Api.Infrastructure.HostedServices
{
    public class GracePeriodManagerService
      : HostedService
    {
        private readonly DriveDropSettings _settings;
        private readonly ILogger<GracePeriodManagerService> _logger;
        private readonly IEventBus _eventBus;

        public GracePeriodManagerService(IOptions<DriveDropSettings> settings,
            IEventBus eventBus,
            ILogger<GracePeriodManagerService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));

            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                CheckConfirmedGracePeriodOrders();

                await Task.Delay(_settings.CheckUpdateTime, cancellationToken);

                continue;
            }

            await Task.CompletedTask;
        }

        private void CheckConfirmedGracePeriodOrders()
        {
            _logger.LogDebug($"Checking confirmed grace period orders");

            var orderIds = GetConfirmedGracePeriodOrders();

            foreach (var shippingId in orderIds)
            {
                var confirmGracePeriodEvent = new GracePeriodConfirmedIntegrationEvent(shippingId);
                _eventBus.Publish(confirmGracePeriodEvent);
            }
        }

        private IEnumerable<int> GetConfirmedGracePeriodOrders()
        {
            IEnumerable<int> orderIds = new List<int>();

            using (var conn = new SqlConnection(_settings.ConnectionString))
            {
                try
                {
                    conn.Open();
                    orderIds = conn.Query<int>(
                        @"SELECT Id FROM [shippings].[shipment] 
                            WHERE DATEDIFF(minute, [ShippingCreateDate], GETDATE()) >= @GracePeriodTime
                            AND [ShippingStatusId] = 1",
                        new { GracePeriodTime = _settings.GracePeriodTime });
                }
                catch (SqlException exception)
                {
                    _logger.LogCritical($"FATAL ERROR: Database connections could not be opened: {exception.Message}");
                }

            }

            return orderIds;
        }
    }
}
