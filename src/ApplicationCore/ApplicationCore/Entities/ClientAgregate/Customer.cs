using ApplicationCore.SeedWork;
using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities.Helpers;

namespace ApplicationCore.Entities.ClientAgregate
{
    public class Customer : Entity, IAggregateRoot
    {
        public string IdentityGuid { get; private set; }
        public string UserGuid { get; private set; }

        public string Email { get; private set; }
        public string Phone { get; private set; }
        public string LastName { get; private set; }
        public string FirstName { get; private set; }
        public CustomerType CustomerType { get; private set; }
        public int CustomerTypeId { get; private set; }
        public int? TransportTypeId { get; private set; }
        public TransportType TransportType { get; private set; }
        public int CustomerStatusId { get; private set; }
        public CustomerStatus CustomerStatus { get; private set; }
        public int? MaxPackage { get; private set; }
        public int? PickupRadius { get; private set; }
        public int? DeliverRadius { get; private set; }

        public decimal Commission { get; private set; }

        public Address DefaultAddress { get; private set; }

        //  public List<Shipment> Driver { get; private set; }
        public virtual ICollection<Shipment> ShipmentDrivers { get; private  set; }
        public virtual ICollection<Shipment> ShipmentSenders { get; private set; }

        public string DriverLincensePictureUri { get; private set; }


        public Customer AddDefaultAddress(Address address)
        {
            DefaultAddress = address;

            return this;
        }


        public Customer AddPicture(string url)
        {
            DriverLincensePictureUri = url;

            return this;
        }

        public string FullName
        {
            get
            {
                return LastName + ", " + FirstName;
            }
        }

        private List<PaymentMethod> _paymentMethods;
 
        protected Customer()
        {
            _paymentMethods = new List<PaymentMethod>();

        }



        public Customer(string identity) : this()
        {
            IdentityGuid = !string.IsNullOrWhiteSpace(identity) ? identity : throw new ArgumentNullException(nameof(identity));
        }

        public Customer(
            string identity,
      string firstName,
      string lastName,
      int? transportTypeId,
      int statusId  ,
      string email,
      string phone,
      int customerTypeId = 2,
      int maxPackage=0,
      int pickupRadius=0,
      int deliverRadius=0, decimal commission =10) : this()
        {
            LastName = lastName;
            FirstName = firstName;            
            TransportTypeId = transportTypeId;
            CustomerStatusId = statusId;
            MaxPackage = maxPackage;
            PickupRadius = pickupRadius;
            DeliverRadius = deliverRadius;
            CustomerTypeId = customerTypeId;
            IdentityGuid = !string.IsNullOrWhiteSpace(identity) ? identity : throw new ArgumentNullException(nameof(identity));

            Email = email;
            Phone = phone;

            Commission = commission;
        } 

        public Customer Update( 
             string identity,
       string firstName,
       string lastName,
       int transportTypeId,
       int statusId, 
       int maxPackage,
       int pickupRadius,
       int deliverRadius,
       int customerTypeId = 2)
        {
            
            LastName = lastName;
            FirstName = firstName;
            TransportTypeId = transportTypeId;
            CustomerStatusId = statusId;
            MaxPackage = maxPackage;
            PickupRadius = pickupRadius;
            DeliverRadius = deliverRadius;
            CustomerTypeId = customerTypeId;
            IdentityGuid = !string.IsNullOrWhiteSpace(identity) ? identity : throw new ArgumentNullException(nameof(identity));

            return this;
        }

         
        public Customer ChangeStatus(CustomerStatus status)
        {
            CustomerStatusId = status.Id;

            return this;
        }


 


        public PaymentMethod VerifyOrAddPaymentMethod(
        int cardTypeId, string alias, string cardNumber,
        string securityNumber, string cardHolderName, DateTime expiration, int orderId)
        {
            var existingPayment = _paymentMethods.Where(p => p.IsEqualTo(cardTypeId, cardNumber, expiration))
                .SingleOrDefault();

            if (existingPayment != null)
            {
                // AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, existingPayment, orderId));

                return existingPayment;
            }
            else
            {
                var payment = new PaymentMethod(cardTypeId, alias, cardNumber, securityNumber, cardHolderName, expiration);

                _paymentMethods.Add(payment);

                // AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, payment, orderId));

                return payment;
            }
        }
    }
}
