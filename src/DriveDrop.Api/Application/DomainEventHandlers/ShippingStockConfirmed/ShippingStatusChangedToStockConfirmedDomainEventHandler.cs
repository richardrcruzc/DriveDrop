namespace DriveDrop.Api.Application.DomainEventHandlers.OrderStockConfirmed
{
    using MediatR; 
    using Microsoft.Extensions.Logging; 
    using System;
    using System.Threading.Tasks;
    using DriveDrop.Api.Application.IntegrationEvents;
    using DriveDrop.Api.Application.IntegrationEvents.Events;
    using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
    using ApplicationCore.Events;

    public class ShippingStatusChangedToStockConfirmedDomainEventHandler
                   : IAsyncNotificationHandler<ShippingStatusChangedToStockConfirmedDomainEvent>
    {
        private readonly IShipmentRepository _sRepository;
        private readonly ILoggerFactory _logger;
        private readonly IShippingIntegrationEventService _sIntegrationEventService;

        public ShippingStatusChangedToStockConfirmedDomainEventHandler(
            IShipmentRepository orderRepository, ILoggerFactory logger,
            IShippingIntegrationEventService sIntegrationEventService)
        {
            _sRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _sIntegrationEventService = sIntegrationEventService;
        }

        public async Task Handle(ShippingStatusChangedToStockConfirmedDomainEvent shippingStatusChangedToStockConfirmedDomainEvent)
        {
            _logger.CreateLogger(nameof(ShippingStatusChangedToStockConfirmedDomainEventHandler))
                .LogTrace($"Shipping with Id: {shippingStatusChangedToStockConfirmedDomainEvent.ShippingId} has been successfully updated with " +
                          $"a status order id: {ShippingStatus.PendingPickUp.Id}");

            var orderStatusChangedToStockConfirmedIntegrationEvent = new ShippingStatusChangedToStockConfirmedIntegrationEvent(shippingStatusChangedToStockConfirmedDomainEvent.ShippingId);
            await _sIntegrationEventService.PublishThroughEventBusAsync(orderStatusChangedToStockConfirmedIntegrationEvent);
        }
    }  
}