using MediatR;
 
namespace ApplicationCore.Events
{
    
    /// <summary>
    /// Event used when the grace period order is confirmed
    /// </summary>
    public class ShippingStatusChangedToAwaitingValidationDomainEvent
         : INotification
    {
        public int ShippingId { get; }
       // public IEnumerable<OrderItem> OrderItems { get; }

        public ShippingStatusChangedToAwaitingValidationDomainEvent(int shippingId
          //,  IEnumerable<OrderItem> orderItems
          )
        {
            ShippingId = shippingId;
            //OrderItems = orderItems;
        }
    }
}
