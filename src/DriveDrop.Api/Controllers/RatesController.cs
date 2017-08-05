using ApplicationCore.Entities.Helpers;
using DriveDrop.Api.Infrastructure;
using DriveDrop.Api.Services;
using DriveDrop.Api.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.Controllers
{ 
    //[Authorize]
    [Route("api/v1/[controller]")]
    public class RatesController : Controller
    {
        private readonly IRateService _rate;
        private readonly IDistanceService _distance;
        private readonly DriveDropContext _context;
        private readonly IHostingEnvironment _env;
        public RatesController(IHostingEnvironment env, DriveDropContext context, IDistanceService distance,
            IRateService rate)
        {
            _context = context;
            _env = env;
            _distance = distance;
            _rate = rate;

        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CalculateAmount(decimal distance, decimal weight, int priority, int packageSizeId, string promoCode="")
        {             
            return Ok(await _rate.CalculateAmount( distance,  weight,  priority,  promoCode, packageSizeId));
        }
        [HttpGet]        
        public async Task<List<Rate>> Get()
        {

            var rates = await _rate.Get();

            return rates;
          
        }

        [HttpGet]
        [Route("[action]/{id:int}")]
        public async Task<Rate> Get(int id)
        {
            /*    JSON.NET Error Self referencing loop detected for type
            https://stackoverflow.com/questions/7397207/json-net-error-self-referencing-loop-detected-for-type
            */
             
            var rate = await _context.Rates
                .Include(x => x.RatePriorities)
                .Include(c => c.PackageSizes)
                .Include(x => x.RateDetails)
                .Where(x => x.Id == id).FirstOrDefaultAsync();
            return rate;

        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> New([FromBody]RateModel m)
        {
            if (m == null)
                return null;
             

            var rate = new Rate(m.StartDate, m.EndDate, m.FixChargePerShipping, m.ChargePerItem, m.Tax); 
             
            //foreach (var i in m.RateDetails)
            //    rate.AddDetails(new RateDetail(i.RateId, i.WeightOrDistance, i.MileOrLbs, i.From, i.To, i.Charge));
             
            //foreach (var d in m.PackageSizes)
            //    rate.AddSize(new RatePackageSize(d.RateId, d.PackageSizeId, d.Charge, d.ChargePercentage ));

            //foreach (var d in m.RatePriorities)
            //    rate.AddPriority(new RatePriority(d.RateId, d.PriorityId, d.Charge, d.ChargePercentage));

            var save = await _rate.Add(rate);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = rate.Id } );
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<bool> Save([FromBody]RateModel m)
        {
            if (m == null)
                return false;
            if (m.Id == 0)
                return false;

            var oldRate =await _context.Rates
                .Include(x=>x.RatePriorities)
                .Include(c=>c.PackageSizes)
                .Include(x=>x.RateDetails)
                .Where(x => x.Id == m.Id).FirstOrDefaultAsync();
            if (oldRate == null)
                return false;


            oldRate.Update(m.StartDate, m.EndDate, m.FixChargePerShipping, m.ChargePerItem, m.Tax, m.Active);


            foreach (var i in m.RateDetails)    
                oldRate.AddDetails(new RateDetail(i.RateId, i.WeightOrDistance, i.MileOrLbs, i.From, i.To, i.Charge));

            foreach (var d in m.PackageSizes)
                oldRate.AddSize(new RatePackageSize(d.RateId, d.PackageSizeId, d.Charge, d.ChargePercentage));

            foreach (var d in m.RatePriorities)
                oldRate.AddPriority(new RatePriority(d.RateId, d.PriorityId, d.Charge, d.ChargePercentage));
            
            var update = await _rate.Update(oldRate);

            await _context.SaveChangesAsync();

            return update;
        }

        [HttpGet]
        [Route("[action]/{id:int}")]
        public async Task<bool> DeleteRate(int id)
        {
            var rate = await _context.Rates
             .Where(x => x.Id == id).FirstOrDefaultAsync();

            if (rate != null)
            { _context.Remove(rate);
                await _context.SaveChangesAsync();
            }

            return true;
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<bool> DeleteDetail([FromBody]RateDeleteDetailModel m)
        {
            if (m == null)
                return false; 

            var rate = await _context.Rates
              .Include(x => x.RatePriorities)
              .Include(c => c.PackageSizes)
              .Include(x => x.RateDetails)
              .Where(x => x.Id == m.RateId).FirstOrDefaultAsync();

            if (rate == null)
                return false;

            foreach (var d in m.RateDetails)
            {                 
                rate.RemoveDetails(d.Id);             
            }
            foreach (var d in m.RatePackageSizes)
            {
                rate.RemoveSize(d.Id);
            }
            foreach (var d in m.RatePriorities)
            {
                rate.RemovePriority(d.Id);
            }
            await _rate.Update(rate);

            return true;
        }
    }
}