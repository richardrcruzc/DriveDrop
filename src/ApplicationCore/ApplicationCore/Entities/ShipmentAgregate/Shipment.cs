﻿using ApplicationCore.Entities.Helpers;
using ApplicationCore.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.ClientAgregate.ShipmentAgregate
{
    public class Shipment
         : Entity, IAggregateRoot
    {

        public int Quantity { get; private set; }
        public string IdentityCode { get; private set; }

        public DateTime ShippingCreateDate { get; private set; }
        public DateTime ShippingUpdateDate { get; private set; }

        public  int SenderId { get; private set; }
        public int? DriverId { get; private set; }

       // [InverseProperty("Author")]
        public virtual Customer Sender { get; private set; }
        //[InverseProperty("Author")]
        public virtual Customer Driver { get; private set; }
       

        public Address PickupAddress { get; private set; }
        public Address DeliveryAddress { get; private set; } 

        public ShippingStatus ShippingStatus { get; private set; }
        public int ShippingStatusId { get; private set; }

        public PriorityType PriorityType { get; private set; }
        public int PriorityTypeId { get; private set; }
        public int PriorityTypeLevel { get; private set; }


        public int TransportTypeId { get; private set; }
        public TransportType TransportType { get; private set; }
        
        public Decimal ShippingWeight { get; private set; }
        public Decimal ShippingValue { get; private set; }

        public Decimal Distance { get; private set; }
        public Decimal ChargeAmount { get; private set; }
        public Decimal Discount { get; private set; }
        public string PromoCode { get; private set; }        
        public Decimal Tax { get; private set; }

        public Decimal AmountPay { get; private set; }

        public string PickupPictureUri { get; private set; }
        public string DeliveredPictureUri { get; private set; }


        public string Note { get; private set; }

        public int? PackageSizeId { get; private set; }
        public PackageSize PackageSize { get; private set; }        


        protected Shipment()
        {
            _paymentMethods = new List<PaymentMethod>();
        }
        public Shipment ApplyPayment(decimal amountPay)
        {
            AmountPay = amountPay;
            return this;
        }


        public Shipment SetupPayAmount(decimal distance, decimal chargeAmount, decimal discount, string promoCode, decimal tax, int qty = 1, decimal amountPay=0)
        {
            Distance = distance;
            ChargeAmount = chargeAmount;
            Discount = discount;
            PromoCode = promoCode;
            Tax = tax;
            Quantity = qty;
            AmountPay = amountPay;
            return this;
        }

        public Shipment(Address pickup, Address delivery, Customer sender, decimal amount, decimal discount, decimal shippingWeight, int priorityTypeId ,
                            int transportTypeId, string note, string pickupPictureUri, string deliveredPictureUri,
                            decimal distance, decimal chargeAmount,  string promoCode, decimal tax, int packageSizeId 
                            ) : this()
        {
            ShippingStatusId = ShippingStatus.PendingPickUp.Id;
            ShippingCreateDate = DateTime.Now;
            ShippingUpdateDate = DateTime.Now;
            ShippingValue = amount;
            Discount = discount;
            PriorityTypeId = priorityTypeId;
            if (transportTypeId == 0)
                TransportTypeId = 1;
            else
            TransportTypeId = transportTypeId;

            Note = note;
            PickupPictureUri = pickupPictureUri;
            DeliveredPictureUri = deliveredPictureUri;

            PickupAddress = pickup;
            DeliveryAddress = delivery;

            Sender = sender;
            // SenderId = sender.Id;

            IdentityCode = string.Format("WA-{0}-{1}",pickup.ZipCode, RandomString());

            ShippingWeight = shippingWeight;
            Quantity = 1; 
            Distance = distance;
            ChargeAmount = chargeAmount; 
            PromoCode = promoCode;
            Tax = tax;

            PackageSizeId = packageSizeId;

        }
        public static string RandomString()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            return finalString;

        }
        public Shipment SetPickupPictureUri(string uri)
        {
            PickupPictureUri = uri;

            return this;
        }
        public Shipment SetDeliveredPictureUri(string uri)
        {
            DeliveredPictureUri = uri;

            return this;
        }

        public Shipment SetDriver(Customer driver)
        {
            Driver = driver;
            DriverId = driver.Id;
            return this;
        }

        public void ChangeStatus(int shippingStatusId)
        {
            ShippingStatusId = shippingStatusId;
            ShippingUpdateDate = DateTime.Now;
        }

 

        private List<PaymentMethod> _paymentMethods;

        public PaymentMethod VerifyOrAddPaymentMethod(
            int cardTypeId, string alias, string cardNumber,
            string securityNumber, string cardHolderName, DateTime expiration, int orderId)
        {
            var existingPayment = _paymentMethods.Where(p => p.IsEqualTo(cardTypeId, cardNumber, expiration))
                .SingleOrDefault();

            if (existingPayment != null)
            {
                //    AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, existingPayment, orderId));

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
