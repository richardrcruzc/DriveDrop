﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Web.Infrastructure
{
    public static class API
    {
        public static class Admin
        {
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

        }
        public static class Sender
        {
            public static string GetbyId(string baseUri, int id)
            {
                var idQs = id.ToString();
                var filterQs = $"/{idQs}";
                return $"{baseUri}/GetbyId{filterQs}";
            }

            public static string SaveNewShipment(string baseUri)
            {
                return $"{baseUri}SaveNewShipment";
            }
        }
        public static class Driver
        {
        }

        public static class Shipping
        {
            public static string GetNotAssignedShipping(string baseUri)
            {
                return $"{baseUri}GetNotAssignedShipping";
            }
            public static string GetShippingByCustomerId(string baseUri, int id)
            {

                var idQs = id.ToString();
                var filterQs = $"/{idQs}";
                return $"{baseUri}GetShippingByCustomerId{filterQs}"; 
            }

           
        }

        public static class Common
        {

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
        }
    }
}
