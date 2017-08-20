
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Web.ViewModels
{
    public class Address 
    {
        public int Id { get;  set; }
        [JsonProperty("street")]
        public String Street { get;  set; }

        public String City { get;  set; }

        public String State { get;  set; }

        public String Country { get;  set; }

        public String ZipCode { get;  set; }

        public String Phone { get;  set; }
        public String Contact { get;  set; }


        public Double Latitude { get;  set; }
        public Double Longitude { get;  set; }
        protected Address() { }

        
    }
}
