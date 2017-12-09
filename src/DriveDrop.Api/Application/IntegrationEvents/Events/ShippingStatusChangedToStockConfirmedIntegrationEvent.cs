namespace DriveDrop.Api.Application.IntegrationEvents.Events
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class ShippingStatusChangedToStockConfirmedIntegrationEvent : IntegrationEvent
    {
        public int ShippingId { get; }

        public ShippingStatusChangedToStockConfirmedIntegrationEvent(int shippingId)
            => ShippingId = shippingId;
    }
}