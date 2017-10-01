using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Web.Infrastructure
{
    public static class API
    {
        public static class Identity
        {
            public static string RegisterUser(string baseUri, string userName, string password)
            { 
                return $"{baseUri}RegisterUser?userName={userName}&password={password}";
            }

            public static string ChangePassword(string baseUri, string Email, string OldPassword, string NewPassword, string ConfirmPassword)
            {
                return $"{baseUri}ChangePassword?Email={Email}&OldPassword={OldPassword}&NewPassword={NewPassword}&ConfirmPassword={ConfirmPassword}";
            }

            
        }
        public static class Tax
        {
            public static string Get(string baseUri)
            {
                return $"{baseUri}GetTaxes";
            }
            public static string GetTax(string baseUri, int id)
            {
                var idQs = id.ToString();
                var filterQs = $"{idQs}";
                return $"{baseUri}GetTax/{filterQs}";
            }
            public static string SaveTax(string baseUri)
            {
                return $"{baseUri}SaveTax";
            }
            public static string DeleteTax(string baseUri, int id)
            {
                var idQs = id.ToString();
                var filterQs = $"{idQs}";
                return $"{baseUri}DeleteTax/{filterQs}";
            }
        }
            public static class Rate
        {
            public static string Amount(string baseUri, decimal distance, decimal weight,   int priority,int packageSizeId,  string promoCode, decimal extraCharge, string extraChargeNote, string pickupState, string pickupCity)
            {
                var packageSizeIdQs = packageSizeId.ToString();
                var distanceQs = distance.ToString();
                var weightQs = weight.ToString(); 
                var priorityQs = priority.ToString();  
                    var pcQs = promoCode;

                var filterQs = $"distance={distanceQs}&weight={weightQs}&priority={priorityQs}&packageSizeId={packageSizeIdQs}&promoCode={pcQs}&extraCharge={extraCharge}&extraChargeNote={extraChargeNote}&pickupState={pickupState}&pickupCity={pickupCity}";
                             
                return $"{baseUri}CalculateAmount?{filterQs}";
            }
            public static string Get(string baseUri) { 
                return $"{baseUri}Get";
            }
            public static string GetbyId(string baseUri, int id)
            {
                var idQs = id.ToString();
                var filterQs = $"{idQs}";
                return $"{baseUri}get/{filterQs}";
            }

            public static string SaveRate(string baseUri)
            {
                return $"{baseUri}Save";
            }
            public static string NewRate(string baseUri)
            {
                return $"{baseUri}New";
            }
            public static string DeleteDetail(string baseUri)
            {
                return $"{baseUri}DeleteDetail";
            }
            public static string DeleteRate(string baseUri, int id)
            {
                var idQs = id.ToString();
                var filterQs = $"{idQs}";
                return $"{baseUri}DeleteRate/{filterQs}";
            } 

            public static string Details(string baseUri)
            {
                return $"{baseUri}Details";
            }
            public static string DetailSave(string baseUri)
            {
                return $"{baseUri}DetailSave";
            }


        }
        public static class Admin
        {
            public static string ChangeCustomerStatus(string baseUri, int customerId, int statusId )
            {
                var customerIdQs = customerId.ToString();
                var statusIdQs = statusId.ToString();

                var filterQs = $"/customerId/{customerIdQs}/statusId/{statusIdQs}";

                return $"{baseUri}/ChangeCustomerStatus{filterQs}";
            }

            public static string GetAllCustomers(string baseUri, int pageIndex, int pageSize, int? status, int? type, int? transporType, string LastName = null)
            {
                var filterQs = "";

                //if (status.HasValue || type.HasValue || transporType.HasValue)
                //{
                    var statusQs = (status.HasValue) ? status.Value.ToString() : "null";
                    var typeQs = (type.HasValue) ? type.Value.ToString() : "null";
                    var transporTypeQs = (transporType.HasValue) ? transporType.Value.ToString() : "null";
                    var LastNameQs = LastName!=null ? LastName.Trim():"null";
                    filterQs = $"/type/{typeQs}/status/{statusQs}/transporType/{transporTypeQs}/LastName/{LastNameQs}";
              //  }

                return $"{baseUri}/Items{filterQs}?pageIndex={pageIndex}&pageSize={pageSize}";
            }

            public static string GetbyId(string baseUri, int id)
            {
                var idQs =  id.ToString()  ;
                var filterQs = $"/{idQs}";
                return $"{baseUri}/GetbyId{filterQs}";
            }
            public static string GetbyUserName(string baseUri, string userName)
            { 
                return $"{baseUri}/GetbyUserName/{userName}";
            }
            public static string SetImpersonate(string baseUri, string adminUser, string userName)
            {
                return $"{baseUri}/SetImpersonate/adminUser/{adminUser}/userName/{userName}";
            }

            public static string EndImpersonated(string baseUri, string adminUser)
            {
                return $"{baseUri}/EndImpersonated/adminUser/{adminUser}";
            }

        }
        public static class Sender
        {
            public static string GetByUserName(string baseUri, string userName )
            { 

                return $"{baseUri}/GetByUserName/{userName}";
            }


            public static string GetSender(string baseUri, string userName, int customerId)
            {
                var userNameIdQs = userName.ToString();
                var customerIdQs = customerId.ToString();

                var filterQs = $"/userName/{userNameIdQs}/customerId/{customerIdQs}";

                return $"{baseUri}/GetSender{filterQs}";
            }

            public static string GetbyId(string baseUri, int id)
            {
                var idQs = id.ToString();
                var filterQs = $"/{idQs}";
                return $"{baseUri}/GetbyId{filterQs}";
            }

            public static string SaveNewShipment(string baseUri)
            {
                return $"{baseUri}/SaveNewShipment";
            }
            public static string NewSender(string baseUri)
            {
                return $"{baseUri}/NewSender";
            }

           
            public static string UpdateInfo(string baseUri)
            {
                return $"{baseUri}/UpdateInfo";
            }

        }
        public static class Driver
        {
            public static string AutoComplete(string baseUri, string values)
            {
                return $"{baseUri}/AutoComplete/{values}";
            }

            public static string GetByUserName(string baseUri, string userName)
            {
                return $"{baseUri}/GetByUserName/{userName}";
            }

            public static string GetDriver(string baseUri, string userName, int customerId)
            {
                var userNameIdQs = userName.ToString();
                var customerIdQs = customerId.ToString();

                var filterQs = $"/userName/{userNameIdQs}/customerId/{customerIdQs}";

                return $"{baseUri}/GetDriver{filterQs}";
            }
            public static string GetbyId(string baseUri, int id)
            {
                var idQs = id.ToString();
                var filterQs = $"/{idQs}";
                return $"{baseUri}/GetbyId{filterQs}";
            }

            public static string NewDriver(string baseUri)
            {
                return $"{baseUri}/NewDriver";
            }
             public static string AssignDriver(string baseUri, int id, int shippingId)
            {
                var idQs = id.ToString(); 
                var shippingIdQs = shippingId.ToString();
                var filterQs = $"/Customer/{idQs}/shipping/{shippingIdQs}";

                return $"{baseUri}/AssignDriver{filterQs}";
            }

            public static string UpdateInfo(string baseUri)
            {
                return $"{baseUri}/UpdateInfo";
            }
             public static string AddAddress(string baseUri)
            {
                return $"{baseUri}/AddAddress";
            }

        }

        public static class Shipping
        {
            

            public static string GetById(string baseUri, int id)
            {

                var idQs = id.ToString();
                var filterQs = $"GetById/shippingid/{idQs}";
                return $"{baseUri}/{filterQs}";
            }

            public static string GetShipping(string baseUri,   int shippingStatusId, int priorityTypeId, string identityCode, int pageIndex, int pageSize)
            {
                var shippingStatusIdQs = shippingStatusId.ToString();
                var priorityTypeIdQs = priorityTypeId.ToString();
                var identityCodeQs = string.IsNullOrEmpty(identityCode) ? "null" : identityCode;
                var filterQs = $"/shippingStatusId/{shippingStatusIdQs}/priorityTypeId/{priorityTypeIdQs}/identityCode/{identityCodeQs}?pageIndex={pageIndex}&pageSize={pageSize}";

                return $"{baseUri}/GetShipping{filterQs}";
            }
            
                public static string GetPackagesReadyForDriver(string baseUri, int id)
            {
                var idQs = id.ToString();
                var filterQs = $"/driverId/{idQs}";

                return $"{baseUri}/GetPackagesReadyForDriver{filterQs}";
            }
            public static string GetNotAssignedShipping(string baseUri)
            {
                return $"{baseUri}/GetNotAssignedShipping";
            }
            public static string GetShippingByCustomerId(string baseUri, int id)
            {

                var idQs = id.ToString();
                var filterQs = $"/{idQs}";
                return $"{baseUri}/GetShippingByCustomerId{filterQs}"; 
            }

            public static string GetShippingByDriverId(string baseUri, int id)
            {

                var idQs = id.ToString();
                var filterQs = $"/{idQs}";
                return $"{baseUri}/GetShippingByDriverId{filterQs}";
            }

            public static string GetByDriverIdAndStatusId(string baseUri, int id, int statusId)
            {
                var statusidQs = statusId.ToString();
                var idQs = id.ToString();
                var filterQs = $"/{idQs}/{statusidQs}";
                return $"{baseUri}/GetByDriverIdAndStatusId{filterQs}";
            }

            public static string UpdatePackageStatus(string baseUri, int shippingId, int shippingStatusId)
            {
                var shippingStatusIdQs = shippingStatusId.ToString();
                var idQs = shippingId.ToString();
                var filterQs = $"/shippingId/{idQs}/shippingStatusId/{shippingStatusIdQs}";
                return $"{baseUri}/UpdatePackageStatus{filterQs}";
            }

        }

        public static class Common
        {
            public static string DeleteCustomer(string baseUri, int id)
            {
                var idQs = id.ToString();
                
                return $"{baseUri}DeleteCustomer/id/{idQs}";
            }

            public static string WelcomeEmail(string baseUri, string userName)
            {
                return $"{baseUri}WelcomeEmail/userName/{userName}";
            }
             
            public static string PostFiles(string baseUri )
            {
                return $"{baseUri}PostFiles";
            }

            public static string GetAllCardTypes(string baseUri)
            {
                return $"{baseUri}CardTypes";
            }
            public static string GetAllCustomerStatus(string baseUri)
            {
                return $"{baseUri}CustomerStatuses";
            }
            public static string GetAllCustomerTypes(string baseUri)
            {
                return $"{baseUri}CustomerTypes";
            }
            public static string GetAllPriorityTypes(string baseUri)
            {
                return $"{baseUri}PriorityTypes";
            }
            public static string GetAllShippingStatus(string baseUri)
            {
                return $"{baseUri}ShippingStatuses";
            }
            public static string GetAllTransportTypes(string baseUri)
            {
                return $"{baseUri}TransportTypes";
            }
            public static string GetAllPackageSizes(string baseUri)
            {
                return $"{baseUri}PackageSizes";
            }

            public static string ValidateUserName(string baseUri,string  userName)
            {
                return $"{baseUri}ValidateUserName/{userName}"; 
            }

            public static string IsAdmin(string baseUri, string userName)
            {
                return $"{baseUri}IsAdmin/{userName}";
            } 
            public static string IdIsUser(string baseUri, string userName, int id)
            { 
                var idQs = id.ToString(); 

                return $"{baseUri}IdIsUser/user/{userName}/id/{idQs}";
            }

            public static string GetUser(string baseUri, string userName)
            {
                return $"{baseUri}GetUser/user/{userName}";
            }

            
            public static string DeleteAddress(string baseUri, int id, int addressid)
            {
                var idQs = id.ToString();
                var addressidQs = addressid.ToString();

                return $"{baseUri}DeleteAddress/customerId/{idQs}/addressid/{addressidQs}";
            }

            public static string DefaultAddress(string baseUri, int id, int addressid)
            {
                var idQs = id.ToString();
                var addressidQs = addressid.ToString();

                return $"{baseUri}DefaultAddress/customerId/{idQs}/addressid/{addressidQs}";
            }


            public static string AddAddress(string baseUri)
            {
                return $"{baseUri}AddAddress";
            }

           

            
        }
        public static class Rating
        {
            public static string InitializeReview(string baseUri, int  shippingId)
            { 
                return $"{baseUri}InitializeReview/shippingId/{shippingId}";
            }

            public static string GetReviewByShippingId(string baseUri, int? shippingId)
            {
                var shippingIdQs = (shippingId.HasValue) ? shippingId.Value.ToString() : "null"; 

                return $"{baseUri}GetReviewByShippingId/shippingId/{shippingIdQs}";
            }

            public static string GetAllReviews(string baseUri, int pageIndex, int pageSize, int? senderId, int? driverId, int? published, int? reviewAppliedTo, int? shippingId)
            {
                if (pageSize <= 0)
                    pageSize = 10;

                if (pageIndex <= 0)
                    pageIndex = 0;


                var filterQs = "";

                //if (status.HasValue || type.HasValue || transporType.HasValue)
                //{
                var senderIdQs = (senderId.HasValue) ? senderId.Value.ToString() : "null";
                var driverIdQs = (driverId.HasValue) ? driverId.Value.ToString() : "null";
                var publishedQs = (published.HasValue) ? published.Value.ToString() : "null";
                var reviewAppliedToQs = reviewAppliedTo != null ? reviewAppliedTo.Value.ToString() : "null";
                var shippingIdQs = shippingId != null ? shippingId.Value.ToString() : "null";
                filterQs = $"/senderId/{senderIdQs}/driverId/{driverIdQs}/published/{publishedQs}/reviewAppliedTo/{reviewAppliedToQs}/shippingId/{shippingIdQs}";
                //  }

                return $"{baseUri}Items{filterQs}?pageIndex={pageIndex}&pageSize={pageSize}";

                
            }

            public static string AddReviews(string baseUri)
            {
                return $"{baseUri}add";
            }

            public static string Publish(string baseUri, int? reviewId, int? published)
            {
                var reviewIdQs = (reviewId.HasValue) ? reviewId.Value.ToString() : "null";
                var publishedQs = (published.HasValue) ? published.Value.ToString() : "null";

                return $"{baseUri}Add/reviewId/{reviewIdQs}/published/{publishedQs}";
            }
        }
        }
}
