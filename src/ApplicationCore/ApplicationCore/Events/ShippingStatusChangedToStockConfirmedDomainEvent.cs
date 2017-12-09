 namespace ApplicationCore.Events
{
    

      using MediatR;

    /// <summary>
    /// Event used when the order stock items are confirmed
    /// </summary>
    public class ShippingStatusChangedToStockConfirmedDomainEvent
        : INotification
    {
        public int ShippingId { get; }

        public ShippingStatusChangedToStockConfirmedDomainEvent(int shippingId)
            => ShippingId = shippingId;
    }
}
