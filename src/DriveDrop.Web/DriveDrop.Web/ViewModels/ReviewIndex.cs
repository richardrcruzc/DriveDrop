using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Web.ViewModels
{
    public class ReviewIndex
    {
        public List<RatingModel> ReviewList { get; set; }
        public List<SelectListItem> Publish { get; set; }
        public List<SelectListItem> ReviewTo { get; set; }

        public int? PublishFilterApplied { get; set; }
        public int? ReviewToFilterApplied { get; set; }
        public int? SenderFilterApplied { get; set; }
        public int? DriverFilterApplied { get; set; }
        public PaginationInfo PaginationInfo { get; set; }

        public string HiddenType { get; set; }
    }

     

}
