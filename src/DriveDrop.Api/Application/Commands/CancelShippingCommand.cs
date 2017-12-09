using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace DriveDrop.Api.Application.Commands
{
    public class CancelShippingCommand : IRequest<bool>
    {

        [DataMember]
        public int OrderNumber { get; private set; }

        public CancelShippingCommand(int orderNumber)
        {
            OrderNumber = orderNumber;
        }
    }
}
