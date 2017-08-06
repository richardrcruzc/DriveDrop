using ApplicationCore.Entities.ClientAgregate;
using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
using ApplicationCore.Entities.Helpers;
using DriveDrop.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
 


namespace DriveDrop.Api.Infrastructure
{

    public class DriveDropSeed
    {

        public static async Task SeedAsync(IApplicationBuilder applicationBuilder, ILoggerFactory loggerFactory, IHostingEnvironment _env, IRateService rate, IDistanceService distance, int? retry = 0)
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
                    context.PriorityTypes.Add(PriorityType.FourHours);
                    context.PriorityTypes.Add(PriorityType.SixHours);
                    context.PriorityTypes.Add(PriorityType.EODSameDay);
                    context.PriorityTypes.Add(PriorityType.EODNextDay);
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

                if (!context.PackageSizes.Any())
                {
                    context.PackageSizes.Add(PackageSize.Envelopes);
                    context.PackageSizes.Add(PackageSize.SmallPackages);
                    context.PackageSizes.Add(PackageSize.MidiunPackages);
                    context.PackageSizes.Add(PackageSize.LargePackages);                    
                    await context.SaveChangesAsync();
                }

                if (!context.Rates.Any())
                {
                    context.Rates.Add(new Rate(DateTime.Now, DateTime.Now.AddDays(3), 8, 4, 12));
                    context.Rates.Add(new Rate(DateTime.Now.AddDays(4), DateTime.Now.AddYears(3), 10, 5, 12));

                    context.SaveChanges();
                }

                if (!context.RateDetails.Any())
                {
                    var first = context.Rates.Where(x => x.Id > 0).FirstOrDefault();
                    if (first != null)
                    {
                        context.RateDetails.Add(new RateDetail(first.Id, "weight", "lbs", 0, 35, 0.55M));

                        context.RateDetails.Add(new RateDetail(first.Id, "distance", "miles", 0, 200, 1M));

                    }
                    var second = context.Rates.Where(x => x.Id > 0).LastOrDefault();
                    if (second != null)
                    {
                        context.RateDetails.Add(new RateDetail(second.Id, "weight", "lbs", 0, 10, 0.55M));
                        context.RateDetails.Add(new RateDetail(second.Id, "weight", "lbs", 10, 15, 0.65M));
                        context.RateDetails.Add(new RateDetail(second.Id, "weight", "lbs", 15, 20, 0.75M));
                        context.RateDetails.Add(new RateDetail(second.Id, "weight", "lbs", 20, 25, 0.85M));
                        context.RateDetails.Add(new RateDetail(second.Id, "weight", "lbs", 25, 30, 0.95M));
                        context.RateDetails.Add(new RateDetail(second.Id, "weight", "lbs", 30, 35, 1M));

                        context.RateDetails.Add(new RateDetail(second.Id, "distance", "miles", 0, 20, 0.07M));
                        context.RateDetails.Add(new RateDetail(second.Id, "distance", "miles", 20, 40, 0.06M));
                        context.RateDetails.Add(new RateDetail(second.Id, "distance", "miles", 40, 60, 0.05M));
                        context.RateDetails.Add(new RateDetail(second.Id, "distance", "miles", 60, 80, 0.04M));
                        context.RateDetails.Add(new RateDetail(second.Id, "distance", "miles", 80, 100, 0.03M));
                        context.RateDetails.Add(new RateDetail(second.Id, "distance", "miles", 100, 120, 0.02M));
                        context.RateDetails.Add(new RateDetail(second.Id, "distance", "miles", 120, 140, 0.01M));

                        await context.SaveChangesAsync();
                    }

                }

                if (!context.RatePriorities.Any())
                {
                    var first = context.Rates.Where(x => x.Id > 0).FirstOrDefault();
                    if (first != null)
                    {
                        context.RatePriorities.Add(new  RatePriority(first.Id, 1,5,false));
                        context.RatePriorities.Add(new RatePriority(first.Id, 2, 3, false));
                        context.RatePriorities.Add(new RatePriority(first.Id, 3, 2, false));
                    }
                    var second = context.Rates.Where(x => x.Id > 0).LastOrDefault();
                    if (second != null)
                    {

                        context.RatePriorities.Add(new RatePriority(second.Id, 1, 10, false));
                        context.RatePriorities.Add(new RatePriority(second.Id, 2, 7, false));
                        context.RatePriorities.Add(new RatePriority(second.Id, 3, 3, false));
                    }
                    await context.SaveChangesAsync();
                }

