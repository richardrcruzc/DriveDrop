namespace DriveDrop.Api.Application.IntegrationEvents.Events
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class ShippingPaymentFailedIntegrationEvent : IntegrationEvent
    {
        public int ShippingId { get; }

        public ShippingPaymentFailedIntegrationEvent(int shippingId) => ShippingId = shippingId;
    }
}