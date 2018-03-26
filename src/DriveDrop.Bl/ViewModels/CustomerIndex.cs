﻿ 
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Bl.ViewModels
{
    public class CustomerIndex
    {
        public IEnumerable<CustomerModel> CustomerList { get; set; }
        public IEnumerable<SelectListItem> CustomerType { get; set; }
        public IEnumerable<SelectListItem> CustomerStatus { get; set; }
        public IEnumerable<SelectListItem> TransportType { get; set; }
        public int? TypeFilterApplied { get; set; }
        public int? StatusFilterApplied { get; set; }
        public int? TransportFilterApplied { get; set; }
        public string LastName { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
    }
}