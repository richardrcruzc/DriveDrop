using ApplicationCore.Entities.ClientAgregate;
using ApplicationCore.Events;
using DriveDrop.Api.Infrastructure.Services;
using MediatR; 
using Microsoft.Extensions.Logging; 
using System;
using System.Threading.Tasks;

namespace DriveDrop.Api.Application.DomainEventHandlers.OrderStartedEvent
{
    public class ValidateOrAddCustomerAggregateWhenShippingStartedDomainEventHandler 
                        : IAsyncNotificationHandler<ShippingStartedDomainEvent>
    {
        private readonly ILoggerFactory _logger;
        private readonly ICustomerRepository _customerRepository;
        private readonly IIdentityService _identityService;

        public ValidateOrAddCustomerAggregateWhenShippingStartedDomainEventHandler(ILoggerFactory logger, ICustomerRepository buyerRepository, IIdentityService identityService)
        {
            _customerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(ShippingStartedDomainEvent sStartedEvent)
        {
            var cardTypeId = (sStartedEvent.CardTypeId != 0) ? sStartedEvent.CardTypeId : 1;

            var customer = await _customerRepository.FindAsync(sStartedEvent.UserId);
            bool buyerOriginallyExisted = (customer == null) ? false : true;

            if (!buyerOriginallyExisted)
            {
                customer = new Customer(sStartedEvent.UserId);
            }

            customer.VerifyOrAddPaymentMethod(cardTypeId,
                                           $"Payment Method on {DateTime.UtcNow}",
                                           sStartedEvent.CardNumber,
                                           sStartedEvent.CardSecurityNumber,
                                           sStartedEvent.CardHolderName,
                                           sStartedEvent.CardExpiration,
                                           sStartedEvent.Shipment.Id);

            var buyerUpdated = buyerOriginallyExisted ? _customerRepository.UpdateAsync(customer) : _customerRepository.AddAsync(customer);

            await _customerRepository.UnitOfWork
                .SaveEntitiesAsync();

            _logger.CreateLogger(nameof(ValidateOrAddCustomerAggregateWhenShippingStartedDomainEventHandler)).LogTrace($"Buyer {buyerUpdated.Id} and related payment method were validated or updated for shippingId: {sStartedEvent.Shipment.Id}.");
        }
    }
}
