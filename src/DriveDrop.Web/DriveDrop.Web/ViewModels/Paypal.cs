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

        
            public string item_number { get; set; }
        public string upload { get; set; }
        public string item_name { get; set; }
        public string amount { get; set; }
        public string price_per_item { get; set; }
        public string quantity { get; set; }
        public string tax { get; set; }

        public string discount { get; set; }
        public string custom { get; set; }
        public string charset { get; set; }
        public string bn { get; set; }
        public string no_note { get; set; }
        public string currency_code { get; set; }
        public string invoice { get; set; }
        public string rm { get; set; }
        public string address_override { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string zip { get; set; }
        public string email { get; set; }


        public string returN { get; set; }
        public string cancel_return { get; set; }
        public string notify_url { get; set; }

    }
}
