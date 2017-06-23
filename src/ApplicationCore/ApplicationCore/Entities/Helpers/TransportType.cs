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
        public static TransportType Sedan = new TransportType(1, nameof(Sedan).ToLowerInvariant());
        public static TransportType Van = new TransportType(2, nameof(Van).ToLowerInvariant());
        public static TransportType Pickup = new TransportType(3, nameof(Pickup).ToLowerInvariant());
        public static TransportType LightTruck = new TransportType(4, nameof(LightTruck).ToLowerInvariant());
        public static TransportType Motocycle = new TransportType(5, nameof(Motocycle).ToLowerInvariant());
        public static TransportType Bicycle = new TransportType(6, nameof(Bicycle).ToLowerInvariant());

        protected TransportType()
        {
        }

        public TransportType(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<TransportType> List()
        {
            return new[] { Sedan, Van, Pickup, LightTruck, Motocycle, Bicycle };
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

