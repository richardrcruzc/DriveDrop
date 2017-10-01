using DriveDrop.Api.Infrastructure;
using DriveDrop.Api.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly DriveDropContext _context;
        private readonly IHostingEnvironment _env;
        
        public CustomerService(IHostingEnvironment env, DriveDropContext context)
        {
            _context = context;
            _env = env;

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

                .Where(x => x.UserName == user || x.Isdeleted==false)
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
                CustomerTypeId = c.CustomerTypeId,
                CustomerStatusId = c.CustomerStatusId,
                CustomerType = c.CustomerType.Name,

                IsDeleted = c.Isdeleted,
                    Email =c.Email,
                    FirstName=c.FirstName,
                    LastName =c.LastName,
                    IdentityGuid=c.IdentityGuid,
                    MaxPackage = c.MaxPackage,
                    Phone = c.Phone,
                    UserGuid = c.UserName,
                    PrimaryPhone = c.PrimaryPhone,

                PickupRadius = c.PickupRadius,
                ShipmentDrivers =c.ShipmentDrivers,
                ShipmentSenders =c.ShipmentSenders,
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
            currentCustomer.DefaultAddress = new AddressModel(c.DefaultAddress.Id, currentCustomer.Id, c.DefaultAddress.Street, c.DefaultAddress.City, c.DefaultAddress.State, c.DefaultAddress.Country, c.DefaultAddress.ZipCode, c.DefaultAddress.Phone, c.DefaultAddress.Contact, c.DefaultAddress.Longitude, c.DefaultAddress.Longitude, c.DefaultAddress.TypeAddress) ;
            currentCustomer.ShipmentDrivers = c.ShipmentDrivers;
            currentCustomer.ShipmentSenders = c.ShipmentSenders;
            currentCustomer.Addresses =  c.Addresses.Select(x=> new AddressModel(x.Id, currentCustomer.Id, x.Street, x.City, x.State, x.Country, x.ZipCode, x.Phone, x.Contact, x.Longitude, x.Longitude, x.TypeAddress  )).ToList();
            if (c.TransportType != null) {
                currentCustomer.TransportType = c.TransportType.Name;
                currentCustomer.TransportTypeId = c.TransportTypeId;
                }


            if (c.CustomerTypeId == 1)
                currentCustomer.IsAdmin = true;

            currentCustomer.IsValid = true;

            if (canBeUnImpersonate)
            { 
                currentCustomer.CanBeUnImpersonate = true;
            }

            return currentCustomer;
        }
    }
}
