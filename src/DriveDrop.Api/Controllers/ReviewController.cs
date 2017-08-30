using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DriveDrop.Api.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using ApplicationCore.Entities.Helpers;
using Microsoft.EntityFrameworkCore;
using DriveDrop.Api.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DriveDrop.Api.Controllers
{

    //[Authorize]
    [Route("api/v1/[controller]")]
    public class ReviewController : Controller
    {
        
        private readonly DriveDropContext _context;
        private readonly IHostingEnvironment _env;
        public ReviewController(IHostingEnvironment env, DriveDropContext context )
        {
            _context = context;
            _env = env;
             

        }


    
    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> QuestionList()
    {
            var questions =await  _context.ReviewQuestions.OrderBy(g => g.Group).ThenBy(q => q.Id).ToListAsync();

            return Ok(questions);

    }

        [HttpGet]
        [Route("[action]/shippingId/{id}")]
        public async Task<IActionResult> GetReviewByShippingId(int id, [FromQuery]int pageIndex = 0, [FromQuery]int pageSize = 10)
        {
            var root = (IQueryable<Review>)_context.Reviews;

            
                root = root.Where(ci => ci.Shipping.Id == id);           


            var totalItems = await root
                .LongCountAsync();

            var itemsOnPage = await root
                .Include(x => x.Details).ThenInclude(Details => Details.ReviewQuestion)
                 //.Include(x=>x.Driver)
                 //.Include(x=>x.Sender)
                 .Include(x => x.Shipping)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                 .ToListAsync();



            var model = new PaginatedItemsViewModel<Review>(
               pageIndex, pageSize, totalItems, itemsOnPage);

            var publish = new List<SelectListItem>();
            var reviewTo = new List<SelectListItem>();

            publish.Add(new SelectListItem { Value = "true", Text = "Published" });
            publish.Add(new SelectListItem { Value = "false", Text = "NoPublished" });

            reviewTo.Add(new SelectListItem { Value = "Sender", Text = "Sender" });
            reviewTo.Add(new SelectListItem { Value = "Driver", Text = "Driver" });


            var vm = new ReviewIndex()
            {
                ReviewList = model.Data,
                //SenderFilterApplied = senderId,
                //DriverFilterApplied = driverId,
                //PublishFilterApplied = published,
                Publish = publish,
                ReviewTo = reviewTo,

                //ReviewToFilterApplied = reviewAppliedTo,

                PaginationInfo = new PaginationInfo()
                {
                    ActualPage = pageIndex,
                    ItemsPerPage = model.Data.Count(),
                    TotalItems = (int)model.Count,
                    TotalPages = int.Parse(Math.Ceiling(((decimal)model.Count / pageSize)).ToString())
                }
            };

            vm.PaginationInfo.Next = (vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1) ? "is-disabled" : "";
            vm.PaginationInfo.Previous = (vm.PaginationInfo.ActualPage == 0) ? "is-disabled" : "";


            return Ok(vm);

        }


        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Publish(int reviewId, bool publish)
        {
            var review = await _context.Reviews.Where(x => x.Id == reviewId).FirstOrDefaultAsync();
            if(review == null )
                return BadRequest("ReviewNotFound");

            _context.Update(review);
            await _context.SaveChangesAsync();

            return Ok(review);

        }


        [HttpGet]
        [Route("[action]/shippingId/{int}")]
        public async Task<IActionResult> InitializeReview(int shippingId)
        { 
            var shipping = await _context
                .Shipments.Where(x => x.Id == shippingId && x.ShippingStatus.Id == 4)
                .Include(x=>x.Sender)
                .Include(x => x.Driver)
                .FirstOrDefaultAsync();
            if (shipping == null)
                return BadRequest("ReviewNotFound");

            if (shipping.Driver == null)
                return BadRequest("DriverNotFound");

            var review = _context.Reviews.Where(x => x.Shipping.Id == shippingId);
                if (review != null)
                return BadRequest("DriverNotFound");

            var sender = await _context.Customers.Where(x =>x.Id == shipping.Sender.Id).FirstOrDefaultAsync();
            if (sender == null)
                return BadRequest("ReviewNotFound");

            var driver = await _context.Customers.Where(x => x.Id ==shipping.Driver.Id).FirstOrDefaultAsync();
            if (driver == null)
                return BadRequest("DriverNotFound");


            var questions = _context.ReviewQuestions.OrderBy(x=>x.Group).ToList();

            var newReviewSender = new Review(shipping, sender, driver, "sender","",false);
            var newReviewdriver = new Review(shipping, sender, driver, "driver", "", false);
            foreach (var q in questions)
            {
                if (q.Group == "sender")
                {
                    var rd = new ReviewDetail(newReviewSender, q, 0);
                    newReviewSender.AddDetails(rd);
                }
                else
                {
                    var rd = new ReviewDetail(newReviewdriver, q, 0);
                    newReviewdriver.AddDetails(rd);
                }
            }

            _context.Add(newReviewSender);

            _context.Add(newReviewdriver);

            await _context.SaveChangesAsync();

            return Ok(newReviewSender);

        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Add([FromBody]ReviewModel model)
        {
            if(model ==  null)
                return BadRequest("ReviewNotFound" );

            var shipping =await _context.Shipments.Where(x => x.Id == model.ShippingId).FirstOrDefaultAsync();
            if(shipping==null)
                return BadRequest("ReviewNotFound");

            var sender = await _context.Customers.Where(x =>x.CustomerType.Id ==2 &&  x.Id == shipping.SenderId).FirstOrDefaultAsync();
            if (sender == null)
                return BadRequest("ReviewNotFound");

            var driver = await _context.Customers.Where(x => x.CustomerType.Id == 3 && x.Id == shipping.DriverId).FirstOrDefaultAsync();
            if (driver == null)
                return BadRequest("ReviewNotFound");

            var updateReview = await _context.Reviews.Where(x => x.Shipping.Id == model.ShippingId && x.Reviewed == model.Reviewed).FirstOrDefaultAsync();

            if (updateReview == null)
            {
                var newReview = new Review(shipping, sender, driver, model.Reviewed, model.Comment, model.Published);

                foreach (var d in model.Details)
                {
                    var rq = _context.ReviewQuestions.Where(x => x.Id == d.ReviewQuestion.Id).FirstOrDefault();
                    
                    var nD = new ReviewDetail(newReview, rq, d.Values);
                    newReview.AddDetails(nD);
                }

                _context.Add(newReview);
                await _context.SaveChangesAsync();

                return Ok(newReview);
            }
            else
            {
                 foreach (var d in model.Details)
                {
                      var rq = _context.ReviewQuestions.Where(x => x.Id == d.ReviewQuestion.Id).FirstOrDefault();

                    var nD = new ReviewDetail(updateReview, rq, d.Values);
                    updateReview.AddDetails(nD);

                }

                _context.Add(updateReview);
                await _context.SaveChangesAsync();

                return Ok(updateReview);
            }



        }
        [HttpGet]
        [Route("[action]/senderId/{senderId}/driverId/{driverId}/published/{published}/reviewAppliedTo/{reviewAppliedTo}/shippingId/{shippingId}")]
        public async Task<IActionResult> Items(int? senderId, int? driverId, int? published, int? reviewAppliedTo, int? shippingId, [FromQuery]int pageIndex = 0, [FromQuery]int pageSize = 10)
        {
            var root = (IQueryable<Review>)_context.Reviews;

            if (senderId.HasValue)
            {
                root = root.Where(ci => ci.Sender.Id == senderId);
            }
            if (driverId.HasValue)
            {
                root = root.Where(ci => ci.Driver.Id == driverId);
            }
            if (published.HasValue)
            {
                bool p = published  == 1 ? true : false;
                root = root.Where(ci => ci.Published == p);
            }
            if (shippingId.HasValue)
            {
                root = root.Where(ci => ci.Shipping.Id == shippingId);
            }
            if (reviewAppliedTo.HasValue)
            {
                var who = "driver";
                 if (reviewAppliedTo == 2)
                    who = "sender";
                root = root.Where(ci => ci.Reviewed == who);
            }

            var totalItems = await root
                .LongCountAsync();

            var itemsOnPage = await root
                .Include(x=>x.Details).ThenInclude(Details => Details.ReviewQuestion)
                //.Include(x=>x.Driver)
                //.Include(x=>x.Sender)
                 .Include(x=>x.Shipping)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                 .ToListAsync();



            var model = new PaginatedItemsViewModel<Review>(
               pageIndex, pageSize, totalItems, itemsOnPage); 

            var publish = new List<SelectListItem>();
            var reviewTo = new List<SelectListItem>();

            publish.Add(new SelectListItem { Value ="true", Text="Published" });
            publish.Add(new SelectListItem { Value ="false", Text="NoPublished"});

            reviewTo.Add(new SelectListItem { Value = "Sender", Text = "Sender" });
            reviewTo.Add(new SelectListItem { Value = "Driver", Text = "Driver" });


            var vm = new ReviewIndex()
            {
                ReviewList = model.Data, 
                 SenderFilterApplied = senderId,
                 DriverFilterApplied = driverId,
                 PublishFilterApplied= published,
                 Publish = publish,
                 ReviewTo = reviewTo,                

                 ReviewToFilterApplied = reviewAppliedTo,

                PaginationInfo = new PaginationInfo()
                {
                    ActualPage = pageIndex,
                    ItemsPerPage = model.Data.Count(),
                    TotalItems = (int)model.Count,
                    TotalPages = int.Parse(Math.Ceiling(((decimal)model.Count / pageSize)).ToString())
                }
            };

            vm.PaginationInfo.Next = (vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1) ? "is-disabled" : "";
            vm.PaginationInfo.Previous = (vm.PaginationInfo.ActualPage == 0) ? "is-disabled" : "";


            return Ok(vm);
        }

    }
}