namespace DriveDrop.Api.Application.IntegrationEvents.EventHandling
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions; 
    using DriveDrop.Api.Application.IntegrationEvents.Events;
    using System.Threading.Tasks;
    using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;

    public class ShippingPaymentSuccededIntegrationEventHandler : 
        IIntegrationEventHandler<ShippingPaymentSuccededIntegrationEvent>
    {
        private readonly IShipmentRepository _sRepository;

        public ShippingPaymentSuccededIntegrationEventHandler(IShipmentRepository orderRepository)
        {
            _sRepository = orderRepository;
        }

        public async Task Handle(ShippingPaymentSuccededIntegrationEvent @event)
        {
            var orderToUpdate = await _sRepository.GetAsync(@event.ShippingId);

            //orderToUpdate.SetPaidStatus();

            await _sRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}