﻿using ApplicationCore.Entities.Helpers;
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
        public async Task<IActionResult> CalculateAmount(decimal distance, decimal weight, int priority, int packageSizeId, string promoCode = "")
        {
            return Ok(await _rate.CalculateAmount(distance, weight, priority, promoCode, packageSizeId));
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Get()
        {
            /*    JSON.NET Error Self referencing loop detected for type
            https://stackoverflow.com/questions/7397207/json-net-error-self-referencing-loop-detected-for-type
            */

            var rate = await _context.Rates
                .Include(x => x.PackageSize)
                .OrderBy(x => x.Id).ToListAsync();


            return Ok(rate);

        }
        [HttpGet]
        [Route("[action]/{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            /*    JSON.NET Error Self referencing loop detected for type
            https://stackoverflow.com/questions/7397207/json-net-error-self-referencing-loop-detected-for-type
            */

            var rate = await _context.Rates
                .Include(x => x.PackageSize)
                .Include("RatePriorities.PriorityType")
                .OrderBy(x => x.RatePriorities.OrderBy(o => o.PriorityTypeId))
                .Where(x => x.Id == id)
                 .FirstOrDefaultAsync();


            return Ok(rate);

        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Save([FromBody]RateModel m)
        {
            if (m == null)
                return null;

            var rateToUpdate = await _context.Rates
                .Include(x => x.PackageSize)
                .Include("RatePriorities.PriorityType")
                .OrderBy(x => x.RatePriorities.OrderBy(o => o.PriorityTypeId))
                .Where(x => x.Id == m.Id)
                 .FirstOrDefaultAsync();

            if (rateToUpdate == null)
                return null;

            //var ps = _context.PackageSizes.Where(p => p.Id == m.PackageSize.Id).FirstOrDefault();
            //if(ps==null)
            //    return null;

            rateToUpdate.Update(m.OverHead);

            foreach (var p in m.RatePriorities)
            {
                rateToUpdate.AddPriority(p.Charge, p.PriorityTypeId);

            }

            _context.Update(rateToUpdate);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = rateToUpdate.Id });
        }


        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Details()
        {
            /*    JSON.NET Error Self referencing loop detected for type
            https://stackoverflow.com/questions/7397207/json-net-error-self-referencing-loop-detected-for-type
            */

            var rateDetails = await _context.RateDetails
                .OrderBy(x => x.Id).ToListAsync();


            return Ok(rateDetails);

        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> DetailSave([FromBody]List<RateDetailModel> m)
        {

            foreach (var d in m)
            {
                if (d.Charge <= 0)
                {
                    if (d.Id > 0)
                    {
                        //Delete record rate details
                        var delete = _context.RateDetails.Where(x => x.Id == d.Id).FirstOrDefault();
                        if (delete != null)
                            _context.Remove(delete);
                    }
                }
                else
                {
                    if (d.Id > 0)
                    {
                        //Update record rate details
                        var update = _context.RateDetails.Where(x => x.Id == d.Id).FirstOrDefault();
                        if (update != null)
                            update.Update(d.WeightOrDistance, d.MileOrLbs, d.From, d.To, d.Charge);
                    }
                    else
                    {
                        //Insert record rate details
                        var insert = new RateDetail(d.WeightOrDistance, d.MileOrLbs, d.From, d.To, d.Charge);
                        _context.Add(insert);
                    }

                }
                await _context.SaveChangesAsync();
            }

            //var rateDetails = await _context.RateDetails
            //   .OrderBy(x => x.WeightOrDistance).ThenBy(x => x.From).ThenBy(x => x.To).ToListAsync();

            //return Ok(rateDetails);

            return CreatedAtAction(nameof(Details), null);

        }


    }
}