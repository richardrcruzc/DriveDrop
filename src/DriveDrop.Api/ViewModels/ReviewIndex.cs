using ApplicationCore.Entities.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.ViewModels
{
    
    public class ReviewIndex
    {
        public IEnumerable<Review> ReviewList { get; set; }
        public IEnumerable<SelectListItem> Publish { get; set; }
        public IEnumerable<SelectListItem> ReviewTo { get; set; }
        
        public int? PublishFilterApplied { get; set; }
        public int? ReviewToFilterApplied { get; set; } 
        public int? SenderFilterApplied { get; set; }
        public int? DriverFilterApplied { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
    }
}
