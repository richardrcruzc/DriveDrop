using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GoDriveDrop.Core.Helpers
{
    public static class ValidPhone
    {
        public static bool IsValidPhone(string Phone)
        {
            try
            {
                if (string.IsNullOrEmpty(Phone))
                    return false;

                if(string.IsNullOrWhiteSpace(Phone))
                       return false;

                var r = new Regex(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$");
                return r.IsMatch(Phone);

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
