using ApplicationCore.Entities.Helpers;
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
        public bool NeedaVanOrPickup { get; private set; }
        public int Quantity { get; private set; }
        public string IdentityCode { get; private set; }
        public string SecurityCode { get; private set; }

        public DateTime ShippingCreateDate { get; private set; }
        public DateTime ShippingUpdateDate { get; private set; }
        public DateTime ShippingPickupDate { get; private set; }

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

        public double Distance { get; private set; }
        public Decimal ChargeAmount { get; private set; }
        public Decimal Discount { get; private set; }
        public string PromoCode { get; private set; }        
        public Decimal Tax { get; private set; }

        public Decimal AmountPay { get; private set; }
         public Decimal ExtraCharge { get; private set; }
       public string ExtraChargeNote { get; private set; }

        public string PickupPictureUri { get; private set; }
        public string DeliveredPictureUri { get; private set; }

        public string DropPictureUri { get; private set; }
        public string DropComment { get; private set; }
        public DateTime Dropby { get; private set; }

        public Shipment SetDropInfo(string dropPictureUri, string comment)
        {
            DropPictureUri = dropPictureUri;
            DropComment = comment;
            Dropby = DateTime.Now;

            return this;
        }


        public string Note { get; private set; }

        public int? PackageSizeId { get; private set; }
        public PackageSize PackageSize { get; private set; }


        public string PaymentReceipt { get; private set; }
        public string PaymentReceived { get; private set; }
        public string PaymentReceivedDate { get; private set; }
        public string PaymentNotes { get; private set; }

        public void SetAwaitingValidationStatus()
        {
        }

        public List<PackageStatusHistory> PackageStatusHistories { get; private set; }
        public Shipment AddStatusHistory(PackageStatusHistory history)
        {
            if (PackageStatusHistories == null)
                PackageStatusHistories = new List<PackageStatusHistory>();

            PackageStatusHistories.Add(history);
            return this;
        }


        public List<Review> Reviews { get; private set; }

        protected Shipment()
        {
            _paymentMethods = new List<PaymentMethod>();
        }
        public Shipment ApplyPayment(decimal amountPay)
        {
            AmountPay = amountPay;
            return this;
        }


        public Shipment SetupPayAmount(double distance, decimal chargeAmount, decimal discount, string promoCode, decimal tax,  decimal amountPay, decimal extraCharge,string extraChargeNote)
        {
            Distance = distance;
            ChargeAmount = chargeAmount;
            Discount = discount;
            PromoCode = promoCode;
            Tax = tax;
            Quantity = 1;
            AmountPay = amountPay;
            ExtraCharge = extraCharge;
            ExtraChargeNote = extraChargeNote;

            return this;
        }

        public Shipment(Address pickup, Address delivery, Customer sender, decimal amount, decimal discount, decimal shippingWeight, int priorityTypeId ,
                            int transportTypeId, string note, string pickupPictureUri, string deliveredPictureUri,
                            double distance, decimal chargeAmount,  string promoCode, decimal tax, int packageSizeId ,
                            decimal extraCharge, string extraChargeNote, bool needaVanOrPickup
                            ) : this()
        {
            NeedaVanOrPickup = needaVanOrPickup;
            ShippingStatusId = ShippingStatus.NoDriverAssigned.Id;
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

            Random randObj = new Random(1000);
            SecurityCode = randObj.Next(10000,99999).ToString();

            ShippingWeight = shippingWeight;
            Quantity = 1; 
            Distance = distance;
            ChargeAmount = chargeAmount; 
            PromoCode = promoCode;
            Tax = tax;

            PackageSizeId = packageSizeId;

            ExtraCharge = extraCharge;
            ExtraChargeNote = extraChargeNote;

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

        public Shipment SetDropPictureUri(string uri)
        {
            DropPictureUri = uri;
            return this;
        }
        public Shipment SetDropComment(string comment)
        {
            DropComment = comment;
            return this;
        }
        public Shipment SetSecyurityCode(string code)
        {
            SecurityCode = code;
            return this;
        }

        public Shipment SetDriver(Customer driver)
        {
            Driver = driver;
            DriverId = driver.Id;
            return this;
        }

        public Shipment ChangeStatus(int shippingStatusId)
        {
            ShippingStatusId = shippingStatusId;
            ShippingUpdateDate = DateTime.Now;
            if(shippingStatusId==4)
            ShippingPickupDate = DateTime.Now;
            return this;
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
