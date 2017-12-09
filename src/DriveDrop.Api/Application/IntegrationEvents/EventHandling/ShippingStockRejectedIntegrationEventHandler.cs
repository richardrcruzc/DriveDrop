namespace DriveDrop.Api.Application.IntegrationEvents.EventHandling
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using System.Threading.Tasks;
    using Events;
    using System.Linq;
    using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;

    public class ShippingStockRejectedIntegrationEventHandler : IIntegrationEventHandler<ShippingStockRejectedIntegrationEvent>
    {
        private readonly IShipmentRepository _sRepository;

        public ShippingStockRejectedIntegrationEventHandler(IShipmentRepository orderRepository)
        {
            _sRepository = orderRepository;
        }

        public async Task Handle(ShippingStockRejectedIntegrationEvent @event)
        {
            var orderToUpdate = await _sRepository.GetAsync(@event.ShippingId);

            //var orderStockRejectedItems = @event.OrderStockItems
            //    .FindAll(c => !c.HasStock)
            //    .Select(c => c.ProductId);

          //  orderToUpdate.SetCancelledStatusWhenStockIsRejected(orderStockRejectedItems);

            await _sRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}