using GoDriveDrop.Core.Models.Commons;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoDriveDrop.Core.Models.Shippments
{ 
    public class NewPackageModel
    {
        public NewPackageModel()
        {
            CustomerTypeList = new List<GenericModel>();
            TransportTypeList = new List<GenericModel>();
            CustomerStatusList = new List<GenericModel>();
            PriorityTypeList = new List<GenericModel>();
            PickupAddresses = new List<AddressModel>();
            DropAddresses = new List<AddressModel>();

            PackageSizeList = new List<GenericModel>();

        }

        
        public bool NeedVanOrPickUp { get; set; }
        public int PickupAddressId { get; set; }
        public int DropAddressId { get; set; }

        public List<AddressModel> PickupAddresses { get; set; }
        public List<AddressModel> DropAddresses { get; set; } 

        public List<GenericModel> CustomerTypeList { get; set; }
        public List<GenericModel> TransportTypeList { get; set; }
        public List<GenericModel> CustomerStatusList { get; set; }
        public List<GenericModel> PriorityTypeList { get; set; }


        public IEnumerable<GenericModel> PackageSizeList { get; set; }

        public double TotalCharge { get; set; }

        public double Distance { get; set; }
        public int CustomerId { get; set; }
        public GenericModel PackageSize { get; set; }
        public int PackageSizeId { get; set; }
        public int TransportTypeId { get; set; } 

        public string PickupPictureUri { get; set; }

         public String PickupPhone { get; set; }

       
        public String PickupContact { get; set; }

      
        public String PickupStreet { get; set; }
       
        public String PickupCity { get; set; }
        public String PickupState { get; set; }
        public String PickupCountry { get; set; }
        
        public String PickupZipCode { get; set; }
        public Double PickupLatitude { get; set; }
        public Double PickupLongitude { get; set; }


        public decimal? Weight { get; set; }

        
        public String DeliveryPhone { get; set; }
        
        public String DeliveryContact { get; set; }

      
        public String DeliveryStreet { get; set; }
      
        public String DeliveryCity { get; set; }
        public String DeliveryState { get; set; }
        public String DeliveryCountry { get; set; }
        
        public String DeliveryZipCode { get; set; }
        public Double DeliveryLatitude { get; set; }
        public Double DeliveryLongitude { get; set; }

        public List<Generic> PickupAddressesList { get; set; }

        /// <summary>
        /// Shipment
        /// </summary>
        public string IdentityCode { get; set; }
        public GenericModel PriorityType { get; set; }
        public int PriorityTypeLevel { get; set; }

       
        public decimal Amount { get; set; }
       
        public decimal ShippingWeight { get; set; }
        public string PromoCode { get; set; }

        public int PriorityTypeId { get; set; }

       
        public string Note { get; set; }

        public Decimal ExtraCharge { get; set; }
        public string ExtraChargeNote { get; set; }
         
    }

}
