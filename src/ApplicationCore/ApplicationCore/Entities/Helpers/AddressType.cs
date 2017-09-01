using ApplicationCore.Execeptions;
using ApplicationCore.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.ClientAgregate.ShipmentAgregate
{
    public class AddressType
         : Enumeration
    {
        public static AddressType Billing = new AddressType(1, "Billing");
        public static AddressType Pickup = new AddressType(2, "Pickup");
        public static AddressType Delivery = new AddressType(3,"Delivery");

        protected AddressType()
        {
        }

        public AddressType(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<AddressType> List()
        {
            return new[] { Billing, Pickup, Delivery };
        }

        public static AddressType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new DriveDropException($"Possible values for AddressType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static AddressType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new DriveDropException($"Possible values for AddressType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}

