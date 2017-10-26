using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Web.ViewModels
{
    public class DropModel
    {
        public int Id { get; set; }
        public String DropPictureUri { get; set; }
        public String DropComment { get; set; }
        public int StatusId { get; set; }
    }
}
 
