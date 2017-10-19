using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Web.ViewModels
{
    public class AceptPackageFromSenderModel
    {
        public int Id { get; set; }
        public String SecurityCode { get; set; }
        public String fileName { get; set; }
        public int StatusId { get; set; }
    }
}
