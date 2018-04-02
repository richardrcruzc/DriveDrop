using ApplicationCore.Entities.Helpers;
using DriveDrop.Bl.Infrastructure;
using DriveDrop.Bl.Services;
using DriveDrop.Bl.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DriveDrop.Bl.Data;
using DriveDrop.Bl.ViewModels;
using AutoMapper;

namespace DriveDrop.Bl.Controllers
{
  //  [Authorize]
    [Route("[controller]")]
    public class RatesController : Controller
    {
        private readonly IIdentityParser<ApplicationUser> _appUserParser;
        private readonly IRateService _rate;
        private readonly IDistanceService _distance;
        private readonly DriveDropContext _context;
        private readonly IHostingEnvironment _env;
        private readonly ICustomerService _cService;
        private readonly ITaxService _tax;
        private readonly IMapper _mapper;

        public RatesController(ICustomerService cService, IHostingEnvironment env, DriveDropContext context, IDistanceService distance,
            IRateService rate, IIdentityParser<ApplicationUser> appUserParser, ITaxService tax, IMapper mapper)
        {
            _mapper = mapper;
            _tax = tax;
            _cService = cService;
            _context = context;
            _env = env;
            _distance = distance;
            _rate = rate;
            _appUserParser = appUserParser;
        }
        [Route("[action]")]
        public async Task<IActionResult> ListTaxes()
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var currentUser = await _cService.Get(user.Email);
            if (!currentUser.IsAdmin)
                return RedirectToAction("index", "home");

            var rate =  _mapper.Map<List<Tax>,List<TaxModel>>(await _tax.GetTaxes());

            return View(rate);
        }

        [Route("[action]")]
        public async Task<IActionResult> AddTax()
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var currentUser = await _cService.Get(user.Email);
            if (!currentUser.IsAdmin)
                return RedirectToAction("index", "home");

