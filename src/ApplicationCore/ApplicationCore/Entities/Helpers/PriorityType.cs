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
        public static PriorityType Asap = new PriorityType(1, "ASAP");
        public static PriorityType FourHours = new PriorityType(2, "Four Hours");
        public static PriorityType SixHours = new PriorityType(3,"Six Hours");
        public static PriorityType EODSameDay = new PriorityType(4,"EOD Same Day");
        public static PriorityType EODNextDay = new PriorityType(5, "EOD Next Day");

        protected PriorityType()
        {
        }

        public PriorityType(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<PriorityType> List()
        {
            return new[] { Asap, FourHours, SixHours, EODSameDay, EODNextDay };
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

