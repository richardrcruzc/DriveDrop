﻿using GoDriveDrop.Core.Models.Commons;
using GoDriveDrop.Core.Models.Shippments;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoDriveDrop.Core.Models.Commons
{
    public class CustomerModel 
    {
        //public CustomerModel()
        //{
        //    Addresses = new List<AddressModel>();
        //    ShipmentDrivers = new List<ShipmentModel>();
        //    ShipmentSenders = new List<ShipmentModel>();

        //}

        public bool CanBeUnImpersonate { get; set; }
        public string UserNameToImpersonate { get; set; }
        public int Id { get; set; }
        public string IdentityGuid { get; set; }
        public string UserGuid { get; set; }
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsDeleted { get; set; }


         public string VerificationId { get; set; }
           public string Email { get; set; }

           public string Phone { get; set; }
            public string PrimaryPhone { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
        public string CustomerType { get; set; }
        public int CustomerTypeId { get; set; }
        public int TransportTypeId { get; set; }
        public string TransportType { get; set; }
        public int CustomerStatusId { get; set; }
        public string CustomerStatus { get; set; }
        public int MaxPackage { get; set; }
        public double? PickupRadius { get; set; }
          public double? DeliverRadius { get; set; }


        public string VehicleInfo { get; set; }

        public decimal Commission { get; set; }

        public List<AddressModel> Addresses { get; set; }
        public virtual List<ShipmentModel> ShipmentDrivers { get; set; }
        public virtual List<ShipmentModel> ShipmentSenders { get; set; }

        public string DriverLincensePictureUri { get; set; }

        public bool IsValid { get; set; }

        public AddressModel DefaultAddress { get; set; }

        public string PaymentMethodId { get; set; }
        public string PersonalPhotoUri { get; set; }
        public string VehiclePhotoUri { get; set; }
        public string InsurancePhotoUri { get; set; }



        public string VehicleMake { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleColor { get; set; }
        public string VehicleYear { get; set; }


        public string FullName
        {
            get
            {
                return LastName + ", " + FirstName;
            }
        }

        public bool IsImpersonated
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.UserNameToImpersonate);
            }
        }


        public string PersonalPhotoUrl
        {
            get
            {
                return $"{GlobalSetting.Instance.BaseEndpoint}{PersonalPhotoUri}";
            }
        }
        public string VehiclePhotoUrl
        {
            get
            {
                return $"{GlobalSetting.Instance.BaseEndpoint}{VehiclePhotoUri}";
            }
        }
        public string InsurancePhotoUrl
        {
            get
            {
                return $"{GlobalSetting.Instance.BaseEndpoint}{InsurancePhotoUri}";
            }
        }
        public string DriverLincensePictureUrl
        {
            get
            {
                return $"{GlobalSetting.Instance.BaseEndpoint}{DriverLincensePictureUri}";
            }
        }
    }
}
