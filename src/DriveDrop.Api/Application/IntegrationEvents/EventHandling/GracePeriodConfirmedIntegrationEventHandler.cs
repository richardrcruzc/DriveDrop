using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions; 
using DriveDrop.Api.Application.IntegrationEvents.Events;
using System.Threading.Tasks;
using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;

namespace DriveDrop.Api.Application.IntegrationEvents.EventHandling
{
    public class GracePeriodConfirmedIntegrationEventHandler : IIntegrationEventHandler<GracePeriodConfirmedIntegrationEvent>
    {
        private readonly IShipmentRepository _sRepository;

        public GracePeriodConfirmedIntegrationEventHandler(IShipmentRepository orderRepository)
        {
            _sRepository = orderRepository;
        }

        /// <summary>
        /// Event handler which confirms that the grace period
        /// has been completed and order will not initially be cancelled.
        /// Therefore, the order process continues for validation. 
        /// </summary>
        /// <param name="event">       
        /// </param>
        /// <returns></returns>
        public async Task Handle(GracePeriodConfirmedIntegrationEvent @event)
        {
            var orderToUpdate = await _sRepository.GetAsync(@event.ShippingId);
           orderToUpdate.SetAwaitingValidationStatus();
            await _sRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
