namespace DriveDrop.Api.Application.IntegrationEvents.Events
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class GracePeriodConfirmedIntegrationEvent : IntegrationEvent
    {
        public int ShippingId { get; }

        public GracePeriodConfirmedIntegrationEvent(int shippingId) =>
            ShippingId = shippingId;
    }
}
