namespace Microsoft.eShopOnContainers.Services.DriveDrop.Api.Application.Commands
{
    using ApplicationCore.Entities.ClientAgregate;
    using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
    using global::DriveDrop.Api.Infrastructure.Idempotency;
    using global::DriveDrop.Api.Infrastructure.Services;
    using MediatR; 
    using System;
    using System.Threading.Tasks;


    public class CreateOrderCommandIdentifiedHandler : IdentifierCommandHandler<CreateShippingCommand, bool>
    {
        public CreateOrderCommandIdentifiedHandler(IMediator mediator, IRequestManager requestManager) : base(mediator, requestManager)
        {
        }

        protected override bool CreateResultForDuplicateRequest()
        {
            return true;                // Ignore duplicate requests for creating order.
        }
    }

    public class CreateCustomerCommandHandler
        : IAsyncRequestHandler<CreateShippingCommand, bool>
    {
        private readonly IShipmentRepository _sRepository;
        private readonly IIdentityService _identityService;
        private readonly IMediator _mediator;

        // Using DI to inject infrastructure persistence Repositories
        public CreateCustomerCommandHandler(IMediator mediator, IShipmentRepository orderRepository, IIdentityService identityService)
        {
            _sRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(CreateShippingCommand message)
        {

            // Add/Update the Buyer AggregateRoot
            // DDD patterns comment: Add child entities and value-objects through the Order Aggregate-Root
            // methods and constructor so validations, invariants and business logic 
            // make sure that consistency is preserved across the whole aggregate
           // var address = new Address(message.Street, message.City, message.State, message.Country, message.ZipCode );
            //var order = new Order(message.UserId, address, message.CardTypeId, message.CardNumber, message.CardSecurityNumber, message.CardHolderName, message.CardExpiration);

            //foreach (var item in message.OrderItems)
            //{
            //    order.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice, item.Discount, item.PictureUrl, item.Units);
            //}

            //_sRepository.Add(order);

            return await _sRepository.UnitOfWork
                .SaveEntitiesAsync();
        }
    }
}
