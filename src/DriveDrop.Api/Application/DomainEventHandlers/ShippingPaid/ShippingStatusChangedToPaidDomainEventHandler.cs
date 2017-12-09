namespace DriveDrop.Api.Application.DomainEventHandlers.OrderPaid
{
    using MediatR; 
    using Microsoft.Extensions.Logging; 
    using System;
    using System.Threading.Tasks;
    using DriveDrop.Api.Application.IntegrationEvents;
    using System.Linq;
    using DriveDrop.Api.Application.IntegrationEvents.Events;
    using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
    using ApplicationCore.Events;

    public class ShippingStatusChangedToPaidDomainEventHandler
                   : IAsyncNotificationHandler<ShippingStatusChangedToPaidDomainEvent>
    {
        private readonly IShipmentRepository _sRepository;
        private readonly ILoggerFactory _logger;
        private readonly IShippingIntegrationEventService _sIntegrationEventService;

        public ShippingStatusChangedToPaidDomainEventHandler(
            IShipmentRepository orderRepository, ILoggerFactory logger,
            IShippingIntegrationEventService sIntegrationEventService)
        {
            _sRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _sIntegrationEventService = sIntegrationEventService;
        }

        public async Task Handle(ShippingStatusChangedToPaidDomainEvent sStatusChangedToPaidDomainEvent)
        {
            _logger.CreateLogger(nameof(ShippingStatusChangedToPaidDomainEventHandler))
                .LogTrace($"Shipping with Id: {sStatusChangedToPaidDomainEvent.ShippingId} has been successfully updated with " +
                          $"a status order id: {1}");

            //var orderStockList = sStatusChangedToPaidDomainEvent.OrderItems
            //    .Select(orderItem => new OrderStockItem(orderItem.ProductId, orderItem.GetUnits()));

            var sStatusChangedToPaidIntegrationEvent = new ShippingStatusChangedToPaidIntegrationEvent(sStatusChangedToPaidDomainEvent.ShippingId);
            await _sIntegrationEventService.PublishThroughEventBusAsync(sStatusChangedToPaidIntegrationEvent);
        }
    }  
}