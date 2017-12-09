namespace DriveDrop.Api.Application.IntegrationEvents.Events
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class ShippingStockConfirmedIntegrationEvent : IntegrationEvent
    {
        public int ShippingId { get; }

        public ShippingStockConfirmedIntegrationEvent(int shippingId) => ShippingId = shippingId;
    }
}