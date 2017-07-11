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
        public async Task<IActionResult> CalculateAmount(int zipFrom, int zipTo, decimal weight, int qty, int priority, int transportTypeId, string promoCode="")
        {             
            return Ok(await _rate.CalculateAmount( zipFrom,  zipTo,  weight,  qty,  priority,  transportTypeId, promoCode));
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

            var rate = await _rate.Get(id);

            return rate;

        }

        [HttpPost]
        [Route("[action]")]
        public async Task<bool> New([FromBody]RateModel model)
        {
            var rate = new Rate();

            var oldRate = _context.Rates
                .Include(x=>x.RateDetails)
                .Where(x => x.Id == model.Id).FirstOrDefault();

            if (oldRate == null)
                return false;

            //oldRate.Update(model.StartDate, model.EndDate, model.MarkUp, model.ChargePerItem, model.Tax);
            //foreach (var item in model.RateDetails)
            //    oldRate.AddDetails(item);


            
            var save = await _rate.Add(rate);
            return save;
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<bool> Save([FromBody]RateModel model)
        {
            var rate = new Rate();

            var update = await _rate.Update(rate);
            return update;
        }
    }
}