 
//using DriveDrop.Web.Infrastructure;
//using DriveDrop.Web.Services;
//using DriveDrop.Web.ViewModels;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http;
//using Microsoft.Extensions.Options;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net.Http;
//using System.Threading.Tasks; 

//namespace DriveDrop.Web.Controllers
//{
//    [Authorize]
//    public class CustomerController: Controller
//    {

//        private readonly IOptionsSnapshot<AppSettings> _settings;
//        private readonly IHttpClient _apiClient;

//        private readonly IHostingEnvironment _env; 
        
//        private readonly IHttpContextAccessor _httpContextAccesor;


//        private readonly string _remoteServiceBaseUrl;


//        public CustomerController (IOptionsSnapshot<AppSettings> settings, IHttpContextAccessor httpContextAccesor, 
//            IHttpClient httpClient, IHostingEnvironment env
//             )
//        {
//            _settings = settings;
//            _apiClient = httpClient;
//            _env = env; 
            

//            _httpContextAccesor = httpContextAccesor;
//            _remoteServiceBaseUrl = $"{_settings.Value.DripDropUrl}/api/v1/admin/";
//        }

//        public IActionResult Sender()
//        {

//            var model = new CustomerModel();

//            PrepareCustomerModel(model);




//            return View(model);
//        }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Sender(CustomerModel c )
//        {
//            //try
//            //{
//            //    if (ModelState.IsValid)
//            //    {
//            //        var deliveryAddres = new Address(c.DeliveryStreet, c.DeliveryCity, "WA", "USA", c.DeliveryZipCode,c.DeliveryPhone,c.DeliveryContact, 0, 0);
//            //        var pickUpAddres = new Address(c.PickupStreet, c.PickupCity, "WA", "USA", c.PickupZipCode, c.PickupPhone, c.PickupContact, 0, 0);

//            //        var tmpUser = Guid.NewGuid().ToString();

//            //        var newCustomer = new Customer(tmpUser, c.FirstName, c.LastName,null, CustomerStatus.WaitingApproval.Id,c.Email, c.Phone, CustomerType.Sender.Id, 0, 0, 0);

//            //        _context.Add(newCustomer);
//            //        await _context.SaveChangesAsync();
                     
                  

//            //        var shipment = new Shipment(pickUpAddres,deliveryAddres, newCustomer, 0, 0, c.PriorityTypeId, c.PriorityTypeLevel,  c.TransportTypeId??0, c.Note, "", "");
//            //        _context.Add(shipment);

//            //        await _context.SaveChangesAsync();


//            //        shipment.VerifyOrAddPaymentMethod(1, c.CardHolderName,c.CardNumber,c.SecurityNumber.ToString(),c.CardHolderName, DateTime.Now.AddMonths(12), shipment.Id);

                     
//            //        _context.Update(shipment);

//            //        _context.Update(newCustomer);


//            //        await _context.SaveChangesAsync();


//            //        return RedirectToAction("Index");
//            //    }
//            //}
//            //catch (DbUpdateException   ex )
//            //{
//            //    //Log the error (uncomment ex variable name and write a log.
//            //    var error = string.Format("Unable to save changes. " +
//            //        "Try again, and if the problem persists " +
//            //        "see your system administrator. {0}", ex.Message);

//            //    ModelState.AddModelError("", error);
//            //}

//            PrepareCustomerModel(c);
//            return View(c);
//        }


//        public IActionResult Create()
//        {

//            var model = new CustomerModel();


//            //model.CustomerTypeList =   _context.CustomerTypes.Select(x=> new SelectListItem { Value=x.Id.ToString(), Text=x.Name }).ToList();
//            //model.TransportTypeList = _context.TransportTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
//            //model.CustomerStatusList = _context.CustomerStatuses.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
           
           

//            return View(model);
//        }



//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(CustomerModel c)
//        {
//            //try
//            //{
//            //    if (ModelState.IsValid)
//            //    {


//            //        var newCustomer = new Customer("rewer", c.FirstName, c.LastName,c.TransportTypeId??0, c.CustomerStatusId,c.Email,c.Phone,2, c.MaxPackage ?? 0, c.PickupRadius ?? 0, c.DeliverRadius ?? 0);


//            //        _context.Add(newCustomer);
//            //        await _context.SaveChangesAsync();
//            //        return RedirectToAction("Index");
//            //    }
//            //}
//            //catch (DbUpdateException /* ex */)
//            //{
//            //    //Log the error (uncomment ex variable name and write a log.
//            //    ModelState.AddModelError("", "Unable to save changes. " +
//            //        "Try again, and if the problem persists " +
//            //        "see your system administrator.");
//            //}
//            return View(c);
//        }

//        //public async Task<IActionResult> Details(int? id)
//        //{
//        //    if (id == null)
//        //    {
//        //        return NotFound();
//        //    }

//        //    var customer = await _context.Customers.FindAsync(id);
//        //        //.Include(s=>s.TransportType).Include(t=>t.CustomerStatus).Include(s=>s.CustomerType)
//        //        //.AsNoTracking()
//        //        //.SingleOrDefaultAsync(m => m.Id == id);

//        //    if (customer == null)
//        //    {
//        //        return NotFound();
//        //    }

//        //    return View(customer);
//        //}
//        //public async Task<IActionResult> Edit(int? id)
//        //{
//        //    if (id == null)
//        //    {
//        //        return NotFound();
//        //    }