                if (!context.RateTranportTypes.Any())
                {
                    var first = context.Rates.Where(x => x.Id > 0).FirstOrDefault();
                    if (first != null)
                    {
                        context.RateTranportTypes.Add(new RateTranportType(first.Id, 1, 5, false));
                        context.RateTranportTypes.Add(new RateTranportType(first.Id, 2, 10, false));
                        context.RateTranportTypes.Add(new RateTranportType(first.Id, 3, 15, false));
                        context.RateTranportTypes.Add(new RateTranportType(first.Id, 4, 20, false));
                        context.RateTranportTypes.Add(new RateTranportType(first.Id, 5, 3, false));
                        context.RateTranportTypes.Add(new RateTranportType(first.Id, 6, 2, false));
                    }
                    var second = context.Rates.Where(x => x.Id > 0).LastOrDefault();
                    if (second != null)
                    {

                        context.RateTranportTypes.Add(new RateTranportType(second.Id, 1, 5, false));
                        context.RateTranportTypes.Add(new RateTranportType(second.Id, 2, 10, false));
                        context.RateTranportTypes.Add(new RateTranportType(second.Id, 3, 15, false));
                        context.RateTranportTypes.Add(new RateTranportType(second.Id, 4, 20, false));
                        context.RateTranportTypes.Add(new RateTranportType(second.Id, 5, 3, false));
                        context.RateTranportTypes.Add(new RateTranportType(second.Id, 6, 1, false));
                        
                    }
                    await context.SaveChangesAsync();
                }
                if (!context.RatePackageSizes.Any())
                {
                    var first = context.Rates.Where(x => x.Id > 0).FirstOrDefault();
                    if (first != null)
                    {
                        context.RatePackageSizes.Add(new RatePackageSize(first.Id, 1, 5, false));
                        context.RatePackageSizes.Add(new RatePackageSize(first.Id, 2, 6, false));
                        context.RatePackageSizes.Add(new RatePackageSize(first.Id, 3, 7, false));
                        context.RatePackageSizes.Add(new RatePackageSize(first.Id, 4, 8, false));
                    }
                    var second = context.Rates.Where(x => x.Id > 0).LastOrDefault();
                    if (second != null)
                    {

                        context.RatePackageSizes.Add(new RatePackageSize(second.Id, 1, 5, false));
                        context.RatePackageSizes.Add(new RatePackageSize(second.Id, 2, 6, false));
                        context.RatePackageSizes.Add(new RatePackageSize(second.Id, 3,7, false));
                        context.RatePackageSizes.Add(new RatePackageSize(second.Id, 4, 8, false));

                    }
                    await context.SaveChangesAsync();
                }

                if (!context.Coupons.Any())
                {
                    context.Add(new Coupon("WildTen", DateTime.Now, DateTime.Now.AddDays(7), true, true, 10));
                    context.Add(new Coupon("Open15", DateTime.Now, DateTime.Now.AddDays(7), true, true, 15));
                    await context.SaveChangesAsync();
                }

