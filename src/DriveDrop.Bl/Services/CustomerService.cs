using ApplicationCore.Entities.ClientAgregate;
using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
using AutoMapper;
using DriveDrop.Bl.Data;
using DriveDrop.Bl.Extensions;
using DriveDrop.Bl.Infrastructure;
using DriveDrop.Bl.Models;
using DriveDrop.Bl.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Bl.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly DriveDropContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IPictureService _pictureService;
        public CustomerService(IHostingEnvironment env, DriveDropContext context, IMapper mapper,
            IOptionsSnapshot<AppSettings> settings, IPictureService pictureService)
        {
            _settings = settings;
            _context = context;
            _env = env;
            _mapper = mapper;
            _pictureService = pictureService;
        }
        public async  Task<CustomerIndex> Get(int? customertype, int? statusId, int? transporTypeId, string LastName = "", int pageIndex = 0, int pageSize = 10)
        {
            var root = _context.Customers.OrderBy(i => i.Id).Where(x => x.Isdeleted == false);

            if (customertype.HasValue)
            {
                root = root.OrderBy(t => t.CustomerTypeId).Where(ci => ci.CustomerTypeId == customertype);
            }
            if (statusId.HasValue)
            {
                root = root.OrderBy(c => c.CustomerStatusId).Where(ci => ci.CustomerStatus.Id == statusId);
            }

            if (transporTypeId.HasValue)
            {
                root = root.OrderBy(t => t.TransportTypeId).Where(ci => ci.TransportType.Id == transporTypeId);
            }
            if (LastName != "null")
            {
                root = root.OrderBy(n => n.LastName).Where(l => l.LastName.Contains(LastName));
                //root = root.Where(l => l.FirstName.Contains(LastName));
            }

            var totalItems = await root
                .LongCountAsync();

            var itemsOnPage = await root
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .Include(x => x.CustomerStatus)
                .Include(y => y.CustomerType)
                .Include(z => z.TransportType)
                .Include(x => x.DefaultAddress)
                .ToListAsync();


            var model = new PaginatedItemsViewModel<Customer>(
               pageIndex, pageSize, totalItems, itemsOnPage);



            var vm = new CustomerIndex()
            {
                CustomerList = _mapper.Map<List<Customer>, List<CustomerModel>>(model.Data.ToList()),
                TypeFilterApplied = customertype,
                StatusFilterApplied = statusId,
                TransportFilterApplied = transporTypeId,
                LastName = LastName,
                PaginationInfo = new PaginationInfo()
                {
                    ActualPage = pageIndex,
                    ItemsPerPage = model.Data.Count(),
                    TotalItems = (int)model.Count,
                    TotalPages = int.Parse(Math.Ceiling(((decimal)model.Count / pageSize)).ToString())
                }
            };


            var dataItems = await _context.CustomerTypes.Select(x => (new SelectListItem { Value = x.Id.ToString(), Text = x.Name })).ToListAsync();
            var items = new List<SelectListItem>
            {
                new SelectListItem() { Value = null, Text = "All Customers", Selected = true }
            };

            foreach (var itm in dataItems)
            {
                items.Add(new SelectListItem()
                {
                    Value = itm.Value,
                    Text = itm.Text
                });
            }

            vm.CustomerType = items;


            var cStatus = await _context.CustomerStatuses.Select(x => (new SelectListItem { Value = x.Id.ToString(), Text = x.Name })).ToListAsync();
            var csitems = new List<SelectListItem>
            {
                new SelectListItem() { Value = null, Text = "All Status", Selected = true }
            };
            foreach (var itm in cStatus)
            {
                csitems.Add(new SelectListItem()
                {
                    Value = itm.Value,
                    Text = itm.Text
                });
            }
            vm.CustomerStatus = csitems;

            var tTypes = await _context.TransportTypes.OrderBy(x => x.Name).Select(x => (new SelectListItem { Value = x.Id.ToString(), Text = x.Name })).ToListAsync();
            var tTitems = new List<SelectListItem>
            {
                new SelectListItem() { Value = null, Text = "All Transport", Selected = true }
            };
            foreach (var itm in tTypes)
            {
                items.Add(new SelectListItem()
                {
                    Value = itm.Value,
                    Text = itm.Text
                });
            }

            vm.TransportType = tTypes;

           

            //vm.CustomerType = await _context.CustomerTypes.Select(x => (new SelectListItem { Value = x.Id.ToString(), Text = x.Name })).ToListAsync();
            // vm.CustomerStatus = await _context.CustomerStatuses.Select(x => (new SelectListItem { Value = x.Id.ToString(), Text = x.Name })).ToListAsync();
            // vm.TransportType = await _context.TransportTypes.Select(x => (new SelectListItem { Value = x.Id.ToString(), Text = x.Name })).ToListAsync();


            vm.PaginationInfo.Next = (vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1) ? "is-disabled" : "";
            vm.PaginationInfo.Previous = (vm.PaginationInfo.ActualPage == 0) ? "is-disabled" : "";

            //return View(vm);


            return vm;

        }
        public async Task<bool> EndImpersonated(string adminUser)
        {
            var admin = await _context.Customers.Where(x => x.UserName == adminUser).FirstOrDefaultAsync();
            if (admin.CustomerTypeId != 1)
                return false;
             
            admin.EndImpersonate();
            _context.Update(admin);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> IsImpersonate(string adminUser)
        {
            var isImpersonated = await _context.Customers.Where(c => c.UserName == adminUser && c.CustomerTypeId == 1).FirstOrDefaultAsync();
            if (isImpersonated == null)
                return false;
            if (string.IsNullOrWhiteSpace(isImpersonated.UserNameToImpersonate))
                return false;

            return true;
        }
        public async Task<bool> SetImpersonate(string adminUser, string userName)
        {
            var admin = await _context.Customers.Where(x => x.UserName == adminUser).FirstOrDefaultAsync();
            if (admin.CustomerTypeId != 1)
                return false;

            var user = await _context.Customers.Where(x => x.UserName == userName).FirstOrDefaultAsync(); ;
            if (user.CustomerTypeId == 1)
                return false;

            admin.Impersonate(userName);
            _context.Update(admin);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<CurrentCustomerModel> Get(int customerId)
        {
            var cc = await GetCustomerById(customerId);
            if (cc.IsImpersonated)
            {
                cc = await Get(cc.UserNameToImpersonate);
            }
            return cc;
        }
        public async Task<CurrentCustomerModel> Get(string user)
    {
            var c = await _context.Customers

                .Include(x=>x.ShipmentDrivers)
                .ThenInclude(p => p.PriorityType)

                 .Include(x => x.ShipmentDrivers)
                .ThenInclude(p => p.PackageSize)
                
                 .Include(x => x.ShipmentDrivers)
                .ThenInclude(p => p.Reviews)                
                
                .Include(x => x.ShipmentSenders)
                .ThenInclude(p => p.PriorityType)
                
                .Include(x => x.ShipmentSenders)
                .ThenInclude(ps => ps.PackageSize)

                .Include(x => x.ShipmentSenders)
                .ThenInclude(ps => ps.Reviews)
                 .Include(x => x.ShipmentSenders)
                .ThenInclude(ps => ps.Driver)
                .Include(x => x.ShipmentSenders)
                .ThenInclude(ps => ps.PackageStatusHistories)
                .ThenInclude(ps => ps.ShippingStatus)
                .Where(x => x.UserName == user && x.Isdeleted==false)
                .FirstOrDefaultAsync();
            if (c == null )
                return new CurrentCustomerModel();

            var cc = await GetCustomerById(c.Id);

            if (cc.IsImpersonated)
            {
                cc = await Get(cc.UserNameToImpersonate);
            }
            
            return cc;

        }
        public async Task<CurrentCustomerModel> Get(string user, int customerId)
        {
            var cc = await GetCustomerById(customerId);

            
            if (cc.UserName == user)
                cc.IsValid = true;
            else
                cc.IsValid = false;

            if (cc.IsImpersonated)
            {
                cc.IsValid = true;
                cc = await Get(cc.UserNameToImpersonate);
            }


            return cc;

        }
        public async Task<CurrentCustomerModel> GetCustomerById(int customerId)
            {
            var canBeUnImpersonate = false;
            //check for impersonated
            var isImpersonated = await _context.Customers
                .Where(x => x.Id == customerId&&x.Isdeleted==false)
                .FirstOrDefaultAsync();

            if (!string.IsNullOrWhiteSpace(isImpersonated.UserNameToImpersonate) 
                && !string.IsNullOrEmpty(isImpersonated.UserNameToImpersonate))
            {
                var iUser= await _context.Customers
                .Where(x => x.UserName == isImpersonated.UserNameToImpersonate)
                .FirstOrDefaultAsync();

                //user impersonated?
                if (iUser != null)
                {
                    customerId = iUser.Id;
                    canBeUnImpersonate = true;
                }
            }

            //alway bring the impersonated user

            var c = await _context.Customers
                  .Include(s => s.TransportType)
                 .Include(t => t.CustomerStatus)
                 .Include(s => s.CustomerType)
                 .Include(a => a.Addresses)
                 .Include(d=>d.DefaultAddress)
                 .Include("ShipmentDrivers.ShippingStatus")
                 .Include("ShipmentDrivers.PickupAddress")
                 .Include("ShipmentDrivers.DeliveryAddress")
                 .Include("ShipmentDrivers.PriorityType")
                 .Include("ShipmentDrivers.PackageSize")
                 .Include("ShipmentSenders.ShippingStatus")
                 .Include("ShipmentSenders.PickupAddress")
                 .Include("ShipmentSenders.DeliveryAddress")
                 .Include("ShipmentSenders.PriorityType")
                 .Include("ShipmentSenders.PackageSize")
                 
                .Where(x =>  x.Id == customerId && x.Isdeleted == false)
                .FirstOrDefaultAsync();

            if (c == null )
                return new CurrentCustomerModel();

            var currentCustomer = new CurrentCustomerModel
            {
                Id = c.Id,
                UserName = c.UserName,
                PersonalPhotoUri = c.PersonalPhotoUri,
                VerificationId = c.VerificationId,
                CustomerStatus = c.CustomerStatus.Name,
                CustomerTypeId = c.CustomerTypeId??0,
                CustomerStatusId = c.CustomerStatusId ?? 0,
                CustomerType = c.CustomerType.Name,

                IsDeleted = c.Isdeleted,
                    Email =c.Email,
                    FirstName=c.FirstName,
                    LastName =c.LastName,
                    IdentityGuid=c.IdentityGuid,
                    MaxPackage = c.MaxPackage ?? 0,
                    Phone = c.Phone,
                    UserGuid = c.UserName,
                    PrimaryPhone = c.PrimaryPhone,

                PickupRadius = c.PickupRadius,
                ShipmentDrivers = _mapper.Map<List<Shipment>, List<ShipmentModel>>( c.ShipmentDrivers.ToList()),
                ShipmentSenders = _mapper.Map<List<Shipment>, List<ShipmentModel>>(c.ShipmentSenders.ToList()),
                DeliverRadius = c.DeliverRadius,

                Commission = c.Commission,

                UserNameToImpersonate = c.UserNameToImpersonate,

                DriverLincensePictureUri = c.DriverLincensePictureUri,
               
                InsurancePhotoUri = c.InsurancePhotoUri,
                VehiclePhotoUri =c.VehiclePhotoUri,
                VehicleMake=c.VehicleMake,
                VehicleModel=c.VehicleModel,
                VehicleColor=c.VehicleColor,
                VehicleYear=c.VehicleYear
            };
            if(c.DefaultAddress!=null)
            currentCustomer.DefaultAddress = _mapper.Map<Address,AddressModel>(c.DefaultAddress);
            currentCustomer.ShipmentDrivers = _mapper.Map<List<Shipment>, List<ShipmentModel>>(c.ShipmentDrivers.ToList());
            currentCustomer.ShipmentSenders = _mapper.Map<List<Shipment>, List<ShipmentModel>>(c.ShipmentSenders.ToList());
            currentCustomer.Addresses = _mapper.Map<List<Address>, List<AddressModel>>(c.Addresses.ToList());
            
            if (c.TransportType != null) {
                currentCustomer.TransportType = c.TransportType.Name;
                currentCustomer.TransportTypeId = c.TransportTypeId ?? 0;
                }


            if (c.CustomerTypeId == 1)
                currentCustomer.IsAdmin = true;

            currentCustomer.IsValid = true;

            if (canBeUnImpersonate)
            { 
                currentCustomer.CanBeUnImpersonate = true;
            }


            if (string.IsNullOrWhiteSpace(currentCustomer.PersonalPhotoUri))
                currentCustomer.PersonalPhotoUri = "profile-icon.png";

            if (string.IsNullOrWhiteSpace(currentCustomer.DriverLincensePictureUri))
                currentCustomer.DriverLincensePictureUri = "profile-icon.png";

            if (string.IsNullOrWhiteSpace(currentCustomer.VehiclePhotoUri))
                currentCustomer.VehiclePhotoUri = "profile-icon.png";


            if (string.IsNullOrWhiteSpace(currentCustomer.InsurancePhotoUri))
                currentCustomer.InsurancePhotoUri = "profile-icon.png";

            currentCustomer.CustomerType = currentCustomer.CustomerType.ToTitleCase();
            currentCustomer.CustomerStatus = currentCustomer.CustomerStatus.ToTitleCase();


            currentCustomer.PersonalPhotoUri = _pictureService.DisplayImage(currentCustomer.PersonalPhotoUri);
            currentCustomer.DriverLincensePictureUri = _pictureService.DisplayImage(currentCustomer.DriverLincensePictureUri);
            currentCustomer.VehiclePhotoUri = _pictureService.DisplayImage(currentCustomer.VehiclePhotoUri);
            currentCustomer.InsurancePhotoUri = _pictureService.DisplayImage(currentCustomer.InsurancePhotoUri);





            return currentCustomer;
        }
    }
}
