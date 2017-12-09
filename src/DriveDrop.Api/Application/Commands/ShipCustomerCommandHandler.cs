using MediatR;
using Microsoft.eShopOnContainers.Services.DriveDrop.Api.Application.Commands; 
using DriveDrop.Api.Infrastructure.Idempotency;
using System.Threading.Tasks;
using ApplicationCore.Entities.ClientAgregate;


namespace DriveDrop.Api.Application.Commands
{
    public class ShipCustomerCommandIdentifiedHandler : IdentifierCommandHandler<ShipCustomerCommand, bool>
    {
        public ShipCustomerCommandIdentifiedHandler(IMediator mediator, IRequestManager requestManager) : base(mediator, requestManager)
        {
        }

        protected override bool CreateResultForDuplicateRequest()
        {
            return true;                // Ignore duplicate requests for processing order.
        }
    }

    public class ShipCustomerCommandHandler : IAsyncRequestHandler<ShipCustomerCommand, bool>
    {        
        private readonly ICustomerRepository _customerRepository;

        public ShipCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        /// <summary>
        /// Handler which processes the command when
        /// administrator executes ship order from app
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<bool> Handle(ShipCustomerCommand command)
        {
            var orderToUpdate = await _customerRepository.GetAsync(command.ShippingNumber);
            //orderToUpdate.SetShippedStatus();
            return await _customerRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
