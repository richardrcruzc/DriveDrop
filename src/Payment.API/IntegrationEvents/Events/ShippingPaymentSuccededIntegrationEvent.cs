namespace Payment.API.IntegrationEvents.Events
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class ShippingPaymentSuccededIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }

        public ShippingPaymentSuccededIntegrationEvent(int orderId) => OrderId = orderId;
    }
}