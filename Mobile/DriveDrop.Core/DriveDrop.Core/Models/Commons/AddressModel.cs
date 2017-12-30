using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveDrop.Core.Models.Commons
{
    public class AddressModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public String TypeAddress { get; set; }

        public String Street { get; set; }

        public String City { get; set; }

        public String State { get; set; }

        public String Country { get; set; }

        public String ZipCode { get; set; }

        public String Phone { get; set; }
        public String Contact { get; set; }


        public Double Latitude { get; set; }
        public Double Longitude { get; set; }
    }
}
