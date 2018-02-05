using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DriveDropWebApi.Controllers
{

    [Route("identity")]
    [Authorize]
    public class ApiController : Controller
    {

        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }

        public IActionResult Index()
        {
            return View();
        }


    }
}