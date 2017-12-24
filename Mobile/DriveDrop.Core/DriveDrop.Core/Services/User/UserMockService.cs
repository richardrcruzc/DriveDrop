using System;
using DriveDrop.Core.Models.User;
using System.Threading.Tasks;

namespace DriveDrop.Core.Services.User
{
    public class UserMockService : IUserService
    {
        private UserInfo MockUserInfo = new UserInfo
        {
            UserId = Guid.NewGuid().ToString(),
            Name = "Jhon",
            LastName = "Doe",
            PreferredUsername = "Jdoe",
            Email = "jdoe@eshop.com",
            EmailVerified = true,
            PhoneNumber = "202-555-0165",
            PhoneNumberVerified = true,
            Address = "Seattle, WA",
            Street = "120 E 87th Street",
            ZipCode = "98101",
            Country = "United States",
            State = "Seattle",
            CardNumber = "378282246310005",
            CardHolder = "American Express",
            CardSecurityNumber = "1234"
        };

        public async Task<UserInfo> GetUserInfoAsync(string authToken)
        {
            await Task.Delay(500);

            return MockUserInfo;
        }

        public async Task<string> GetUserInfoAsync(string authToken, string userName, string password)
        {
            await Task.Delay(500);

            return "";
        }
    }
}