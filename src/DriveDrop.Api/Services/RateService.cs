﻿using ApplicationCore.Entities.Helpers;
using DriveDrop.Api.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DriveDrop.Api.ViewModels;

namespace DriveDrop.Api.Services
{
    public class RateService : IRateService
    {

        private readonly DriveDropContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IDistanceService _distance;

        public RateService(IHostingEnvironment env, DriveDropContext context, IDistanceService distance)
        {
            _context = context;
            _env = env;
            _distance = distance;
        }



        public async Task<CalculatedCharge> CalculateAmount(decimal distance, decimal weight,  int priority, string promoCode, int packageSizeId=0) {

            var miles = distance; // await _distance.FromZipToZipInMile(zipFrom, zipTo);
            var milesDecimal = distance; // (decimal)miles;

            var myRate = await _context.Rates.Include(c => c.RateDetails).LastOrDefaultAsync();

            var rateDistance = myRate.RateDetails.Where(x => x.MileOrLbs == "miles" && x.WeightOrDistance == "distance" && x.From <= milesDecimal && milesDecimal < x.To).FirstOrDefault();
            var rateWeight = myRate.RateDetails.Where(x => x.MileOrLbs == "lbs" && x.WeightOrDistance == "weight" && x.From <= weight && weight < x.To).FirstOrDefault();

            var chargePerPriority = _context.RatePriorities.Where(x => x.RateId == myRate.Id && x.PriorityId == priority).FirstOrDefault();
            // var chargePerTransport = _context.RateTranportTypes.Where(x => x.RateId == myRate.Id && x.TranportTypeId == transportTypeId).FirstOrDefault();

            decimal rateSize = 0;
           var chargePerSize = _context.RatePackageSizes.Where(x => x.RateId == myRate.Id && x.PackageSizeId == packageSizeId).FirstOrDefault();
            if(chargePerSize!=null)
              rateSize = chargePerSize.Charge;
            //if (chargePerSize.ChargePercentage)
            //    rateSize = chargePerSize.Charge / 100;
            //else
            //    rateSize = chargePerSize.Charge;

            var amountToCharge = 1 * myRate.FixChargePerShipping
                                     + weight * rateWeight.Charge
                                     + (decimal)miles * rateDistance.Charge
                                     + 1 * myRate.ChargePerItem
                                     + chargePerPriority.Charge 
                                     + rateSize;


            var totalDiscount = 0M;
            var promo = _context.Coupons
              .Where(x => x.Code == promoCode && x.StartDate.Date<= DateTime.Now   && x.EndDate.Date>=DateTime.Now.Date)
              .FirstOrDefault();


            if (promo != null)
            {

                if (promo.Percentage)
                    totalDiscount = amountToCharge * promo.Amount / 100;
                else
                    totalDiscount =  promo.Amount;

            } 


            var model = new CalculatedCharge
            {
                TaxRate = myRate.Tax,
                TaxAmount = myRate.Tax / 100 * amountToCharge,
                AmountToCharge = amountToCharge,
                Distance = milesDecimal,
                DistanceAmount = rateDistance.Charge,
                PriorityAmount = chargePerPriority.Charge,
                TransportTypeAmount = 0, //chargePerTransport.Charge,
                WeightAmount = rateWeight.Charge,
                Discount = totalDiscount,
                AmountPerSize = rateSize

            };

            return model;
        }


        public async Task<bool> Add(Rate rate)
        {

            await _context.Rates.AddAsync(rate);
            await _context.SaveChangesAsync();
            return true;


        }
        public async Task<bool> Update(Rate rate)
        {

             _context.Rates.Update(rate);
            await _context.SaveChangesAsync();
            return true;


        }
        public async Task<List<Rate>> Get()
        {

            var rates = await _context.Rates.ToListAsync();
            
            return rates;


        }

        public async Task<Rate> Get(int id)
        {

            var rate = await _context.Rates
                .Include(x=>x.RateDetails)
                .Where(x=>x.Id==id).FirstOrDefaultAsync();

            return rate;


        }

        public async Task<decimal> Distance(string fromZip, string toZip)
        {
 

            var model = await _context.Rates.FindAsync(1);
            return 0M;
        }

        
    }
}
