using ApplicationCore.Execeptions;
using ApplicationCore.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.ClientAgregate.ShipmentAgregate
{
    public class PriorityType
         : Enumeration
    {
        public static PriorityType Asap = new PriorityType(1, nameof(Asap).ToLowerInvariant());
        public static PriorityType Hours = new PriorityType(2, nameof(Hours).ToLowerInvariant());
        public static PriorityType Days = new PriorityType(3, nameof(Days).ToLowerInvariant());

        protected PriorityType()
        {
        }

        public PriorityType(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<PriorityType> List()
        {
            return new[] { Asap, Hours, Days };
        }

        public static PriorityType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new DriveDropException($"Possible values for DeliveryType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static PriorityType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new DriveDropException($"Possible values for DeliveryType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}

