using GoDriveDrop.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GoDriveDrop.Core.Services.User
{
    public interface IUserService
    {
        Task<string> GetLoginTokenAsync(string username, string password);
        Task<UserInfo> GetUserInfoAsync(string authToken);
    }
}
