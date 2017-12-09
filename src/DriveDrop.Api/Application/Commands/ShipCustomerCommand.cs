using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace DriveDrop.Api.Application.Commands
{
    public class ShipCustomerCommand : IRequest<bool>
    {

        [DataMember]
        public int ShippingNumber { get; private set; }

        public ShipCustomerCommand(int shippingNumber)
        {
            ShippingNumber = shippingNumber;
        }
    }
}