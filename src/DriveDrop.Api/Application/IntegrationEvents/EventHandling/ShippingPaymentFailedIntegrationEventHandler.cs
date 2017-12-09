namespace DriveDrop.Api.Application.IntegrationEvents.EventHandling
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions; 
    using DriveDrop.Api.Application.IntegrationEvents.Events;
    using System.Threading.Tasks;
    using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;

    public class ShippingPaymentFailedIntegrationEventHandler : 
        IIntegrationEventHandler<ShippingPaymentFailedIntegrationEvent>
    {
        private readonly IShipmentRepository _sRepository;

        public ShippingPaymentFailedIntegrationEventHandler(IShipmentRepository orderRepository)
        {
            _sRepository = orderRepository;
        }

        public async Task Handle(ShippingPaymentFailedIntegrationEvent @event)
        {
            var orderToUpdate = await _sRepository.GetAsync(@event.ShippingId);

            //orderToUpdate.SetCancelledStatus();

            await _sRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
