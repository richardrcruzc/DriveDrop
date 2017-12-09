using FluentValidation;
using Microsoft.eShopOnContainers.Services.DriveDrop.Api.Application.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.eShopOnContainers.Services.DriveDrop.Api.Application.Commands.CreateShippingCommand;

namespace DriveDrop.Api.Application.Validations
{
    public class CreateShippingCommandValidator : AbstractValidator<CreateShippingCommand>
    {
        public CreateShippingCommandValidator()
        {
            RuleFor(command => command.City).NotEmpty();
            RuleFor(command => command.Street).NotEmpty();
            RuleFor(command => command.State).NotEmpty();
            RuleFor(command => command.Country).NotEmpty();
            RuleFor(command => command.ZipCode).NotEmpty();
            RuleFor(command => command.CardNumber).NotEmpty().Length(12, 19); 
            RuleFor(command => command.CardHolderName).NotEmpty();
            RuleFor(command => command.CardExpiration).NotEmpty().Must(BeValidExpirationDate).WithMessage("Please specify a valid card expiration date"); 
            RuleFor(command => command.CardSecurityNumber).NotEmpty().Length(3); 
            RuleFor(command => command.CardTypeId).NotEmpty();
            RuleFor(command => command.OrderItems).Must(ContainOrderItems).WithMessage("No order items found"); 
        }

        private bool BeValidExpirationDate(DateTime dateTime)
        {
            return dateTime >= DateTime.UtcNow;
        }

        private bool ContainOrderItems(IEnumerable<OrderItemDTO> orderItems)
        {
            return orderItems.Any();
        }
    }
}
