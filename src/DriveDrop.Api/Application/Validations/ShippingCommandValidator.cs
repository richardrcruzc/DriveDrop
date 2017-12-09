using FluentValidation;
using DriveDrop.Api.Application.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.Application.Validations
{
    public class ShippingCommandValidator : AbstractValidator<ShipCustomerCommand>
    {
        public ShippingCommandValidator()
        {
            RuleFor(order => order.ShippingNumber).NotEmpty().WithMessage("No shippingId found");
        }
    }
}
