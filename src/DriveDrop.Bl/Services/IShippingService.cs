
using DriveDrop.Bl.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Bl.Services
{
    public interface IShippingService
    {
        Task<ShipmentModel> GetById(int id);
        Task<string> UpdateDriver(int id, int driverId);
        Task<string> SetDropbyInfo(DropbyInfoModel model);
        Task<string> SetDeliveredPictureUri(AcceptModel model);
        Task<string> UpdatePackageStatus(int shippingId, int shippingStatusId);
        Task<PaginatedItemsViewModel<ShipmentModel>> GetShippingByDriverId(int id, int pageSize = 10, int pageIndex = 0);
        Task<PaginatedItemsViewModel<ShipmentModel>> GetByDriverIdAndStatusId(int id, int[] statusId, int pageSize = 10, int pageIndex = 0);
        Task<List<ShipmentModel>> GetShippingByCustomerId(int id);
        Task<ShippingIndex> GetShipping(int shippingStatusId, int priorityTypeId, string identityCode, int pageSize = 10, int pageIndex = 0);
        Task<PaginatedItemsViewModel<ShipmentModel>> GetPackagesReadyForDriver(int driverId, int pageSize = 10, int pageIndex = 0);
        Task<PaginatedItemsViewModel<ShipmentModel>> GetNotAssignedShipping(int pageSize = 10, int pageIndex = 0);
        Task<int> SaveNewShipment(NewShipment c);
       
    }
}
