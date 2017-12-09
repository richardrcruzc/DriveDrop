using FluentValidation;
using Microsoft.eShopOnContainers.Services.DriveDrop.Api.Application.Commands;

namespace DriveDrop.Api.Application.Validations
{
    public class IdentifierCommandValidator : AbstractValidator<IdentifiedCommand<CreateShippingCommand,bool>>
    {
        public IdentifierCommandValidator()
        {
            RuleFor(command => command.Id).NotEmpty();    
        }
    }
}
