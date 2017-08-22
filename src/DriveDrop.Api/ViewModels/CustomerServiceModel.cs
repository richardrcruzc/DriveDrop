using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.ViewModels
{
    public class CustomerServiceModel
    {
        public CustomerViewModel CustomerIndo { set; get; }
        public bool IsAdmin { set; get; }
        public bool IsValid { set; get; }

    }
}
