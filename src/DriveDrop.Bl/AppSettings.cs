using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Bl
{
    public class AppSettings
    { 
        public Connectionstrings ConnectionStrings { get; set; }
        public string DriveDropUrl { get; set; }
        public string IdentityUrl { get; set; }
        public string CallBackUrl { get; set; }
        public string ImagesUrl { get; set; }
        public string PicBaseUrl { get; set; }
        public Logging Logging { get; set; }

        public string BusinessAccountKey { get; set; }
        public bool UseSandbox { get; set; }
        public string CancelURL { get; set; }
        public string ReturnURL { get; set; }
        public string NotifyURL { get; set; }
        public string CurrencyCode { get; set; }


        public string RediConnectionString { get; set; }

        public bool UseCustomizationData { get; set; }
         
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string MvcClient { get; set; } 

        public string EmailSenderEmail { get; set; }
        public string EmailSenderName { get; set; }
        public string EmailLocalDomain { get; set; }
        public int EmailLocalPort { get; set; }
        public string EmailUser { get; set; }
        public string EmailPassword { get; set; } 
    }
    public class Connectionstrings
    {
        public string DefaultConnection { get; set; }
    }

    public class Logging
    {
        public bool IncludeScopes { get; set; }
        public Loglevel LogLevel { get; set; }
    }

    public class Loglevel
    {
        public string Default { get; set; }
        public string System { get; set; }
        public string Microsoft { get; set; }
    }
}
