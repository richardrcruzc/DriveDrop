﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoDriveDrop.Core.Models.Commons
{
    public class GoogleGeoCodeResponse
    {

        public string status { get; set; }
        public results[] results { get; set; }

    }

    public class results
    {
        public string formatted_address { get; set; }
        public geometry geometry { get; set; }
        public string[] types { get; set; }
        public address_component[] address_components { get; set; }
    }

    public class geometry
    {
        public bounds bounds { get; set; }
        public string location_type { get; set; }
        public location location { get; set; }
        public viewport viewport { get; set; }
    }

    public class location
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }

    public class address_component
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public string[] types { get; set; }
    }
    public class viewport
    {
        public northeast northeast { get; set; }
        public southwest southwest { get; set; }
    }

    public class bounds
    {
        public northeast northeast { get; set; }
    }

    public class northeast
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }

    public class southwest
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }
}