                if (!context.Customers.Any())
                {

                    context.Customers.Add(new Customer("Admin1", "Admin", "Admin", TransportType.Sedan.Id, CustomerStatus.WaitingApproval.Id,"123213123","W@S.com",1,0,0,0,0, "W@S.com","","","","","",""));

                    context.Customers.Add(new Customer("Sender1", "First", "Sender", TransportType.Sedan.Id, CustomerStatus.Active.Id, "123213123", "W@S.com", 2, 0, 0, 0, 0, "W@S.com", "", "", "", "", "", ""));
                    context.Customers.Add(new Customer("Sender2", "Second", "Sender", TransportType.LightTruck.Id, CustomerStatus.Active.Id, "123213123", "W@S.com", 2, 0, 0, 0, 0, "W@S.com", "", "", "", "", "", ""));
                    context.Customers.Add(new Customer("Sender3", "Third", "Sender", TransportType.Sedan.Id, CustomerStatus.WaitingApproval.Id, "123213123", "W@S.com", 2, 0, 0, 0, 0, "W@S.com", "", "", "", "", "", ""));
                    context.Customers.Add(new Customer("Sender4", "Forth", "Sender", TransportType.Sedan.Id, CustomerStatus.Suspended.Id, "123213123", "W@S.com", 2, 0, 0, 0, 0, "W@S.com", "", "", "", "", "", ""));
                    context.Customers.Add(new Customer("Sender5", "Fith", "Sender", TransportType.Motocycle.Id, CustomerStatus.Canceled.Id, "123213123", "W@S.com", 2, 0, 0, 0, 0, "W@S.com", "", "", "", "", "", ""));


                    context.Customers.Add(new Customer("Driver 5", "Fisrt 5", "Driver", TransportType.Sedan.Id, CustomerStatus.WaitingApproval.Id, "123213123", "W@S.com", 3,2,5,5,0,"", "", "", "", "", "",""));
                    context.Customers.Add(new Customer("Driver 10", "Last 10", "Driver", TransportType.Sedan.Id, CustomerStatus.Active.Id, "123213123", "W@S.com", 3,2,10,10,15, "", "", "", "", "", "", ""));
                    context.Customers.Add(new Customer("Driver 15", "Last 15", "Driver", TransportType.Sedan.Id, CustomerStatus.Active.Id, "123213123", "W@S.com", 3, 2, 15, 15, 15, "", "", "", "", "", "", ""));
                                     

                    await context.SaveChangesAsync();

                    //set default driver's address

                    var driver = context.Customers.Where(x => x.FirstName == "Fisrt 5").FirstOrDefault();
                    driver.AddDefaultAddress(new Address("3703 9th St SW", "WA", "Tacoma", "USA", "98373", "1234567890", "Contact one", 0, 0));
                    driver.AddPicture("/Uploads/Img/Driver/DLsample-New-Adult-EDL.jpg");


                    var driver1 = context.Customers.Where(x => x.FirstName == "Last 10").FirstOrDefault();
                    driver1.AddDefaultAddress(new Address("15706 Meridian E", "WA", "Tacoma", "USA", "98375", "1234567890", "Contact one", 0, 0));
                    driver1.AddPicture("/Uploads/Img/Driver/DLsample-New-Minor-Standard.jpg");

                    var driver2 = context.Customers.Where(x => x.FirstName == "Last 15").FirstOrDefault();
                    driver2.AddDefaultAddress(new Address("16003 Pacific Ave S", "WA", "Spanaway", "USA", "98387", "1234567890", "Contact one", 0, 0));
                    driver2.AddPicture("/Uploads/Img/Driver/DLsample-Old-Adult-Standard.jpg");

                    await context.SaveChangesAsync();
                }

                //var serderA = context.Customers.Include(a=>a.Addresses).Where(x => x.CustomerTypeId == 2).FirstOrDefault();
                //if (!serderA.Addresses.Any())
                //{
                //    var addressPickup1 = new Address("5211 20th St E", "WA", "Fife", "USA", "98424", "1234567890", "Contact one", 0, 0);
                //    var addressDelivery1 = new Address("5215 25th ave se", "WA", "Lacey", "USA", "98503", "1234567890", "Contact two", 0, 0);

                //    serderA.AddAddress(addressPickup1);
                //    serderA.AddAddress(addressDelivery1);

                //    await context.SaveChangesAsync();
                //}

