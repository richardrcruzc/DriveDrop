
using DriveDrop.Bl.Services;
using DriveDrop.Bl.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc; 
using Microsoft.Extensions.Options; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DriveDrop.Bl.Data;
using ApplicationCore.Entities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DriveDrop.Bl.ViewComponents
{
  
    public class RatingByShippingIdListViewComponent : ViewComponent
    {
        
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IHttpContextAccessor _httpContextAccesor;
        private readonly IIdentityParser<Models.ApplicationUser> _appUserParser;
        private readonly DriveDropContext _context;

        public RatingByShippingIdListViewComponent(
             DriveDropContext context,
            IOptionsSnapshot<AppSettings> settings,
            IHttpContextAccessor httpContextAccesor,
             IIdentityParser<Models.ApplicationUser> appUserParser)
        {
            
            _settings = settings;
            _httpContextAccesor = httpContextAccesor; 
            _appUserParser = appUserParser;

            _context = context;

        } 
        public async Task<IViewComponentResult> InvokeAsync(int pageIndex, int pageSize,int? senderId, int? driverId,int? published, int? reviewAppliedTo, int? shippingId , string hidden =null)        
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
                bool p = published == 1 ? true : false;
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
                ReviewList = model.Data.Select(r=> new RatingModel
                {
                    Published =r.Published,
                     Comment=r.Comment,
                     DateCreated=r.DateCreated,
                    // Details=r.Details.ToList()

                }).ToList(),
                SenderFilterApplied = senderId,
                DriverFilterApplied = driverId,
                PublishFilterApplied = published,
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



            return View(vm);

             
        }

    }
}
