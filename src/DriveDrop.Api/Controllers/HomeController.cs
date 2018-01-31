 
using Microsoft.AspNetCore.Mvc; 

namespace DriveDrop.Api.Controllers
{
    public class HomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
          //  return new RedirectResult("~/swagger");
        }
    }
}
