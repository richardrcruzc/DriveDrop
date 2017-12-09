using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
using DriveDrop.Api.Infrastructure.Idempotency;
using MediatR;
using Microsoft.eShopOnContainers.Services.DriveDrop.Api.Application.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.Application.Commands
{
    public class CancelShippingCommandIdentifiedHandler : IdentifierCommandHandler<CancelShippingCommand, bool>
    {
        public CancelShippingCommandIdentifiedHandler(IMediator mediator, IRequestManager requestManager) : base(mediator, requestManager)
        {
        }

        protected override bool CreateResultForDuplicateRequest()
        {
            return true;                // Ignore duplicate requests for processing order.
        }
    }

    public class CancelShippingCommandHandler : IAsyncRequestHandler<CancelShippingCommand, bool>
    {
        private readonly IShipmentRepository _sRepository;

        public CancelShippingCommandHandler(IShipmentRepository sRepository)
        {
            _sRepository = sRepository;
        }

        /// <summary>
        /// Handler which processes the command when
        /// customer executes cancel order from app
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<bool> Handle(CancelShippingCommand command)
        {
            var orderToUpdate = await _sRepository.GetAsync(command.OrderNumber);
           // orderToUpdate.SetCancelledStatus();
            return await _sRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
