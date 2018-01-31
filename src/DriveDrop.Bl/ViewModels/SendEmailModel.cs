using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Bl.ViewModels
{
    public class SendEmailModel
    {
        public string UserName { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
