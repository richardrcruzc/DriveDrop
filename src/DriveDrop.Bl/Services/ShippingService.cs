using ApplicationCore.Entities.ClientAgregate;
using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
using AutoMapper;
using DriveDrop.Bl.Data;
using DriveDrop.Bl.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Bl.Services
{
    public class ShippingService: IShippingService
    {
        private readonly IPictureService _ps;
        private readonly DriveDropContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IRateService _rateService;

        public ShippingService(DriveDropContext context, IHostingEnvironment env, IMapper mapper, IRateService rateService,
             IPictureService ps)
        {
            _ps = ps;
            _context = context;
            _env = env;
            _mapper = mapper;
            _rateService = rateService;
        } 

        public async Task<ShipmentModel> GetById(int id)
        {
            var root = await _context.Shipments
            .Where(x => x.Id == id)
            .OrderBy(i => i.Id)
            .Include(d => d.DeliveryAddress)
            .Include(d => d.PickupAddress)
            .Include(d => d.ShippingStatus)
            .Include(d => d.PriorityType)
              .Include(d => d.Reviews)
              .Include(d => d.Sender)
              .Include(d => d.Driver)
              .Include(d => d.PackageSize)
              .Include(d => d.PackageStatusHistories)
              .ThenInclude(d => d.ShippingStatus)


              .FirstOrDefaultAsync();

            return _mapper.Map<Shipment, ShipmentModel>(root);
        }

        public async Task<string> UpdateDriver(int id, int driverId)
        {

            var shipping = await _context.Shipments.FindAsync(id);
            if (shipping != null)
            {
                var driver = _context.Customers.OrderBy(i => i.Id).Where(c => c.Id == driverId).FirstOrDefault();
                if (driver != null)
                {
                    shipping.SetDriver(driver);
                    _context.Update(shipping);
                    await _context.SaveChangesAsync();
                }
            }
            return "updated";
        }

        public async Task<string> SetDropbyInfo(DropbyInfoModel model)
        {
            var shipping = _context.Shipments.Find(model.Id);

            if (shipping == null)
                return  "PackageNoFound";
            if (model.StatusId > 0)
                shipping.ChangeStatus(model.StatusId);
            else
            {
                shipping.ChangeStatus(ShippingStatus.Delivered.Id);

            }
            shipping.SetDropInfo(System.Net.WebUtility.UrlDecode(model.DropPictureUri), model.DropComment);
            _context.Update(shipping);
            await _context.SaveChangesAsync();

            return  "SetDeliveredPictureUri";
        }

        public async Task<string> SetDeliveredPictureUri(AcceptModel model)
        { 
            var shipping = _context.Shipments.Find(model.Id);

            if (shipping == null)
                return "PackageNoFound";
            if (model.StatusId > 0)
                shipping.ChangeStatus(model.StatusId);
            else
            {
                shipping.ChangeStatus(ShippingStatus.DeliveryInProcess.Id);

            }
            shipping.SetDeliveredPictureUri(System.Net.WebUtility.UrlDecode(model.fileName));
            _context.Update(shipping);
            await _context.SaveChangesAsync();

            return "SetDeliveredPictureUri";
        }
        public async Task<string> UpdatePackageStatus(int shippingId, int shippingStatusId)
        {
            var shipping = await _context.Shipments.OrderBy(i => i.Id).Where(x => x.Id == shippingId).FirstOrDefaultAsync();
            if (shipping != null)
            {
                shipping.ChangeStatus(shippingStatusId); 
                _context.Update(shipping);
                await _context.SaveChangesAsync();

            }
            return "updated";
        }
        public async Task<PaginatedItemsViewModel<ShipmentModel>> GetShippingByDriverId(int id, int pageSize = 10, int pageIndex = 0)
        {


            var root = _context.Shipments
          .Where(x => x.DriverId == id).OrderBy(x => x.DriverId)
          .Include(d => d.DeliveryAddress)
          .Include(d => d.PickupAddress)
          .Include(d => d.ShippingStatus)
          .Include(d => d.PriorityType);

            var totalItems = await root
             .LongCountAsync();

            var itemsOnPage = await root
           .Skip(pageSize * pageIndex)
           .Take(pageSize)
           .ToListAsync();

            var query = _mapper.Map<List<Shipment>, List<ShipmentModel>>(itemsOnPage.ToList());

           // itemsOnPage = ChangeUriPlaceholder(itemsOnPage);

            var model = new PaginatedItemsViewModel<ShipmentModel>(
                pageIndex, pageSize, totalItems, query);



            return model;

        }
        public async Task<PaginatedItemsViewModel<ShipmentModel>> GetByDriverIdAndStatusId(int id, int[] statusId, int pageSize = 10, int pageIndex = 0)
        {
            var root = _context.Shipments
              .OrderBy(d => d.DriverId)
          .Where(x => x.DriverId == id && statusId.Contains(x.ShippingStatusId))
          .Include(d => d.DeliveryAddress)
          .Include(d => d.PickupAddress)
          .Include(d => d.ShippingStatus)
          .Include(d => d.PriorityType)
          .Include(s => s.Reviews);


            var totalItems = await root
             .LongCountAsync();

            var itemsOnPage = await root
           .Skip(pageSize * pageIndex)
           .Take(pageSize)
           .ToListAsync();

            var query = _mapper.Map<List<Shipment>, List<ShipmentModel>>(itemsOnPage.ToList());

            //   itemsOnPage = ChangeUriPlaceholder(itemsOnPage);

            var model = new PaginatedItemsViewModel<ShipmentModel>(
                pageIndex, pageSize, totalItems, query);



            return model;
        }
        public async Task<List<ShipmentModel>> GetShippingByCustomerId(int id)
        {

            var query = await _context.Shipments
                   .OrderBy(s => s.SenderId)
              .Where(x => x.SenderId == id)
              .Include(d => d.DeliveryAddress)
              .Include(d => d.PickupAddress)
              .Include(d => d.ShippingStatus)
              .Include(d => d.PriorityType)
              .Include(s => s.Reviews)
              .OrderByDescending(x => x.Id)
              .ToListAsync();

            var model = _mapper.Map<List<Shipment>, List<ShipmentModel>>(query.ToList());

            return model;

        }
        public async Task<ShippingIndex> GetShipping(int shippingStatusId, int priorityTypeId, string identityCode, int pageSize = 10, int pageIndex = 0)
        {
            var root = _context.Shipments
           .Where(x => x.Id > 0);

            if (shippingStatusId > 0)
                root = root.Where(x => x.ShippingStatusId == shippingStatusId);

            if (priorityTypeId > 0)
                root = root.Where(x => x.PriorityTypeId == priorityTypeId);

            if (identityCode != null)
                root = root.Where(x => x.IdentityCode == identityCode);

            var totalItems = await root
             .LongCountAsync();

            var query = await root
                .Include(d => d.DeliveryAddress)
           .Include(d => d.PickupAddress)
           .Include(d => d.ShippingStatus)
           .Include(d => d.PriorityType)
           .Include(s => s.Sender)
           .Include(s => s.Driver)
             .Include(s => s.Reviews)
           .OrderBy(x => x.ShippingCreateDate).ThenBy(x => x.PriorityTypeId)
           .Skip(pageSize * pageIndex)
           .Take(pageSize)
           .ToListAsync();

            var itemsOnPage = _mapper.Map<List<Shipment>, List<ShipmentModel>>(query.ToList());

            // itemsOnPage = ChangeUriPlaceholder(itemsOnPage);

            var tt1 = new SelectListItem { Text = "All Priorities", Value = "0" };
            var tt2 = new SelectListItem { Text = "All Status", Value = "0" };

            var shippingStatu = new List<SelectListItem>();
            var priorityType = new List<SelectListItem>();

            shippingStatu.Add(tt2);
            priorityType.Add(tt1);

            shippingStatu.AddRange(await _context.ShippingStatuses.OrderBy(x => x.Name).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToListAsync());
            priorityType.AddRange(await _context.PriorityTypes.OrderBy(x => x.Name).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToListAsync());


            //var model = new PaginatedItemsViewModel<ShipmentModel>(
            //    pageIndex, pageSize, totalItems, itemsOnPage, shippingStatu, priorityType, null);


            var vm = new ShippingIndex()
            {
                ShippingList = itemsOnPage,
                PriorityTypeFilterApplied = priorityTypeId,
                ShippingStatusFilterApplied = priorityTypeId,
                PriorityType = priorityType,
                ShippingStatus = shippingStatu,

                PaginationInfo = new PaginationInfo()
                {
                    ActualPage = pageIndex,
                    ItemsPerPage = itemsOnPage.Count(),
                    TotalItems = (int)itemsOnPage.Count,
                    TotalPages = int.Parse(Math.Ceiling(((decimal)itemsOnPage.Count / pageSize)).ToString())
                }

            };
            vm.PaginationInfo.Next = (vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1) ? "is-disabled" : "";
            vm.PaginationInfo.Previous = (vm.PaginationInfo.ActualPage == 0) ? "is-disabled" : "";


            return vm;

        }

        public async Task<PaginatedItemsViewModel<ShipmentModel>> GetPackagesReadyForDriver(int driverId, int pageSize = 10, int pageIndex = 0)
        {
            var driver = _context
                    .Customers
                    .OrderBy(i => i.Id)
                    .Include(x => x.DefaultAddress)
                    .Where(x => x.Id == driverId && x.CustomerType.Id == 3).FirstOrDefault();
            if (driver == null)
                return null;

            var driverActualLat = driver.DefaultAddress.Latitude;
            var driverActualLng = driver.DefaultAddress.Longitude;

            var driverPickupDistance = driver.PickupRadius ?? 0;
            var driverDelivertDistance = driver.DeliverRadius ?? 0;

            var ready = new List<Shipment>();


            var root = await _context.Shipments
                .OrderBy(d => d.DriverId)
           .Where(x => x.ShippingStatusId == ShippingStatus.NoDriverAssigned.Id
            && x.DriverId == null && x.Distance <= driverDelivertDistance)
           .Include(d => d.DeliveryAddress)
           .Include(d => d.PickupAddress)
           .Include(d => d.ShippingStatus)
           .Include(d => d.PriorityType)
           .ToListAsync();

            //root = root.Where(d=>d.Distance<=driver.PickupRadius)
            foreach (var dir in root)
            {

                var pickUpLat = dir.PickupAddress.Latitude;
                var pickUpLng = dir.PickupAddress.Longitude;

                var deliveryLat = dir.DeliveryAddress.Latitude;
                var deliverypLng = dir.DeliveryAddress.Longitude;

                
                var pickupDistance = DistanceAlgorithm.DistanceBetweenPlaces(driverActualLng, driverActualLat, pickUpLng, pickUpLat);

                var deliveryDistance = driverDelivertDistance;
                if (deliverypLng != 0 && deliveryLat != 0)
                    deliveryDistance = DistanceAlgorithm.DistanceBetweenPlaces(pickUpLng, pickUpLat, deliverypLng, deliveryLat);

                if (pickupDistance > driverPickupDistance || deliveryDistance > driverDelivertDistance)
                    continue; 

                ready.Add(dir);

            } 


            var totalItems = ready
             .LongCount();

            var query  = ready
           .Skip(pageSize * pageIndex)
           .Take(pageSize)
           .ToList();

            var itemsOnPage = _mapper.Map<List<Shipment>, List<ShipmentModel>>(query.ToList());

            foreach (var s in itemsOnPage)
            {
                s.PickupPictureUri = _ps.DisplayImage(s.PickupPictureUri);
                s.DeliveredPictureUri = _ps.DisplayImage(s.DeliveredPictureUri);
                s.DropPictureUri = _ps.DisplayImage(s.DropPictureUri);
            }

            var model = new PaginatedItemsViewModel<ShipmentModel>(
                pageIndex, pageSize, totalItems, itemsOnPage); 


            return model;

        }
        public async Task<PaginatedItemsViewModel<ShipmentModel>> GetNotAssignedShipping(int pageSize = 10, int pageIndex = 0)
        {
            var root = _context.Shipments
                 .OrderBy(d => d.DriverId)
            .Where(x => x.ShippingStatusId == ShippingStatus.PendingPickUp.Id && x.DriverId == null)
            .Include(d => d.DeliveryAddress)
            .Include(d => d.PickupAddress)
            .Include(d => d.ShippingStatus)
            .Include(d => d.PriorityType);

            var totalItems = await root
             .LongCountAsync();

            var query = await root
           .Skip(pageSize * pageIndex)
           .Take(pageSize)
           .ToListAsync();

            var itemsOnPage = _mapper.Map<List<Shipment>, List<ShipmentModel>>(query.ToList());

            var model = new PaginatedItemsViewModel<ShipmentModel>(
                pageIndex, pageSize, totalItems, itemsOnPage); 

            return model;


        }
        public async Task<int> SaveNewShipment(NewShipment c)
        { 
            var sender = _context.Customers.Find(c.CustomerId);


            var deliveryAddres = new Address(c.DeliveryStreet, c.DeliveryCity, c.DeliveryState, c.DeliveryCountry, c.DeliveryZipCode, c.DeliveryPhone, c.DeliveryContact, c.DeliveryLatitude, c.DeliveryLongitude, "drop");
            var pickUpAddres = new Address(c.PickupStreet, c.PickupCity, c.PickupState, c.PickupCountry, c.PickupZipCode, c.PickupPhone, c.PickupContact, c.PickupLatitude, c.PickupLongitude, "pickup");

            var tmpUser = Guid.NewGuid().ToString();


            // var rate = await _rateService.CalculateAmount(int.Parse(c.PickupZipCode), int.Parse(c.DeliveryZipCode), c.ShippingWeight, c.Quantity, c.PriorityTypeId, c.TransportTypeId, c.PromoCode);
            var rate = await _rateService.CalculateAmount(c.Distance, c.ShippingWeight??0, c.PriorityTypeId, c.PromoCode, c.PackageSizeId, c.ExtraCharge ?? 0, c.ExtraChargeNote, c.PickupState, c.PickupCity);


            var test = rate.AmountToCharge + c.ExtraCharge ?? 0;

            rate.AmountToCharge += c.ExtraCharge ?? 0;

            var shipment = new Shipment(pickup: pickUpAddres, delivery: deliveryAddres, sender: sender, amount: c.Amount??0, discount: rate.Discount,
                shippingWeight: c.ShippingWeight??0, priorityTypeId: c.PriorityTypeId, transportTypeId: c.TransportTypeId, note: c.Note, pickupPictureUri: c.PickupPictureUri, deliveredPictureUri: "",
                distance: rate.Distance, chargeAmount: rate.AmountToCharge, promoCode: c.PromoCode, tax: rate.TaxAmount, packageSizeId: c.PackageSizeId, extraCharge: c.ExtraCharge ?? 0, extraChargeNote: c.ExtraChargeNote, needaVanOrPickup: c.NeedaVanOrPickup);

            _context.Add(shipment);

            _context.SaveChanges();

            await _context.SaveChangesAsync();


            sender.AddAddress(deliveryAddres);
            sender.AddAddress(pickUpAddres);

            _context.Update(sender);

            await _context.SaveChangesAsync();

            return shipment.Id;
        }

    }
}
