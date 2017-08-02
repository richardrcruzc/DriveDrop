using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Web.ViewModels
{
    public class Paypal
    {
        public Paypal()
        {
        }
        public string cmd { get; set; }
        public string business { get; set; }
        public string no_shipping { get; set; }
        public string returN { get; set; }
        public string cancel_return { get; set; }
        public string notify_url { get; set; }
        public string currency_code { get; set; }
        public string item_name { get; set; }
        public string item_number { get; set; }
        public string amount { get; set; }
        
    }
}
