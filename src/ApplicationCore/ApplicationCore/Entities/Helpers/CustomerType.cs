using ApplicationCore.SeedWork;
using ApplicationCore.Execeptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.ClientAgregate.ShipmentAgregate
{
    public class CustomerType
         : Enumeration
    {
        public static CustomerType Administrator = new CustomerType(1, nameof(Administrator).ToLowerInvariant());
        public static CustomerType Sender = new CustomerType(2, nameof(Sender).ToLowerInvariant());
        public static CustomerType Driver = new CustomerType(3, nameof(Driver).ToLowerInvariant());


        protected CustomerType()
        {
        }

        public CustomerType(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<CustomerType> List()
        {
            return new[] { Sender, Driver, Administrator };
        }

        public static CustomerType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new DriveDropException($"Possible values for ClientType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static CustomerType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new DriveDropException($"Possible values for ClientType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}

