namespace Payment.API.IntegrationEvents.Events
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class ShippingPaymentFailedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }

        public ShippingPaymentFailedIntegrationEvent(int orderId) => OrderId = orderId;
    }
}