 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Web.ViewModels
{
    public class Address 
    {
        public int Id { get; private set; }
        public String Street { get; private set; }

        public String City { get; private set; }

        public String State { get; private set; }

        public String Country { get; private set; }

        public String ZipCode { get; private set; }

        public String Phone { get; private set; }
        public String Contact { get; private set; }


        public Double Latitude { get; private set; }
        public Double Longitude { get; private set; }
        protected Address() { }

        
    }
}
