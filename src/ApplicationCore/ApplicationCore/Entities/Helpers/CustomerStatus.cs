using ApplicationCore.SeedWork;
using ApplicationCore.Execeptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.ClientAgregate.ShipmentAgregate
{
    public class CustomerStatus
         : Enumeration
    {
        public static CustomerStatus WaitingApproval = new CustomerStatus(1, nameof(WaitingApproval).ToLowerInvariant());
        public static CustomerStatus Active = new CustomerStatus(2, nameof(Active).ToLowerInvariant());
        public static CustomerStatus Suspended = new CustomerStatus(3, nameof(Suspended).ToLowerInvariant());
        public static CustomerStatus Canceled = new CustomerStatus(4, nameof(Canceled).ToLowerInvariant());


        protected CustomerStatus()
        {
        }

        public CustomerStatus(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<CustomerStatus> List()
        {
            return new[] { WaitingApproval, Active, Suspended, Canceled };
        }

        public static CustomerStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new DriveDropException($"Possible values for ClientType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static CustomerStatus From(int id)
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