                if (!context.Shipments.Any())
                {

                    var serder = context.Customers.Where(x =>  x.CustomerTypeId == 2).FirstOrDefault();

                    var serder1 = context.Customers.Where(x => x.CustomerTypeId == 2).LastOrDefault();

                    var serder2 = context.Customers.Where(x => x.CustomerTypeId == 2 && x.FirstName == "Forth").LastOrDefault();

                    var serder3 = context.Customers.Where(x => x.CustomerTypeId == 2 && x.FirstName == "Third").LastOrDefault();

                    var serder4 = context.Customers.Where(x => x.CustomerTypeId == 2 && x.FirstName == "Second").LastOrDefault();


                    var addressPickup = new Address("5211 20th St E", "WA", "Fife", "USA", "98424", "1234567890", "Contact one", 0, 0);
                    var addressDelivery = new Address("5215 25th ave se", "WA", "Lacey", "USA", "98503", "1234567890", "Contact two", 0, 0);

                    var addressPickup1 = new Address("5826 54th Way SE", "WA", "Tacoma", "USA", "98513", "1234567890", "Contact one", 0, 0);
                    var addressDelivery1 = new Address("5700 Ruddell Rd SE", "WA", "Lacey", "USA", "98503", "1234567890", "Contact two and two", 0, 0);


                    var addressPickup2 = new Address("5612 176th St E", "WA", "Puyallup", "USA", "98375", "1234567890", "Contact one", 0, 0);
                    var addressDelivery2 = new Address("3500 S Meridian #494,", "WA", "Puyallup", "USA", "98373", "1234567890", "Contact two and three", 0, 0);


                    var addressPickup3 = new Address("110 9th Ave SW", "WA", "Puyallup", "USA", "98371", "1234567890", "Contact one", 0, 0);
                    var addressDelivery3 = new Address("7905 S Hosmer St", "WA", "Tacoma", "USA", "98408", "1234567890", "Contact three", 0, 0);

                    var addressPickup4 = new Address("12831 Pacific Hwy SW", "WA", "Lakewood", "USA", "98499", "1234567890", "Contact one", 0, 0);
                    var addressDelivery4 = new Address("7905 S Hosmer St", "WA", "Tacoma", "USA", "98408", "1234567890", "Contact three", 0, 0);


                    var addressPickup5 = new Address("5402 S Washington St", "WA", "Tacoma", "USA", "98409", "1234567890", "Contact one", 0, 0);
                    var addressDelivery5 = new Address("7304 Lakewood Dr W #1", "WA", "Lakewood", "USA", "98499", "1234567890", "Contact three", 0, 0);


                    context.Shipments.Add(new Shipment(addressPickup, addressDelivery, serder, 20, 2,5, PriorityType.Asap.Id, TransportType.Van.Id, "This is closed gate community", "/Uploads/Img/Shipment/download1.jpg", "/Uploads/Img/Shipment/download1.jpg",4,234,"",12,1));
                    context.Shipments.Add(new Shipment(addressPickup1, addressDelivery1, serder, 20, 2, 5, PriorityType.FourHours.Id,   TransportType.Van.Id, "package left behain door", "/Uploads/Img/Shipment/download2.jpg", "/Uploads/Img/Shipment/download2.jpg", 4, 234, "", 12, 1));

                    context.Shipments.Add(new Shipment(addressPickup2, addressDelivery2, serder1, 20, 2, 10, PriorityType.Asap.Id,   TransportType.Van.Id, "ring the bell", "/Uploads/Img/Shipment/5c71877b-46fd-48f1-8abb-0721ed6fb71b.jpg", "/Uploads/Img/Shipment/5c71877b-46fd-48f1-8abb-0721ed6fb71b.jpg", 4, 234, "", 12, 1));
                    
                    context.Shipments.Add(new Shipment(addressPickup3, addressDelivery3, serder2, 20, 2, 2, PriorityType.SixHours.Id,   TransportType.Van.Id, "package left behain door", "/Uploads/Img/Shipment/download2.jpg", "/Uploads/Img/Shipment/download2.jpg", 4, 234, "", 12, 1));

                    context.Shipments.Add(new Shipment(addressPickup4, addressDelivery4, serder3, 20, 2, 7, PriorityType.EODNextDay.Id,   TransportType.Van.Id, "package left behain door", "/Uploads/Img/Shipment/5c71877b-46fd-48f1-8abb-0721ed6fb71b.jpg", "/Uploads/Img/Shipment/5c71877b-46fd-48f1-8abb-0721ed6fb71b.jpg", 4, 234, "", 12, 1));

                    context.Shipments.Add(new Shipment(addressPickup5, addressDelivery5, serder3, 20, 2, 15, PriorityType.EODSameDay.Id,   TransportType.Van.Id, "package left behain door", "/Uploads/Img/Shipment/62ed16c2-e0d6-483e-ab61-3efd6efc7089.jpg", "/Uploads/Img/Shipment/62ed16c2-e0d6-483e-ab61-3efd6efc7089.jpg", 4, 234, "", 12, 1));


                    await context.SaveChangesAsync();
                }


               

