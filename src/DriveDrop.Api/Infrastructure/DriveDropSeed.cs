using ApplicationCore.Entities.ClientAgregate;
using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
using ApplicationCore.Entities.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ApplicationCore.Entities.Helpers.HelperTable;

namespace DriveDrop.Api.Infrastructure
{

    public class DriveDropSeed
    {

        public static async Task SeedAsync(IApplicationBuilder applicationBuilder, ILoggerFactory loggerFactory, int? retry = 0)
        {
            int retryForAvailability = retry.Value;
            try
            {
                var context = (DriveDropContext)applicationBuilder
                .ApplicationServices.GetService(typeof(DriveDropContext));


                if (!context.CustomerStatuses.Any())
                {
                    context.CustomerStatuses.Add(CustomerStatus.Active);
                    context.CustomerStatuses.Add(CustomerStatus.Canceled);
                    context.CustomerStatuses.Add(CustomerStatus.WaitingApproval);
                    context.CustomerStatuses.Add(CustomerStatus.Suspended);
                    await context.SaveChangesAsync();
                }


                if (!context.AddressTypes.Any())
                {
                    context.AddressTypes.Add(AddressType.Billing);
                    context.AddressTypes.Add(AddressType.Delivery);
                    context.AddressTypes.Add(AddressType.Pickup);
                    await context.SaveChangesAsync();
                }

                //if (!context.CardTypes.Any())
                //{
                //    context.CardTypes.Add(CardType.Amex);
                //    context.CardTypes.Add(CardType.Visa);
                //    context.CardTypes.Add(CardType.MasterCard);
                //    await context.SaveChangesAsync();
                //}

                if (!context.CustomerTypes.Any())
                {
                    context.CustomerTypes.Add(CustomerType.Administrator);
                    context.CustomerTypes.Add(CustomerType.Driver);
                    context.CustomerTypes.Add(CustomerType.Sender);
                    await context.SaveChangesAsync();
                }
                if (!context.PriorityTypes.Any())
                {
                    context.PriorityTypes.Add(PriorityType.Asap);
                    context.PriorityTypes.Add(PriorityType.Days);
                    context.PriorityTypes.Add(PriorityType.Hours);
                    await context.SaveChangesAsync();
                }
                if (!context.ShippingStatuses.Any())
                {
                    context.ShippingStatuses.Add(ShippingStatus.PendingPickUp);
                    context.ShippingStatuses.Add(ShippingStatus.Pickup);
                    context.ShippingStatuses.Add(ShippingStatus.DeliveryInProcess);
                    context.ShippingStatuses.Add(ShippingStatus.Delivered);
                    context.ShippingStatuses.Add(ShippingStatus.Canceled);
                    await context.SaveChangesAsync();
                }
                if (!context.TransportTypes.Any())
                {
                    context.TransportTypes.Add(TransportType.Sedan);
                    context.TransportTypes.Add(TransportType.Van);
                    context.TransportTypes.Add(TransportType.Pickup);
                    context.TransportTypes.Add(TransportType.LightTruck);
                    context.TransportTypes.Add(TransportType.Motocycle);
                    context.TransportTypes.Add(TransportType.Motocycle);
                    context.TransportTypes.Add(TransportType.Bicycle);
                    await context.SaveChangesAsync();
                }




                if (!context.Customers.Any())
                {

                    context.Customers.Add(new Customer("Admin1", "Admin", "Admin", TransportType.Sedan.Id, CustomerStatus.WaitingApproval.Id,"123213123","W@S.com",1));

                    context.Customers.Add(new Customer("Sender1", "First", "Sender", TransportType.Sedan.Id, CustomerStatus.Active.Id, "123213123", "W@S.com", 2));
                    context.Customers.Add(new Customer("Sender2", "Second", "Sender", TransportType.Sedan.Id, CustomerStatus.Active.Id, "123213123", "W@S.com", 2));
                    context.Customers.Add(new Customer("Sender3", "Third", "Sender", TransportType.Sedan.Id, CustomerStatus.WaitingApproval.Id, "123213123", "W@S.com", 2));
                    context.Customers.Add(new Customer("Sender4", "Forth", "Sender", TransportType.Sedan.Id, CustomerStatus.Suspended.Id, "123213123", "W@S.com", 2));
                    context.Customers.Add(new Customer("Sender5", "Fith", "Sender", TransportType.Sedan.Id, CustomerStatus.Canceled.Id, "123213123", "W@S.com", 2));


                    context.Customers.Add(new Customer("Driver", "Fisrt", "Driver", TransportType.Sedan.Id, CustomerStatus.WaitingApproval.Id, "123213123", "W@S.com", 3));
                    context.Customers.Add(new Customer("Driver", "Last", "Driver", TransportType.Sedan.Id, CustomerStatus.Active.Id, "123213123", "W@S.com", 3));

                    await context.SaveChangesAsync();
                }



                if (!context.Shipments.Any())
                {

                    var serder = context.Customers.Where(x =>  x.CustomerTypeId == 2).FirstOrDefault();
                     

                    var addressPickup = new Address("5215 90th st E", "WA", "Tacoma", "USA", "98446", "1234567890", "Contact one", 0, 0);
                    var addressDelivery = new Address("5215 25th ave se", "WA", "Lacey", "USA", "98503", "1234567890", "Contact two", 0, 0);

                    context.Shipments.Add(new Shipment(addressPickup, addressDelivery, serder, 20, 2, PriorityType.Asap.Id, 4, TransportType.Van.Id, "This is closed gate community", "", ""));
                    context.Shipments.Add(new Shipment(addressPickup, addressDelivery, serder, 20, 2, PriorityType.Hours.Id, 6, TransportType.Van.Id, "package left behain door", "", ""));
                    context.Shipments.Add(new Shipment(addressPickup, addressDelivery, serder, 20, 2, PriorityType.Asap.Id, 4, TransportType.Van.Id, "ring the bell", "", ""));
                    await context.SaveChangesAsync();
                }

                //if (!context.ShipmentCustomers.Any())
                //{

                //    var addressPickup = new Address("5215 90th st E", "WA", "Tacoma", "USA", "98446","1234567890","Contact one", 0, 0);
                //    var addressDelivery = new Address("5215 25th ave se", "WA", "Lacey", "USA", "98503", "1234567890", "Contact two", 0, 0);

                //    var customer = context.Customers.Where(x => x.FirstName == "First" && x.CustomerTypeId == 2).FirstOrDefault();
                //    var shipment = context.Shipments.Where(c => c.Note == "This is closed gate community").FirstOrDefault();

                //    context.ShipmentCustomers.Add(new ShipmentCustomer { CustomerId = customer.Id, CustomerTypeId = CustomerType.Sender.Id, ShipmentId = shipment.Id });
                //    await context.SaveChangesAsync();
                //    customer = context.Customers.Where(x => x.FirstName == "Last" && x.CustomerTypeId == 3).FirstOrDefault();

                //    context.ShipmentCustomers.Add(new ShipmentCustomer { CustomerId = customer.Id, CustomerTypeId = CustomerType.Driver.Id, ShipmentId = shipment.Id });
                //    await context.SaveChangesAsync();

                //    context.ShipmentAddresses.Add(new ShipmentAddress { AddressTypeId = AddressType.Pickup.Id, Address = addressPickup, ShipmentId = shipment.Id });
                //    context.ShipmentAddresses.Add(new ShipmentAddress { AddressTypeId = AddressType.Delivery.Id, Address = addressDelivery, ShipmentId = shipment.Id });

                //    await context.SaveChangesAsync();

                //}




                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (retryForAvailability < 10)
                {
                    retryForAvailability++;
                    var log = loggerFactory.CreateLogger("drivedrop seed");
                    log.LogError(ex.Message);
                    await SeedAsync(applicationBuilder, loggerFactory, retryForAvailability);
                }
            }
        }
    }
}
    
 
