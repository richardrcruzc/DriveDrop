using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Events
{
    
    /// <summary>
    /// Event used when an order is created
    /// </summary>
    public class ShippingStartedDomainEvent
        : INotification
    {
        public string UserId { get; private set; }
        public int CardTypeId { get; private set; }
        public string CardNumber { get; private set; }
        public string CardSecurityNumber { get; private set; }
        public string CardHolderName { get; private set; }
        public DateTime CardExpiration { get; private set; }
        public Shipment Shipment { get; private set; }

        public ShippingStartedDomainEvent(Shipment shipment, string userId,
            int cardTypeId, string cardNumber,
            string cardSecurityNumber, string cardHolderName,
            DateTime cardExpiration)
        {
            Shipment = shipment;
            UserId = userId;
            CardTypeId = cardTypeId;
            CardNumber = cardNumber;
            CardSecurityNumber = cardSecurityNumber;
            CardHolderName = cardHolderName;
            CardExpiration = cardExpiration;
        }
    }
}
