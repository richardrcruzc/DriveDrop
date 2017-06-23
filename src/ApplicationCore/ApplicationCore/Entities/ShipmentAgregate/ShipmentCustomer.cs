//using ApplicationCore.Entities.Helpers;
//using ApplicationCore.SeedWork;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace ApplicationCore.Entities.ClientAgregate.ShipmentAgregate
//{
//    public class ShipmentCustomer : Entity
//    {
//        public int ShipmentId { get; set; }
//        public int CustomerId { get; set; }
//        public int CustomerTypeId { get; set; }

//        public Shipment Shipment { get; set; }
//        public  Customer Customer { get;  set; }       
//        public HelperTable CustomerType { get; set; }



//        public bool IsEqualTo(int shipmentId,int customerId,  int customerTypeId )
//        {
//            return ShipmentId == shipmentId
//                && CustomerId == customerId
//                && CustomerTypeId == customerTypeId;
//        }

//    }
//}
