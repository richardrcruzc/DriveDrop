﻿ 
namespace Identity.Api
{
    public class AppSettings
    {
        public string MvcClient { get; set; }
        public string EmailSenderEmail { get; set; }
        public string EmailSenderName { get; set; }
        public string EmailLocalDomain { get; set; }
        public int EmailLocalPort { get; set; }        
        public string EmailUser { get; set; }
        public string EmailPassword { get; set; }
        public string DriveDropApi { get; set; }

        public bool UseCustomizationData { get; set; }
    }
}
