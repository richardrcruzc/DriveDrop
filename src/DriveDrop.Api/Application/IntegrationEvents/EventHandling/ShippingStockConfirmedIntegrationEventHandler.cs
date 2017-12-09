namespace DriveDrop.Api.Application.IntegrationEvents.EventHandling
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using System.Threading.Tasks;
    using Events;
    using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;

    public class ShippingStockConfirmedIntegrationEventHandler : 
        IIntegrationEventHandler<ShippingStockConfirmedIntegrationEvent>
    {
        private readonly IShipmentRepository _sRepository;

        public ShippingStockConfirmedIntegrationEventHandler(IShipmentRepository orderRepository)
        {
            _sRepository = orderRepository;
        }

        public async Task Handle(ShippingStockConfirmedIntegrationEvent @event)
        {
            var orderToUpdate = await _sRepository.GetAsync(@event.ShippingId);

            //orderToUpdate.SetStockConfirmedStatus();

            await _sRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}