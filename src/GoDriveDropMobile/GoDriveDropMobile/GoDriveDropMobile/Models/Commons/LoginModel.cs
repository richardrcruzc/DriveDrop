﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoDriveDrop.Core.Models.Commons
{
    public class LoginModel
    { 
        public string Email { get; set; }
 
        public string Password { get; set; }

       
        public string ReturnUrl { get; set; }
    }
}