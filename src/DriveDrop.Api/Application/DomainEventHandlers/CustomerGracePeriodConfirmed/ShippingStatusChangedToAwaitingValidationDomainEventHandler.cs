namespace DriveDrop.Api.Application.DomainEventHandlers.OrderGracePeriodConfirmed
{
    using MediatR; 
    using Microsoft.Extensions.Logging; 
    using System;
    using System.Threading.Tasks;
    using DriveDrop.Api.Application.IntegrationEvents;
    using System.Linq;
    using DriveDrop.Api.Application.IntegrationEvents.Events;
    using ApplicationCore.Events;
    using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;

    public class ShippingStatusChangedToAwaitingValidationDomainEventHandler
                   : IAsyncNotificationHandler<ShippingStatusChangedToAwaitingValidationDomainEvent>
    {
        private readonly IShipmentRepository _sRepository;
        private readonly ILoggerFactory _logger;
        private readonly IShippingIntegrationEventService _sIntegrationEventService;

        public ShippingStatusChangedToAwaitingValidationDomainEventHandler(
            IShipmentRepository sRepository, ILoggerFactory logger,
            IShippingIntegrationEventService sIntegrationEventService)
        {
            _sRepository = sRepository ?? throw new ArgumentNullException(nameof(sRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _sIntegrationEventService = sIntegrationEventService;
        }

        public async Task Handle(ShippingStatusChangedToAwaitingValidationDomainEvent shippingStatusChangedToAwaitingValidationDomainEvent)
        {
            _logger.CreateLogger(nameof(ShippingStatusChangedToAwaitingValidationDomainEvent))
                .LogTrace($"Shipping with Id: {shippingStatusChangedToAwaitingValidationDomainEvent.ShippingId} has been successfully updated with " +
                      $"a status order id: {1}");

            //var orderStockList = shippingStatusChangedToAwaitingValidationDomainEvent.ShippingId
            //    .Select(orderItem => new OrderStockItem(orderItem.ProductId, orderItem.GetUnits()));

            var shippingStatusChangedToAwaitingValidationIntegrationEvent = new ShippingStatusChangedToAwaitingValidationIntegrationEvent(
                shippingStatusChangedToAwaitingValidationDomainEvent.ShippingId);
            await _sIntegrationEventService.PublishThroughEventBusAsync(shippingStatusChangedToAwaitingValidationIntegrationEvent);
        }
    }  
}