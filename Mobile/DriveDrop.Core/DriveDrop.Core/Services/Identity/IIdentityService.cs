using DriveDrop.Core.Models.Token;
using System.Threading.Tasks;

namespace DriveDrop.Core.Services.Identity
{
    public interface IIdentityService
    {
        string CreateAuthorizationRequest();
        string CreateLogoutRequest(string token);
        Task<UserToken> GetTokenAsync(string code);
        Task<UserToken> GetTokenAsync(string code, string userName, string password);
    }
}