using MediatR; 
using Microsoft.Extensions.Logging;
using DriveDrop.Api.Application.IntegrationEvents;
using DriveDrop.Api.Application.IntegrationEvents.Events; 
using System;
using System.Threading.Tasks;
using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
using ApplicationCore.Events;

namespace DriveDrop.Api.Application.DomainEventHandlers.BuyerAndPaymentMethodVerified
{
    public class UpdateShippingWhenCustomerAndPaymentMethodVerifiedDomainEventHandler 
                   : IAsyncNotificationHandler<CustomerAndPaymentMethodVerifiedDomainEvent>
    {
        private readonly IShipmentRepository _sRepository;        
        private readonly ILoggerFactory _logger;        

        public UpdateShippingWhenCustomerAndPaymentMethodVerifiedDomainEventHandler(
            IShipmentRepository orderRepository, ILoggerFactory logger)            
        {
            _sRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));            
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Domain Logic comment:
        // When the Buyer and Buyer's payment method have been created or verified that they existed, 
        // then we can update the original Order with the BuyerId and PaymentId (foreign keys)
        public async Task Handle(CustomerAndPaymentMethodVerifiedDomainEvent buyerPaymentMethodVerifiedEvent)
        {
            var orderToUpdate = await _sRepository.GetAsync(buyerPaymentMethodVerifiedEvent.ShippingId);
            //orderToUpdate.SetBuyerId(buyerPaymentMethodVerifiedEvent.Customer.Id);
            //orderToUpdate.SetPaymentId(buyerPaymentMethodVerifiedEvent.Payment.Id);                                                

            _logger.CreateLogger(nameof(UpdateShippingWhenCustomerAndPaymentMethodVerifiedDomainEventHandler))
                .LogTrace($"Shipping with Id: {buyerPaymentMethodVerifiedEvent.ShippingId} has been successfully updated with a payment method id: { buyerPaymentMethodVerifiedEvent.Payment.Id }");                        
        }
    }  
}
