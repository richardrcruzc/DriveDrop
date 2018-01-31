using ApplicationCore.Entities.Helpers;
using DriveDrop.Bl.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DriveDrop.Bl.Models;
using DriveDrop.Bl.Data;
using DriveDrop.Bl.ViewModels;

namespace DriveDrop.Bl.Services
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



        public async Task<CalculatedChargeModel> CalculateAmount(double distance, decimal weight,  int priority, string promoCode, int packageSizeId,decimal extraCharge, string extraNote,  string state  , string city ) {

            var miles = distance; // await _distance.FromZipToZipInMile(zipFrom, zipTo);
            var milesDecimal = distance; // (decimal)miles;

            var myRate = await _context.Rates
                .Include(c => c.PackageSize)
                .Include(x=>x.RatePriorities)
                .Where(p=>p.PackageSize.Id == packageSizeId)
                .FirstOrDefaultAsync();
            if (myRate == null)
                return new CalculatedChargeModel();


            var milesDecimalD = decimal.Parse(milesDecimal.ToString());

            var rateDistance =_context.RateDetails.OrderBy(u => u.MileOrLbs).Where(x => x.MileOrLbs == "miles" && x.WeightOrDistance == "distance" && x.From <= milesDecimalD && milesDecimalD < x.To).FirstOrDefault();
            var rateWeight = _context.RateDetails.OrderBy(u => u.MileOrLbs).Where(x => x.MileOrLbs == "lbs" && x.WeightOrDistance == "weight" && x.From <= weight && weight < x.To).FirstOrDefault();
            if(rateWeight==null)
                rateWeight = _context.RateDetails.OrderBy(u => u.MileOrLbs).Where(x => x.MileOrLbs == "lbs" && x.WeightOrDistance == "weight").OrderByDescending(x=>x.From).FirstOrDefault();

            var chargePerPriority = myRate.RatePriorities.OrderBy(u => u.PriorityTypeId).Where(p => p.PriorityTypeId == priority).FirstOrDefault();

              decimal rateSize = myRate.OverHead;
            

               var amountToCharge =    rateWeight.Charge
                                     + rateDistance.Charge 
                                     + chargePerPriority.Charge 
                                     + rateSize
                                     + extraCharge;


            var totalDiscount = 0M;
            var promo = _context.Coupons.OrderBy(u => u.Code)
              .Where(x => x.Code == promoCode && x.StartDate.Date<= DateTime.Now   && x.EndDate.Date>=DateTime.Now.Date)
              .FirstOrDefault();


            if (promo != null)
            {

                if (promo.Percentage)
                    totalDiscount = amountToCharge * promo.Amount / 100;
                else
                    totalDiscount =  promo.Amount;

            }
            var taxRates = 0M;
            var qTaxes = _context.TaxRates.Where(x=>x.Id>0);

            if (city != null)
                qTaxes = qTaxes.Where(x => x.City.ToLower() == city.ToLower());

            if (state != null)
                qTaxes = qTaxes.Where(x => x.State.ToLower() == state.ToLower());
            
            var tax = qTaxes.FirstOrDefault();

            if (tax != null)
                taxRates = tax.Rate;
            else {
                var defaultTax = _context.TaxRates.Where(x => x.RateDefault == true).FirstOrDefault();
                if (defaultTax != null)
                    taxRates = defaultTax.Rate;
                else
                    taxRates = 0;
                }

            var priorityName = _context.PriorityTypes.Where(x => x.Id == priority).FirstOrDefault();

            var taxAmountDetails = string.Format("Tax Rate:{0}", taxRates);
            //var distanceAmountDetails = string.Format("{0} Miles Range From: {1} To:{2}", milesDecimal, rateDistance.From, rateDistance.To);
            var distanceAmountDetails = string.Format("{0} Miles", distance);
            var priorityAmountDetail = string.Format("{0}", priorityName.Name);
            //var weightAmountDetails = string.Format("{0} Lbs Weight Range From: {1} To:{2}", weight, rateWeight.From, rateWeight.To);
            var weightAmountDetails = string.Format("{0} Lbs", weight);
            var amountPerSizeDetails = string.Format("{0}", myRate.PackageSize.Name);
            var extraChargeDetail = string.Format("{0} {1}",extraNote, extraCharge);

            var model = new CalculatedChargeModel
            {
                AmountToCharge = amountToCharge,

                ExtraCharge =extraCharge,
                ExtraChargeDetail= extraChargeDetail,

                TaxRate = taxRates,
                TaxAmount = taxRates / 100 * amountToCharge,
                TaxAmountDetails = taxAmountDetails,

                DistanceAmount = rateDistance.Charge,
                Distance = milesDecimal,
                DistanceAmountDetails = distanceAmountDetails,
                                
                PriorityAmount = chargePerPriority.Charge, 
                PriorityAmountDetail = priorityAmountDetail,

                WeightAmount =  rateWeight.Charge, 
                WeightAmountDetails = weightAmountDetails,

                AmountPerSize = rateSize,
                AmountPerSizeDetails = amountPerSizeDetails,

            };

            if (model.AmountToCharge>0)
                model.Valid = true;

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

            var rates = await _context.Rates.Include(x=>x.PackageSize).ToListAsync();
            
            return rates;


        }

        public async Task<Rate> Get(int id)
        {

            var rate = await _context.Rates
                .Include(x=>x.PackageSize)
                .Include(c=>c.RatePriorities).ThenInclude(r=>r.PriorityType)
                
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
