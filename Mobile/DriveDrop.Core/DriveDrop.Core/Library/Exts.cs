using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveDrop.Core.Library
{
    public static class Exts
    {
        public static string Replace(this string s, char[] separators, string newVal)
        {
            return String.Join(newVal, s.Split(separators, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
