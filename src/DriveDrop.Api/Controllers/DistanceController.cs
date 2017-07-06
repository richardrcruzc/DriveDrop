using DriveDrop.Api.Infrastructure;
using DriveDrop.Api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.Controllers
{ 
    //[Authorize]
    [Route("api/v1/[controller]")]
    public class DistanceController : Controller
    {
        private readonly IDistanceService _distance;
        private readonly DriveDropContext _context;
        private readonly IHostingEnvironment _env;
        public DistanceController(IHostingEnvironment env, DriveDropContext context, IDistanceService distance)
        {
            _context = context;
            _env = env;
            _distance = distance;

        }
        [HttpGet]
        [Route("[action]/from/{from:int}/to/{to:int}")]
        public async Task<double> FromZipToZipInMile(int from, int to )
        {

          var miles=  await _distance.FromZipToZipInMile(from, to);
            return miles;
        }

        [HttpGet]
        [Route("[action]/from/{from:int}/to/{to:int}")]
        public async Task<double> FromZipToZipInKilometer(int from, int to)
        {

            var miles = await _distance.FromZipToZipInMile(from, to);
            return miles;
        }

        [HttpGet]
        [Route("[action]/from/{from:int}/miles/{miles:int}")]
        public async Task<List<ZipCodeDistance>> FindLessThanDistance(int from, int miles)
        {

            var distances = await _distance.FindLessThanDistance(from,miles);
            return distances.ToList();
        }

    }
}