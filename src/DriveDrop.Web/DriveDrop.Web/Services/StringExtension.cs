using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DriveDrop.Web.Services
{
    public static class StringExtension
    {
        /// <summary>
        /// Should capitalize the first letter of each word. Acronyms will stay uppercased.
        /// Anything after a non letter or number will keep be capitalized. 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToTitleCase(this string str)
        {
            var tokens = str.Split(new[] { " " }, StringSplitOptions.None);
            var stringBuilder = new StringBuilder();
            for (var ti = 0; ti < tokens.Length; ti++)
            {
                var token = tokens[ti];
                if (token == token.ToUpper())
                    stringBuilder.Append(token + " ");
                else
                {
                    var previousWasSeperator = false;
                    var previousWasNumber = false;
                    var ignoreNumber = false;
                    for (var i = 0; i < token.Length; i++)
                    {

                        if (char.IsNumber(token[i]))
                        {
                            stringBuilder.Append(token[i]);
                            previousWasNumber = true;
                        }
                        else if (!char.IsLetter(token[i]))
                        {
                            stringBuilder.Append(token[i]);
                            previousWasSeperator = true;
                        }
                        else if ((previousWasNumber && !ignoreNumber) || previousWasSeperator)
                        {
                            stringBuilder.Append(char.ToUpper(token[i]));
                            previousWasSeperator = false;
                            previousWasNumber = false;
                        }
                        else if (i == 0)
                        {
                            ignoreNumber = true;
                            stringBuilder.Append(char.ToUpper(token[i]));
                        }
                        else
                        {
                            ignoreNumber = true;
                            stringBuilder.Append(char.ToLower(token[i]));
                        }
                    }
                    stringBuilder.Append(" ");
                }
            }
            return stringBuilder.ToString().TrimEnd();
        }
        /// <summary>
        /// Should format to USA phone number.        
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UsaPhone(this string str)
        {

            
                // If phone format is empty, code will use default format (###) ###-####
              var   phoneFormat = "(###) ###-####";
            

            // First, remove everything except of numbers
            Regex regexObj = new Regex(@"[^\d]");
            str = regexObj.Replace(str, "");

            // Second, format numbers to phone string 
            if (str.Length > 0)
            {
                str = Convert.ToInt64(str).ToString(phoneFormat);
            }

            return str;
        }

    }
}
