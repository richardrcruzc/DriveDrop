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
        public string UserName { get; private set; }
        public string UserNameToImpersonate { get; private set; }
        public string VerificationId { get; private set; }

        public string Email { get; private set; }
        public string PrimaryPhone { get; private set; }
        public string Phone { get; private set; }
        public string LastName { get; private set; }
        public string FirstName { get; private set; }
        public CustomerType CustomerType { get; private set; }
        public int? CustomerTypeId { get; private set; }
        public int? TransportTypeId { get; private set; }
        public TransportType TransportType { get; private set; }
        public int? CustomerStatusId { get; private set; }
        public CustomerStatus CustomerStatus { get; private set; }
        public int? MaxPackage { get; private set; }
        public double? PickupRadius { get; private set; }
        public double? DeliverRadius { get; private set; }

        public decimal Commission { get; private set; }

        public string VehicleMake { get; private set; }
        public string VehicleModel { get; private set; }
        public string VehicleColor { get; private set; }
        public string VehicleYear { get; private set; } 

        public Address DefaultAddress { get; private set; }

        public string PaymentMethodId { get; private set; }
         
        //  public List<Shipment> Driver { get; private set; }
        public virtual ICollection<Shipment> ShipmentDrivers { get; private  set; }
        public virtual ICollection<Shipment> ShipmentSenders { get; private set; }

        public string DriverLincensePictureUri { get; private set; }
        public string PersonalPhotoUri { get; private set; }
        public string VehiclePhotoUri { get; private set; }
        public string InsurancePhotoUri { get; private set; }

        public Customer EndImpersonate()
        {
            UserNameToImpersonate = null;

            return this;
        }

        public Customer Impersonate(string userName)
        {
            UserNameToImpersonate = userName;

            return this;
        }

        public Customer AddUserName(string userName)
        {
            UserName  = userName;

            return this;
        }
        public Customer AddDefaultAddress(Address address)
        {
            address.UpdateType("home");
            DefaultAddress = address; 

            return this;
        }
        public Customer UpdateVehicleInfo(string lincensePictureUri, string vehiclePhotoUri, string insurancePhotoUri, int vehicleTypeId, 
            string vehicleMake, string vehicleModel, string vehicleColor, string vehicleYear)
        {
            DriverLincensePictureUri = lincensePictureUri;
            VehiclePhotoUri = vehiclePhotoUri;
            InsurancePhotoUri = insurancePhotoUri;
            TransportTypeId = vehicleTypeId;

            VehicleMake = vehicleMake;
            VehicleModel = vehicleModel;
            VehicleColor = vehicleColor;
            VehicleYear = vehicleYear;

            return this;
        }
        public Customer UpdateInfo(int statusId, string firstName, string lastName, string email, string primaryPhone, string phone , string photoUrl, string verificationId)
        {
           // CustomerStatusId = statusId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PrimaryPhone = primaryPhone;
            Phone = phone;
            PersonalPhotoUri = photoUrl;
            VerificationId = verificationId;

            return this;
        }
        
        public Customer AddPicture(string url, string type = "driver")
        {
            if(type == "driver")
            DriverLincensePictureUri = url;
            if (type == "personal")
                PersonalPhotoUri = url;
            if (type == "vehicle")
                VehiclePhotoUri = url;
            if (type == "insurance")
                InsurancePhotoUri = url;

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
        public ICollection<Address> Addresses { get; private set; }

        protected Customer()
        {
            _paymentMethods = new List<PaymentMethod>();
            Addresses = new List<Address>();
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
      int customerTypeId  ,
      int maxPackage ,
      int pickupRadius ,
      int deliverRadius , 
      decimal commission , 
      string userName , 
      string primaryPhone,
      string driverLincensePictureUri, 
      string personalPhotoUri, 
      string vehiclePhotoUri, 
      string insurancePhotoUri,

      string vehicleMake,
      string vehicleModel,
      string vehicleColor,
      string vehicleYear
      ) : this()
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
            UserName = userName; 
             
            PrimaryPhone = primaryPhone;
            PersonalPhotoUri = personalPhotoUri;
            DriverLincensePictureUri = driverLincensePictureUri;
            VehiclePhotoUri = vehiclePhotoUri;
            InsurancePhotoUri = InsurancePhotoUri;

            VehicleMake= vehicleMake;
            VehicleModel = vehicleModel;
            VehicleColor = vehicleColor;
            VehicleYear = vehicleYear;
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
        string email,
      string phone,
       int customerTypeId = 2,      
        decimal commission = 10, string userName = "",   string primaryPhone = "")
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
            UserName = userName; 
            PrimaryPhone = primaryPhone;
            return this;
        }

         
        public Customer ChangeStatus(CustomerStatus status)
        {
            CustomerStatusId = status.Id;

            return this;
        }
        public Address AddAddress(Address address)
        { 
            var existing = Addresses.Where(a => a.Equals(address)).SingleOrDefault();
           
            if (existing != null)            
                return existing;
           
            Addresses.Add(address);
 
            return address;
        }

        public Address DeleteAddress(Address address)
        {
            var existing = Addresses.Where(a => a.Equals(address)).SingleOrDefault();

            if (existing == null)
                return existing;

            Addresses.Remove(address);

            return address;
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