                    if (!context.ZipCodeStates.Any())
                {

                    /*
                     
                    "zip_code","latitude","longitude","city","state","county"
                    "00501",40.922326,-72.637078,"Holtsville","NY","Suffolk"
                    "00544",40.922326,-72.637078,"Holtsville","NY","Suffolk"
                    "00601",18.165273,-66.722583,"Adjuntas","PR","Adjuntas"
                     */

                    var uploads = Path.Combine(_env.WebRootPath, "uploads\\SeedData");
                    var filePath = string.Format("{0}/zip_codes_states.csv", uploads);

                    
                    using(FileStream fileStream = new FileStream(filePath, FileMode.Open))
                    //var lines = File.ReadAllLines(filePath).Select(a => a.Split(';'));
                    //var csv = from line in lines
                    //          select (line.Split(',')).ToArray();
                    

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                        var line = reader.ReadLine();
                        while (!reader.EndOfStream)
                        {
                            try
                            {
                                  line = reader.ReadLine();
                                String[] values = CSVParser.Split(line);

                                // clean up the fields (remove " and leading spaces)
                                for (int i = 0; i < values.Length; i++)
                                {
                                    values[i] = values[i].TrimStart(' ', '"');
                                    values[i] = values[i].TrimEnd('"');
                                }



                                //var values = line.Split(',');
                                var zip = values[0];
                                var lt = values[1];
                                var lg = values[2];
                                var city = values[3];
                                var state = values[4];
                                var county = values[5];

                                var ltD = double.Parse(lt);
                                var lgD = double.Parse(lg);
                                if (state.ToLower() == "wa")
                                {
                                    context.ZipCodeStates.Add(new ZipCodeState(zip, ltD, lgD, city, state, county));
                                    context.SaveChanges();
                                }
                            }
                            catch(Exception ex)
                            {
                                var log = loggerFactory.CreateLogger("drivedrop seed "+line);
                                log.LogError(ex.Message);
                            }
                        }
                    }

                }

                if (context.Shipments.Any())
                {
                    foreach (var shiping in context.Shipments.Include(x=>x.PickupAddress).Include(x => x.DeliveryAddress).ToList())
                    {
                        if (shiping.ChargeAmount > 0)
                            continue;

                        var drop = shiping.DeliveryAddress;
                        var pick = shiping.PickupAddress;

                        var miles =await distance.FromZipToZipInMile(int.Parse(pick.ZipCode), int.Parse(drop.ZipCode));

                        decimal milesDecimal = (decimal)miles;

                        var myRate = context.Rates.Include(c=>c.RateDetails).Where(X=>X.Id==2).FirstOrDefault();

                        var rateDistance = myRate.RateDetails.Where(x => x.MileOrLbs == "miles" && x.WeightOrDistance == "distance" && x.From <= milesDecimal && milesDecimal<x.To ).FirstOrDefault();
                        var rateWeight = myRate.RateDetails.Where(x => x.MileOrLbs == "lbs" && x.WeightOrDistance == "weight" && x.From <= shiping.ShippingWeight && shiping.ShippingWeight< x.To ).FirstOrDefault();

                        var chargePerTransport = context.RateTranportTypes.Where(x=>x.RateId == myRate.Id && x.TranportTypeId==shiping.TransportTypeId).FirstOrDefault();
                        var chargePerPriority = context.RatePriorities.Where(x => x.RateId == myRate.Id && x.PriorityId == shiping.PriorityTypeId).FirstOrDefault();

                        var amountToCharge = 1 * myRate.FixChargePerShipping
                                             + shiping.ShippingWeight * rateWeight.Charge
                                             + (decimal)miles * rateDistance.Charge
                                             + shiping.Quantity*myRate.ChargePerItem
                                             + chargePerPriority.Charge 
                                             + chargePerTransport.Charge;


                        shiping.SetupPayAmount((decimal)miles, amountToCharge, 0, "", 0.9M);

                        context.Shipments.Update(shiping);

                    }
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
                    await SeedAsync(applicationBuilder, loggerFactory,_env, rate, distance, retryForAvailability);
                }
            }
        }
    }
}
    
 
