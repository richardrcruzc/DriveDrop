using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DriveDrop.Bl.Services;
using DriveDrop.Bl.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DriveDrop.Bl.Controllers
{
    [Route("api/v1/[controller]")]
    //[Authorize]
    public class CurrentUserController : Controller
    {
        private readonly ICustomerService _cService;
        public CurrentUserController(ICustomerService cService)
        {
            _cService = cService;
        }
        [HttpGet("{userName}")]
        [ProducesResponseType(typeof(CurrentCustomerModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetbyUserName(string userName)
        {
            var c = await _cService.Get(userName);
            if (c == null)
                return NotFound();



            var content = JsonConvert.SerializeObject(c, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            
                            NullValueHandling = NullValueHandling.Ignore,
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });



            return Ok(content);





            //  c = new CurrentCustomerModel { };

           // return new JsonResult( c);
        }
    }
}