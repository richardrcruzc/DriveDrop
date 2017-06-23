using ApplicationCore.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.ClientAgregate.ShipmentAgregate
{

    //This is just the RepositoryContracts or Interface defined at the Domain Layer
    //as requisite for the Shipment Aggregate

    public interface IShipmentRepository : IRepository<Shipment>
    {
        Shipment Add(Shipment shipment);

        Shipment Update(Shipment shipment);

        Task<Shipment> GetAsync(int shipmentId);

        Task<Shipment> GetShipments(int pageIndex, int itemsPage, int? type, int? status, int? transport);
    }
}