//        //    var customer = await _context.Customers.SingleOrDefaultAsync(m => m.Id == id);
//        //    if (customer == null)
//        //    {
//        //        return NotFound();
//        //    }
//        //    return View(customer);
//        //}
//        //[HttpPost, ActionName("Edit")]
//        //[ValidateAntiForgeryToken]
//        //public async Task<IActionResult> EditPost(int? id, Customer c)
//        //{
//        //    if (id == null)
//        //    {
//        //        return NotFound();
//        //    }
//        //    var u = await _context.Customers.SingleOrDefaultAsync(s => s.Id == id);

//        //    u.Update(c.IdentityGuid,
//        //   c.FirstName,
//        // c.LastName,
//        //c.TransportTypeId ?? 0,
//        // c.CustomerStatusId,
//        // c.MaxPackage ?? 0,
//        // c.PickupRadius ?? 0,
//        // c.DeliverRadius ?? 0,
//        // c.CustomerTypeId);
            

//        //        try
//        //        {
//        //            _context.Update(u);
//        //            await _context.SaveChangesAsync(); 
//        //            return RedirectToAction("Index");
//        //        }
//        //        catch (DbUpdateException /* ex */)
//        //        {
//        //            //Log the error (uncomment ex variable name and write a log.)
//        //            ModelState.AddModelError("", "Unable to save changes. " +
//        //                "Try again, and if the problem persists, " +
//        //                "see your system administrator.");
//        //        }
           
//        //    return View(u);
//        //}


//        public async Task<IActionResult> Index(int? TypeFilterApplied, int? StatusFilterApplied, int? TransportFilterApplied,  int? page, string LastName = null)
//        {
//            var itemsPage = 10;
//            if (page < 0)
//                page = 0;

//            var allUri = API.Admin.GetAllCustomers(_remoteServiceBaseUrl, page??0, itemsPage, StatusFilterApplied, TypeFilterApplied, TransportFilterApplied,LastName);

//            var dataString = await _apiClient.GetStringAsync(allUri);

//            var response = JsonConvert.DeserializeObject<CustomersList>(dataString);

//            var vm = new CustomerIndex()
//            {
//                CustomerList = response.Data,
//                //CustomerType = await _customerService.GetCustomerType(),
//                //CustomerStatus = await _customerService.GetCustomerStatus(),
//                //TransportType = await _customerService.GetCustomerTrasnport(),
//                TypeFilterApplied = TypeFilterApplied ?? 0,
//                StatusFilterApplied = StatusFilterApplied ?? 0,
//                TransportFilterApplied = TransportFilterApplied ?? 0,
//                LastName = LastName,
//                PaginationInfo = new PaginationInfo()
//                {
//                    ActualPage = page ?? 0,
//                    ItemsPerPage = response.Data.Count,
//                    TotalItems = response.Count,
//                    TotalPages = int.Parse(Math.Ceiling(((decimal)response.Count / itemsPage)).ToString())
//                }
//            };

//            vm.PaginationInfo.Next = (vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1) ? "is-disabled" : "";
//            vm.PaginationInfo.Previous = (vm.PaginationInfo.ActualPage == 0) ? "is-disabled" : ""; 

//            return View(vm);

//            //var customers = await _customerService.GetCustomers(page ?? 0, itemsPage, TypeFilterApplied, StatusFilterApplied, TransportFilterApplied, LastName);

//            //var vm = new CustomerIndex()
//            //{
//            //    CustomerList = customers.Data,
//            //    CustomerType = await _customerService.GetCustomerType(),
//            //    CustomerStatus = await _customerService.GetCustomerStatus(),
//            //    TransportType = await _customerService.GetCustomerTrasnport(),
//            //    TypeFilterApplied = TypeFilterApplied ?? 0,
//            //    StatusFilterApplied = StatusFilterApplied ?? 0,
//            //    TransportFilterApplied = TransportFilterApplied ?? 0,
//            //    LastName = LastName,
//            //    PaginationInfo = new PaginationInfo()
//            //    {
//            //        ActualPage = page ?? 0,
//            //        ItemsPerPage = customers.Data.Count,
//            //        TotalItems = customers.Count,
//            //        TotalPages = int.Parse(Math.Ceiling(((decimal)customers.Count / itemsPage)).ToString())
//            //    }
//            //};

//            //vm.PaginationInfo.Next = (vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1) ? "is-disabled" : "";
//            //vm.PaginationInfo.Previous = (vm.PaginationInfo.ActualPage == 0) ? "is-disabled" : "";


//        }

//        public CustomerModel PrepareCustomerModel(CustomerModel model)
//        {
//            //model.CustomerTypeList = _context.CustomerTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
//            //model.TransportTypeList = _context.TransportTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
//            //model.CustomerStatusList = _context.CustomerStatuses.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

//            //model.PriorityTypeList = _context.PriorityTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

//            return model;
//        }

//        //[HttpGet("[controller]/pic/{id}")]
//        //public IActionResult GetImage(int id)
//        //{
//        //    byte[] imageBytes;
//        //    try
//        //    {
//        //        imageBytes = _imageService.GetImageBytesById(id);
//        //    }
//        //    catch (ImageMissingException /* ex*/)
//        //    {
//        //        _logger.LogWarning($"No image found for id: {id}");
//        //        return NotFound();
//        //    }
//        //    return File(imageBytes, "image/png");
//        //}


//        public IActionResult Error()
//        {
//            return View();
//        }



//        async Task<string> GetUserTokenAsync()
//        {
//            var context = _httpContextAccesor.HttpContext;

//            return await context.Authentication.GetTokenAsync("access_token");
//        }
//    }
//}
