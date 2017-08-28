using ApplicationCore.SeedWork;
using ApplicationCore.Execeptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.ClientAgregate.ShipmentAgregate
{
    public class TransportType
         : Enumeration
    {
        public static TransportType Sedan2 = new TransportType(1, "2-door sedan");
        public static TransportType Sedan4 = new TransportType(2, "4-door sedan");
        public static TransportType HatchBack = new TransportType(3, "HatchBack");
        public static TransportType Van = new TransportType(4, "Van");
        public static TransportType PickUp = new TransportType(5, "PickUp");
        public static TransportType Bike = new TransportType(6, "Bike");

        protected TransportType()
        {
        }

        public TransportType(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<TransportType> List()
        {
            return new[] { Sedan2 , Sedan4 , HatchBack , Van , PickUp , Bike };
        }

        public static TransportType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new DriveDropException($"Possible values for TransportType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static TransportType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new DriveDropException($"Possible values for TransportType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}

