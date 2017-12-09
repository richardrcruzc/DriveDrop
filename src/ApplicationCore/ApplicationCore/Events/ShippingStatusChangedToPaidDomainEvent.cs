using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Events
{
    
    /// <summary>
    /// Event used when the order is paid
    /// </summary>
    public class ShippingStatusChangedToPaidDomainEvent
        : INotification
    {
        public int ShippingId { get; }
        //public IEnumerable<OrderItem> OrderItems { get; }

        public ShippingStatusChangedToPaidDomainEvent(int shippingId
            //,            IEnumerable<OrderItem> orderItems
            )
        {
            ShippingId = shippingId;
            //OrderItems = orderItems;
        }
    }
}
