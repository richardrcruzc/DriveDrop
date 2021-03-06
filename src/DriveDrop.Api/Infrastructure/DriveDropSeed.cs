﻿using ApplicationCore.Entities.ClientAgregate;
using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
using ApplicationCore.Entities.Helpers;
using DriveDrop.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
 


namespace DriveDrop.Api.Infrastructure
{

    public class DriveDropContextSeed
    {
        public async Task SeedAsync(DriveDropContext context, IHostingEnvironment env, IOptions<DriveDropSettings> settings, ILogger<DriveDropContextSeed> logger)
  {
            var policy = CreatePolicy(logger, nameof(DriveDropContextSeed));
             

            await policy.ExecuteAsync(async () =>
            {

                var useCustomizationData = settings.Value
                .UseCustomizationData;

               // var contentRootPath = env.ContentRootPath;

                if (!context.TaxRates.Any())
                {
                    context.TaxRates.Add(new Tax("WA", "Pierce", "Tacoma", 12.5M, true));
                    context.TaxRates.Add(new Tax("WA", "King", "Seattle", 14.5M, false));
                }

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
                    context.ShippingStatuses.Add(ShippingStatus.NoDriverAssigned);
                    context.ShippingStatuses.Add(ShippingStatus.PendingPickUp);
                    context.ShippingStatuses.Add(ShippingStatus.Pickup);
                    context.ShippingStatuses.Add(ShippingStatus.DeliveryInProcess);
                    context.ShippingStatuses.Add(ShippingStatus.Delivered);
                    context.ShippingStatuses.Add(ShippingStatus.Cancelled);
                    await context.SaveChangesAsync();
                }
                if (!context.TransportTypes.Any())
                {
                    context.TransportTypes.Add(TransportType.Sedan2);
                    context.TransportTypes.Add(TransportType.Sedan4);
                    context.TransportTypes.Add(TransportType.HatchBack);
                    context.TransportTypes.Add(TransportType.Van);
                    context.TransportTypes.Add(TransportType.PickUp);
                    context.TransportTypes.Add(TransportType.Bike);
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


                if (!context.ReviewQuestions.Any())
                {
                    context.ReviewQuestions.Add(new ReviewQuestion("driver ", "Was the driver on-time?"));
                    context.ReviewQuestions.Add(new ReviewQuestion("driver ", "Was the driver professional in appearance?"));
                    context.ReviewQuestions.Add(new ReviewQuestion("driver ", "How was driver customer service?"));

                    context.ReviewQuestions.Add(new ReviewQuestion("sender ", "Was the sender professional?"));
                    context.ReviewQuestions.Add(new ReviewQuestion("sender ", "Was the sender package ready to go when I got there?"));
                    context.ReviewQuestions.Add(new ReviewQuestion("sender ", "How was sender customer service?"));

                    await context.SaveChangesAsync();
                }


                if (!context.Reviews.Any())
                {
                    var shiping = context.Shipments
                        .Include(x => x.Driver)
                        .Include(x => x.Sender)
                        .FirstOrDefault();

                    if (shiping != null)
                    {
                        var questionForDriver = context.ReviewQuestions.Where(r => r.Group == "sender").ToList();

                        var driverReview = new Review(shiping, shiping.Sender, shiping.Driver, "sender", "Very well!", true);
                        var x = 1;
                        foreach (var q in questionForDriver)
                        {
                            var detail = new ReviewDetail(driverReview, q, x++);
                            driverReview.AddDetails(detail);
                        }
                        context.Reviews.Add(driverReview);


                        var questionForSender = context.ReviewQuestions.Where(r => r.Group == "driver").ToList();

                        var senderReview = new Review(shiping, shiping.Sender, shiping.Driver, "driver", "Very well!", true);
                        x = 1;
                        foreach (var q in questionForDriver)
                        {
                            var detail = new ReviewDetail(driverReview, q, x++);
                            senderReview.AddDetails(detail);
                        }
                        context.Reviews.Add(driverReview);

                        context.Reviews.Add(senderReview);


                        await context.SaveChangesAsync();
                    }
                }

                if (!context.Rates.Any())
                {
                    var e = context.PackageSizes.Find(1);
                    var s = context.PackageSizes.Find(2);
                    var m = context.PackageSizes.Find(3);
                    var l = context.PackageSizes.Find(4);

                    var re = new Rate(3, e);
                    re.AddPriority(1, 1);
                    re.AddPriority(1, 2);
                    re.AddPriority(1, 3);
                    re.AddPriority(1, 4);
                    re.AddPriority(1, 5);

                    context.Rates.Add(re);

                    var rs = new Rate(4, s);
                    rs.AddPriority(1.25M, 1);
                    rs.AddPriority(1.25M, 2);
                    rs.AddPriority(1.25M, 3);
                    rs.AddPriority(1.25M, 4);
                    rs.AddPriority(1.25M, 5);
                    context.Rates.Add(rs);

                    var rm = new Rate(5, m);
                    rm.AddPriority(1.5M, 1);
                    rm.AddPriority(1.5M, 2);
                    rm.AddPriority(1.5M, 3);
                    rm.AddPriority(1.5M, 4);
                    rm.AddPriority(1.5M, 5);
                    context.Rates.Add(rm);

                    var rl = new Rate(6, l);
                    rl.AddPriority(1.5M, 1);
                    rl.AddPriority(1.5M, 2);
                    rl.AddPriority(1.5M, 3);
                    rl.AddPriority(1.5M, 4);
                    rl.AddPriority(1.5M, 5);

                    context.Rates.Add(rl);
                    context.SaveChanges();
                }

                if (!context.RateDetails.Any())
                {

                    context.RateDetails.Add(new RateDetail("weight", "lbs", 0, 10, 0.55M));
                    context.RateDetails.Add(new RateDetail("weight", "lbs", 10, 15, 0.65M));
                    context.RateDetails.Add(new RateDetail("weight", "lbs", 15, 20, 0.75M));
                    context.RateDetails.Add(new RateDetail("weight", "lbs", 20, 25, 0.85M));
                    context.RateDetails.Add(new RateDetail("weight", "lbs", 25, 30, 0.95M));
                    context.RateDetails.Add(new RateDetail("weight", "lbs", 30, 35, 1M));

                    context.RateDetails.Add(new RateDetail("distance", "miles", 0, 20, 0.07M));
                    context.RateDetails.Add(new RateDetail("distance", "miles", 20, 40, 0.06M));
                    context.RateDetails.Add(new RateDetail("distance", "miles", 40, 60, 0.05M));
                    context.RateDetails.Add(new RateDetail("distance", "miles", 60, 80, 0.04M));
                    context.RateDetails.Add(new RateDetail("distance", "miles", 80, 100, 0.03M));
                    context.RateDetails.Add(new RateDetail("distance", "miles", 100, 120, 0.02M));
                    context.RateDetails.Add(new RateDetail("distance", "miles", 120, 140, 0.01M));

                    await context.SaveChangesAsync();


                }

                //if (!context.RatePriorities.Any())
                //{
                //    var first = context.Rates.Where(x => x.Id > 0).FirstOrDefault();
                //    if (first != null)
                //    {
                //        context.RatePriorities.Add(new  RatePriority(first.Id, 1,5,false));
                //        context.RatePriorities.Add(new RatePriority(first.Id, 2, 3, false));
                //        context.RatePriorities.Add(new RatePriority(first.Id, 3, 2, false));
                //    }
                //    var second = context.Rates.Where(x => x.Id > 0).LastOrDefault();
                //    if (second != null)
                //    {

                //        context.RatePriorities.Add(new RatePriority(second.Id, 1, 10, false));
                //        context.RatePriorities.Add(new RatePriority(second.Id, 2, 7, false));
                //        context.RatePriorities.Add(new RatePriority(second.Id, 3, 3, false));
                //    }
                //    await context.SaveChangesAsync();
                //}



                if (!context.Coupons.Any())
                {
                    context.Add(new Coupon("WildTen", DateTime.Now, DateTime.Now.AddDays(7), true, true, 10));
                    context.Add(new Coupon("Open15", DateTime.Now, DateTime.Now.AddDays(7), true, true, 15));
                    await context.SaveChangesAsync();
                }

                if (!context.Customers.Any())
                {

                    context.Customers.Add(new Customer("Admin1", "Admin", "Admin", TransportType.Sedan2.Id, CustomerStatus.Active.Id, "admin@driveDrop.com", "1234567890", 1, 0, 0, 0, 0, "admin@driveDrop.com", "1234567890", "", "", "", "", "", "", "", ""));

                    //context.Customers.Add(new Customer("Sender1", "First", "Sender", TransportType.Sedan4.Id, CustomerStatus.Active.Id, "123213123", "W@S.com", 2, 0, 0, 0, 0, "W@S.com",   "", "", "", "", "", "", "", "", ""));
                    //context.Customers.Add(new Customer("Sender2", "Second", "Sender", TransportType.PickUp.Id, CustomerStatus.Active.Id, "123213123", "W@S.com", 2, 0, 0, 0, 0, "W@S.com",  "", "", "", "", "", "", "", "", ""));
                    //context.Customers.Add(new Customer("Sender3", "Third", "Sender", TransportType.Sedan2.Id, CustomerStatus.WaitingApproval.Id, "123213123", "W@S.com", 2, 0, 0, 0, 0, "W@S.com",  "", "", "", "", "", "", "", "", ""));
                    //context.Customers.Add(new Customer("Sender4", "Forth", "Sender", TransportType.Sedan2.Id, CustomerStatus.Suspended.Id, "123213123", "W@S.com", 2, 0, 0, 0, 0, "W@S.com", "", "", "", "", "", "", "", "", "" ));
                    //context.Customers.Add(new Customer("Sender5", "Fith", "Sender", TransportType.Bike.Id, CustomerStatus.Canceled.Id, "123213123", "W@S.com", 2, 0, 0, 0, 0, "W@S.com", "", "", "", "", "", "", "", "", "" ));


                    //context.Customers.Add(new Customer("Driver 5", "Fisrt 5", "Driver", TransportType.Sedan2.Id, CustomerStatus.WaitingApproval.Id, "123213123", "W@S.com", 3,2,5,5,0,"", "", "", "", "", "","", "", "", ""));
                    //context.Customers.Add(new Customer("Driver 10", "Last 10", "Driver", TransportType.Sedan4.Id, CustomerStatus.Active.Id, "123213123", "W@S.com", 3,2,10,10,15, "", "", "", "", "", "", "", "", "", ""));
                    //context.Customers.Add(new Customer("Driver 15", "Last 15", "Driver", TransportType.Sedan2.Id, CustomerStatus.Active.Id, "123213123", "W@S.com", 3, 2, 15, 15, 15, "", "", "", "", "", "", "", "", "", ""));


                    await context.SaveChangesAsync();

                    //set default driver's address

                    //var driver = context.Customers.Where(x => x.FirstName == "Fisrt 5").FirstOrDefault();
                    //driver.AddDefaultAddress(new Address("3703 9th St SW", "WA", "Tacoma", "USA", "98373", "1234567890", "Contact one", 0, 0));
                    //driver.AddPicture("/Uploads/Img/Driver/DLsample-New-Adult-EDL.jpg");


                    //var driver1 = context.Customers.Where(x => x.FirstName == "Last 10").FirstOrDefault();
                    //driver1.AddDefaultAddress(new Address("15706 Meridian E", "WA", "Tacoma", "USA", "98375", "1234567890", "Contact one", 0, 0));
                    //driver1.AddPicture("/Uploads/Img/Driver/DLsample-New-Minor-Standard.jpg");

                    //var driver2 = context.Customers.Where(x => x.FirstName == "Last 15").FirstOrDefault();
                    //driver2.AddDefaultAddress(new Address("16003 Pacific Ave S", "WA", "Spanaway", "USA", "98387", "1234567890", "Contact one", 0, 0));
                    //driver2.AddPicture("/Uploads/Img/Driver/DLsample-Old-Adult-Standard.jpg");

                    //await context.SaveChangesAsync();
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

                //if (!context.Shipments.Any())
                //{

                //    var serder = context.Customers.Where(x =>  x.CustomerTypeId == 2).FirstOrDefault();

                //    var serder1 = context.Customers.Where(x => x.CustomerTypeId == 2).LastOrDefault();

                //    var serder2 = context.Customers.Where(x => x.CustomerTypeId == 2 && x.FirstName == "Forth").LastOrDefault();

                //    var serder3 = context.Customers.Where(x => x.CustomerTypeId == 2 && x.FirstName == "Third").LastOrDefault();

                //    var serder4 = context.Customers.Where(x => x.CustomerTypeId == 2 && x.FirstName == "Second").LastOrDefault();


                //    var addressPickup = new Address("5211 20th St E", "WA", "Fife", "USA", "98424", "1234567890", "Contact one", 0, 0);
                //    var addressDelivery = new Address("5215 25th ave se", "WA", "Lacey", "USA", "98503", "1234567890", "Contact two", 0, 0);

                //    var addressPickup1 = new Address("5826 54th Way SE", "WA", "Tacoma", "USA", "98513", "1234567890", "Contact one", 0, 0);
                //    var addressDelivery1 = new Address("5700 Ruddell Rd SE", "WA", "Lacey", "USA", "98503", "1234567890", "Contact two and two", 0, 0);


                //    var addressPickup2 = new Address("5612 176th St E", "WA", "Puyallup", "USA", "98375", "1234567890", "Contact one", 0, 0);
                //    var addressDelivery2 = new Address("3500 S Meridian #494,", "WA", "Puyallup", "USA", "98373", "1234567890", "Contact two and three", 0, 0);


                //    var addressPickup3 = new Address("110 9th Ave SW", "WA", "Puyallup", "USA", "98371", "1234567890", "Contact one", 0, 0);
                //    var addressDelivery3 = new Address("7905 S Hosmer St", "WA", "Tacoma", "USA", "98408", "1234567890", "Contact three", 0, 0);

                //    var addressPickup4 = new Address("12831 Pacific Hwy SW", "WA", "Lakewood", "USA", "98499", "1234567890", "Contact one", 0, 0);
                //    var addressDelivery4 = new Address("7905 S Hosmer St", "WA", "Tacoma", "USA", "98408", "1234567890", "Contact three", 0, 0);


                //    var addressPickup5 = new Address("5402 S Washington St", "WA", "Tacoma", "USA", "98409", "1234567890", "Contact one", 0, 0);
                //    var addressDelivery5 = new Address("7304 Lakewood Dr W #1", "WA", "Lakewood", "USA", "98499", "1234567890", "Contact three", 0, 0);


                //    context.Shipments.Add(new Shipment(addressPickup, addressDelivery, serder, 20, 2,5, PriorityType.Asap.Id, TransportType.Van.Id, "This is closed gate community", "/Uploads/Img/Shipment/download1.jpg", "/Uploads/Img/Shipment/download1.jpg",4,234,"",12,1));
                //    context.Shipments.Add(new Shipment(addressPickup1, addressDelivery1, serder, 20, 2, 5, PriorityType.FourHours.Id,   TransportType.Van.Id, "package left behain door", "/Uploads/Img/Shipment/download2.jpg", "/Uploads/Img/Shipment/download2.jpg", 4, 234, "", 12, 1));

                //    context.Shipments.Add(new Shipment(addressPickup2, addressDelivery2, serder1, 20, 2, 10, PriorityType.Asap.Id,   TransportType.Van.Id, "ring the bell", "/Uploads/Img/Shipment/5c71877b-46fd-48f1-8abb-0721ed6fb71b.jpg", "/Uploads/Img/Shipment/5c71877b-46fd-48f1-8abb-0721ed6fb71b.jpg", 4, 234, "", 12, 1));

                //    context.Shipments.Add(new Shipment(addressPickup3, addressDelivery3, serder2, 20, 2, 2, PriorityType.SixHours.Id,   TransportType.Van.Id, "package left behain door", "/Uploads/Img/Shipment/download2.jpg", "/Uploads/Img/Shipment/download2.jpg", 4, 234, "", 12, 1));

                //    context.Shipments.Add(new Shipment(addressPickup4, addressDelivery4, serder3, 20, 2, 7, PriorityType.EODNextDay.Id,   TransportType.Van.Id, "package left behain door", "/Uploads/Img/Shipment/5c71877b-46fd-48f1-8abb-0721ed6fb71b.jpg", "/Uploads/Img/Shipment/5c71877b-46fd-48f1-8abb-0721ed6fb71b.jpg", 4, 234, "", 12, 1));

                //    context.Shipments.Add(new Shipment(addressPickup5, addressDelivery5, serder3, 20, 2, 15, PriorityType.EODSameDay.Id,   TransportType.Van.Id, "package left behain door", "/Uploads/Img/Shipment/62ed16c2-e0d6-483e-ab61-3efd6efc7089.jpg", "/Uploads/Img/Shipment/62ed16c2-e0d6-483e-ab61-3efd6efc7089.jpg", 4, 234, "", 12, 1));


                //    await context.SaveChangesAsync();
                //}




                //    if (!context.ZipCodeStates.Any())
                //{

                //    /*

                //    "zip_code","latitude","longitude","city","state","county"
                //    "00501",40.922326,-72.637078,"Holtsville","NY","Suffolk"
                //    "00544",40.922326,-72.637078,"Holtsville","NY","Suffolk"
                //    "00601",18.165273,-66.722583,"Adjuntas","PR","Adjuntas"
                //     */

                //    var uploads = Path.Combine(_env.WebRootPath, "uploads\\SeedData");
                //    var filePath = string.Format("{0}/zip_codes_states.csv", uploads);


                //    using(FileStream fileStream = new FileStream(filePath, FileMode.Open))
                //    //var lines = File.ReadAllLines(filePath).Select(a => a.Split(';'));
                //    //var csv = from line in lines
                //    //          select (line.Split(',')).ToArray();


                //    using (StreamReader reader = new StreamReader(fileStream))
                //    {
                //        Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                //        var line = reader.ReadLine();
                //        while (!reader.EndOfStream)
                //        {
                //            try
                //            {
                //                  line = reader.ReadLine();
                //                String[] values = CSVParser.Split(line);

                //                // clean up the fields (remove " and leading spaces)
                //                for (int i = 0; i < values.Length; i++)
                //                {
                //                    values[i] = values[i].TrimStart(' ', '"');
                //                    values[i] = values[i].TrimEnd('"');
                //                }



                //                //var values = line.Split(',');
                //                var zip = values[0];
                //                var lt = values[1];
                //                var lg = values[2];
                //                var city = values[3];
                //                var state = values[4];
                //                var county = values[5];

                //                var ltD = double.Parse(lt);
                //                var lgD = double.Parse(lg);
                //                if (state.ToLower() == "wa")
                //                {
                //                    context.ZipCodeStates.Add(new ZipCodeState(zip, ltD, lgD, city, state, county));
                //                    context.SaveChanges();
                //                }
                //            }
                //            catch(Exception ex)
                //            {
                //                var log = loggerFactory.CreateLogger("drivedrop seed "+line);
                //                log.LogError(ex.Message);
                //            }
                //        }
                //    }

                //}

                //if (context.Shipments.Any())
                //{
                //    foreach (var shiping in context.Shipments.Include(x=>x.PickupAddress).Include(x => x.DeliveryAddress).ToList())
                //    {
                //        if (shiping.ChargeAmount > 0)
                //            continue;

                //        var drop = shiping.DeliveryAddress;
                //        var pick = shiping.PickupAddress;

                //        var miles =await distance.FromZipToZipInMile(int.Parse(pick.ZipCode), int.Parse(drop.ZipCode));

                //        decimal milesDecimal = (decimal)miles;

                //        var myRate = context.Rates.Include(c=>c.RateDetails).Where(X=>X.Id==2).FirstOrDefault();

                //        var rateDistance = myRate.RateDetails.Where(x => x.MileOrLbs == "miles" && x.WeightOrDistance == "distance" && x.From <= milesDecimal && milesDecimal<x.To ).FirstOrDefault();
                //        var rateWeight = myRate.RateDetails.Where(x => x.MileOrLbs == "lbs" && x.WeightOrDistance == "weight" && x.From <= shiping.ShippingWeight && shiping.ShippingWeight< x.To ).FirstOrDefault();

                //        var chargePerTransport = context.RateTranportTypes.Where(x=>x.RateId == myRate.Id && x.TranportTypeId==shiping.TransportTypeId).FirstOrDefault();
                //        var chargePerPriority = context.RatePriorities.Where(x => x.RateId == myRate.Id && x.PriorityId == shiping.PriorityTypeId).FirstOrDefault();

                //        var amountToCharge = 1 * myRate.FixChargePerShipping
                //                             + shiping.ShippingWeight * rateWeight.Charge
                //                             + (decimal)miles * rateDistance.Charge
                //                             + shiping.Quantity*myRate.ChargePerItem
                //                             + chargePerPriority.Charge 
                //                             + chargePerTransport.Charge;


                //        shiping.SetupPayAmount((decimal)miles, amountToCharge, 0, "", 0.9M);

                //        context.Shipments.Update(shiping);

                //    }
                // await context.SaveChangesAsync();

                //    }








                //            //if (!context.ShipmentCustomers.Any())
                //            //{

                //            //    var addressPickup = new Address("5215 90th st E", "WA", "Tacoma", "USA", "98446","1234567890","Contact one", 0, 0);
                //            //    var addressDelivery = new Address("5215 25th ave se", "WA", "Lacey", "USA", "98503", "1234567890", "Contact two", 0, 0);

                //            //    var customer = context.Customers.Where(x => x.FirstName == "First" && x.CustomerTypeId == 2).FirstOrDefault();
                //            //    var shipment = context.Shipments.Where(c => c.Note == "This is closed gate community").FirstOrDefault();

                //            //    context.ShipmentCustomers.Add(new ShipmentCustomer { CustomerId = customer.Id, CustomerTypeId = CustomerType.Sender.Id, ShipmentId = shipment.Id });
                //            //    await context.SaveChangesAsync();
                //            //    customer = context.Customers.Where(x => x.FirstName == "Last" && x.CustomerTypeId == 3).FirstOrDefault();

                //            //    context.ShipmentCustomers.Add(new ShipmentCustomer { CustomerId = customer.Id, CustomerTypeId = CustomerType.Driver.Id, ShipmentId = shipment.Id });
                //            //    await context.SaveChangesAsync();

                //            //    context.ShipmentAddresses.Add(new ShipmentAddress { AddressTypeId = AddressType.Pickup.Id, Address = addressPickup, ShipmentId = shipment.Id });
                //            //    context.ShipmentAddresses.Add(new ShipmentAddress { AddressTypeId = AddressType.Delivery.Id, Address = addressDelivery, ShipmentId = shipment.Id });

                //            //    await context.SaveChangesAsync();

                //            //}


                await context.SaveChangesAsync();

            });
        }
            
        private Policy CreatePolicy(ILogger<DriveDropContextSeed> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogTrace($"[{prefix}] Exception {exception.GetType().Name} with message ${exception.Message} detected on attempt {retry} of {retries}");
                    }
                );
        }
    }
}
    
 