            return View();
        }



        [Route("[action]")]
        public async Task<IActionResult> Items()
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var currentUser = await _cService.Get(user.Email);
            if (!currentUser.IsAdmin)
                return NotFound();

            var rate = _mapper.Map<List<Tax>, List<TaxModel>>(await _tax.GetTaxes());

            return Ok(rate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("[action]")]
        public async Task<IActionResult> SaveTax(TaxModel m)
        {
            var result = "Tax no created.";
            if (m == null)
                return null;
            if (m.RateDefault == true)
            {
                var taxes = await _context.TaxRates.Where(x => x.RateDefault).FirstOrDefaultAsync();
                if (taxes != null)
                {
                    taxes.SetDefault(false);
                    _context.Update(taxes);

                    await _context.SaveChangesAsync();
                }
            }

            var tax = await _context.TaxRates.Where(x => x.Id == m.Id).FirstOrDefaultAsync();

            if (tax == null)
            {
                tax = new Tax(m.State, m.County, m.City, m.Rate, m.RateDefault);
                _context.Add(tax);
                await _context.SaveChangesAsync();
                result = "tax created";
            }
            else
            {
                tax.Update(m.State, m.County, m.City, m.Rate, m.RateDefault);
                _context.Update(tax);
                await _context.SaveChangesAsync();
                result = "tax updated";
            }

           

            return Ok(result);
        }
        [Route("[action]")]
        public async Task<IActionResult> Edit(int id)
        {
            var rate = _mapper.Map<Rate, RateModel>(await _rate.Get(id));


            return View(rate);
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SaveRate(RateModel m)
        {
            var results = await UpdateRate(m);
            return results;
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SaveRateFromBody([FromBody]RateModel m)
        {
            var results = await  UpdateRate(m);
            return results;
        }

        private async Task<IActionResult> UpdateRate(RateModel m)
        {
            if (m == null)
                return null;

            //var rateToUpdate = await _context.Rates
            //    .Include(x => x.PackageSize)
            //     .Include(x => x.RatePriorities).ThenInclude(x => x.PriorityType)
            //    .OrderBy(x => x.RatePriorities.OrderBy(o => o.PriorityTypeId))
            //    .Where(x => x.Id == m.Id)
            //     .FirstOrDefaultAsync();
            var rateToUpdate = await _rate.Get(m.Id);


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

            return new JsonResult("rate updated");
        }

        // GET: rates/Details/5
        [Route("[action]")]
        public async Task<IActionResult> DistanceAndWeight(int id)
        {
            var query = await _context.RateDetails
             .OrderBy(x => x.Id).ToListAsync();

            var rateDetails =_mapper.Map<List<RateDetail>, List<RateDetailModel>>(query.ToList());

            var model = new WeightAndDistance
            {
                RateWeightSizeModel = rateDetails.Where(x => x.WeightOrDistance == "weight" && x.Charge > 0).OrderBy(f => f.From).ToList(),
                RateDistanceModel = rateDetails.Where(x => x.WeightOrDistance == "distance" && x.Charge > 0).OrderBy(f => f.From).ToList(),
            };

            return View(model);
        }
        private async Task<IActionResult> GetDistanceAndWeight(WeightAndDistance model)
        {

            var save = new List<RateDetailModel>();
            save.AddRange(model.RateDistanceModel);
            save.AddRange(model.RateWeightSizeModel);

            foreach (var d in save)
            {
                if (d.Charge <= 0)
                {
                    if (d.Id > 0)
                    {
                        //Delete record rate details
                        var delete = _context.RateDetails.OrderBy(u => u.Id).Where(x => x.Id == d.Id).FirstOrDefault();
                        if (delete != null)
                            _context.Remove(delete);
                    }
                }
                else
                {
                    if (d.Id > 0)
                    {
                        //Update record rate details
                        var update = _context.RateDetails.OrderBy(u => u.Id).Where(x => x.Id == d.Id).FirstOrDefault();
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

            return Ok("updated");
        }
        [HttpPost]
        [Route("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DistanceAndWeightFromBody([FromBody] WeightAndDistance m)
        {
            var t = await   GetDistanceAndWeight(m);
            return t;
        }
        [HttpPost]
        [Route("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DistanceAndWeight(WeightAndDistance m)
        {
            var t = await GetDistanceAndWeight(m);
            return t;
        }

        [Route("[action]")]
        public async Task<IActionResult> EditTax(int id)
        {
            var tax =_mapper.Map<Tax, TaxModel>(await _tax.GetTax(id));

            return View(tax);
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> AddSize(string m)
        {
            if (string.IsNullOrEmpty(m))
                return NotFound();
            try
            {
                var sql = string.Format("declare @id int;" +
                            "select top 1 @id = id from[shippings].[packageSizes] order by id desc;" +
                            "insert [shippings].[packageSizes] values( @id+1,'{0}');", m);

                var results = await _context.Database.ExecuteSqlCommandAsync(sql);

                var lastSizeId = _context.PackageSizes.AsNoTracking().OrderByDescending(x => x.Id).FirstOrDefault().Id;
                var priorities =  _context.PriorityTypes.AsNoTracking().OrderBy(x => x.Name).ToList();
                 
               var ps = _context.PackageSizes.Find(lastSizeId);
                var rate = new Rate(1m, ps);
                var y = 0;
                foreach (var p in priorities)
                {
                    //var rp = new RatePriority(priorityTypeId:p.Id, charge:0, percentage:false);
                    rate.AddPriority(++y, p.Id);
                }
                _context.Rates.Add(rate); 
                await _context.SaveChangesAsync();

                return Ok("Size added");
            }
            catch (Exception e) {
                return Ok("failed to add");
            }

            

           // await _context.SaveChangesAsync();

           
        }
        [HttpGet]
        [Route("[action]/{id:int}")]
        public async Task<IActionResult> GetRateByPackageSize(int id)
        {
            var tax = await _context.Rates
                .Include(x=>x.RatePriorities).ThenInclude(x=>x.PriorityType)
                .OrderBy(x => x.PackageSize.Id)
                .Where(x => x.PackageSize.Id == id)
                .FirstOrDefaultAsync();

            return Ok(tax);

        }
        [Route("[action]")]
        public async Task<IActionResult> Index()
        {

          var rates= _mapper.Map<List<Rate>,List<RateModel>>(await _rate.Get());
            return View(rates);
        }

        [HttpGet]
        [Route("[action]/{id:int}")]
        public async Task<IActionResult> DeleteTax(int id)
        {

            var tax = await _context.TaxRates
                .OrderBy(x => x.Id)
                .Where(x => x.Id == id).FirstOrDefaultAsync();

            if (tax==null)
                return Ok("Invalid");

            _context.Remove(tax);

            await _context.SaveChangesAsync();

            return Ok("Deleted");

        }
        [HttpGet]
        [Route("[action]/{id:int}")]
        public async Task<IActionResult> GetTax(int id)
        {

            var tax = await _context.TaxRates
                .Where(x => x.Id == id).FirstOrDefaultAsync();


            return Ok(tax);

        }


        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetTaxes()
        {
    
            var rate = await _context.TaxRates 
                .OrderBy(x => x.State).ThenBy(x=>x.County).ThenBy(x=>x.City)
                .ToListAsync();


            return Ok(rate);

        }

       



        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CalculateAmount(double distance, decimal weight, int priority, int packageSizeId, string promoCode =null, 
                                                        decimal extraCharge=0, string extraNote = null, string state = null, string city = null)
        {
            return Ok(await _rate.CalculateAmount(distance, weight, priority, promoCode, packageSizeId,extraCharge, extraNote, state, city));
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
                .Include(x => x.RatePriorities).ThenInclude(x=>x.PriorityType)
                // .OrderBy(x => x.RatePriorities.OrderBy(o => o.PriorityTypeId))
                .OrderBy(x => x.Id)
                .Where(x => x.Id == id)
                 .FirstOrDefaultAsync();


            return Ok(rate);

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
                        var delete = _context.RateDetails.OrderBy(u => u.Id).Where(x => x.Id == d.Id).FirstOrDefault();
                        if (delete != null)
                            _context.Remove(delete);
                    }
                }
                else
                {
                    if (d.Id > 0)
                    {
                        //Update record rate details
                        var update = _context.RateDetails.OrderBy(u => u.Id).Where(x => x.Id == d.Id).FirstOrDefault();
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