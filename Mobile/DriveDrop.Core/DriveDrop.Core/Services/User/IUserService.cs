using DriveDrop.Core.Models.User;
using System.Threading.Tasks;

namespace DriveDrop.Core.Services.User
{
    public interface IUserService
    {
        Task<UserInfo> GetUserInfoAsync(string authToken);
    }
}
