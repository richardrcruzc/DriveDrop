using FluentValidation;
using DriveDrop.Api.Application.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.Application.Validations
{
    public class CancelShippingCommandValidator : AbstractValidator<CancelShippingCommand>
    {
        public CancelShippingCommandValidator()
        {
            RuleFor(order => order.OrderNumber).NotEmpty().WithMessage("No shippingId found");
        }
    }
}
