using ApplicationCore.Entities.ClientAgregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.ViewModels
{
    public class CustomersList
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public List<Customer> Data { get; set; }
    }
}
