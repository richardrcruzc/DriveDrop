using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Api.Models
{
    public class MyCustomError
    {
        public string content { get; set; }
        public string errorMessage { get; set; }
        public int  errorCode { get; set; }
    }
}
