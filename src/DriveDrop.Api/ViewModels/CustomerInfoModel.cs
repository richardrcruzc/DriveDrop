using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.ViewModels
{
    public class CustomerInfoModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PrimaryPhone { get; set; }
        public string PhotoUrl { get; set; }
        public string CustomerStatus { get; set; }
        public int StatusId { get; set; }
        public int Id { get; set; }
    }
}
