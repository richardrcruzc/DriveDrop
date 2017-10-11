using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.Infrastructure
{
   
        public static class API
        {
            public static class Identity
            {
            public static string GenerateEmailConfirmationTokenAsync(string baseUri, string userName )
            {
                return $"{baseUri}RegisterUser?GenerateEmailConfirmationTokenAsync={userName}";
            }

            public static string RegisterUser(string baseUri, string userName, string password)
                {
                    return $"{baseUri}RegisterUser?userName={userName}&password={password}";
                }

                public static string ChangePassword(string baseUri, string Email, string OldPassword, string NewPassword, string ConfirmPassword)
                {
                    return $"{baseUri}ChangePassword?Email={Email}&OldPassword={OldPassword}&NewPassword={NewPassword}&ConfirmPassword={ConfirmPassword}";
                }

            
        }
        }
        }
 