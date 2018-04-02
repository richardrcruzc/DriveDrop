using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GoDriveDrop.Core.Models.Commons
{
    public static class StringExtension
    {
        private static Regex digitsOnly = new Regex(@"[^\d]");

        // This is the extension method.
        // The first parameter takes the "this" modifier
        // and specifies the type for which the method is defined.
        public static string TrimAndReduce(this string str)
        {
            return ConvertWhitespacesToSingleSpaces(str).Trim();
        }

        public static string ConvertWhitespacesToSingleSpaces(this string value)
        {
            return Regex.Replace(value, @"\s+", " ");
        }

        public static string CleanPhone(this string phone)
        {
            return digitsOnly.Replace(phone, "");
        }
    }
}
