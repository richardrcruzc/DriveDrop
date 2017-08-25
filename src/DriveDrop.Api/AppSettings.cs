using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api
{
    public class AppSettings
    {
       

        public string BusinessAccountKey { get; set; }
        public bool UseSandbox { get; set; }
        public string CancelURL { get; set; }
        public string ReturnURL { get; set; }
        public string NotifyURL { get; set; }
        public string CurrencyCode { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }




    }

   
}
