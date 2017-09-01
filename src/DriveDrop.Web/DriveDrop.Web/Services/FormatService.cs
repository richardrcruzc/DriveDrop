using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DriveDrop.Web.Services
{
    public  class FormatService
    {
        public FormatService() { }

        public   string FormatPhoneNumber(string phoneNum, string phoneFormat)
    {

        if (phoneFormat == "")
        {
            // If phone format is empty, code will use default format (###) ###-####
            phoneFormat = "(###) ###-####";
        }

        // First, remove everything except of numbers
        Regex regexObj = new Regex(@"[^\d]");
        phoneNum = regexObj.Replace(phoneNum, "");

        // Second, format numbers to phone string 
        if (phoneNum.Length > 0)
        {
            phoneNum = Convert.ToInt64(phoneNum).ToString(phoneFormat);
        }

        return phoneNum;
    }
}
}
