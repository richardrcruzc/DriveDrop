using ApplicationCore.Entities.ClientAgregate;
using MediatR;
 

namespace ApplicationCore.Events
{
 
    public class CustomerAndPaymentMethodVerifiedDomainEvent
       : INotification
    {
        public Customer Customer { get; private set; }
        public PaymentMethod Payment { get; private set; }
        public int ShippingId { get; private set; }

        public CustomerAndPaymentMethodVerifiedDomainEvent(Customer customer, PaymentMethod payment, int shippingId)
        {
            Customer = customer;
            Payment = payment;
            ShippingId = shippingId;
        }
    }
}
