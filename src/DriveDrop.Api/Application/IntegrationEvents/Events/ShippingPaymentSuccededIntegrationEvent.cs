namespace DriveDrop.Api.Application.IntegrationEvents.Events
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class ShippingPaymentSuccededIntegrationEvent : IntegrationEvent
    {
        public int ShippingId { get; }

        public ShippingPaymentSuccededIntegrationEvent(int shippingId) => ShippingId = shippingId;
    }
}