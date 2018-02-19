using DriveDrop.Core.Models.Token;
using IdentityModel.Client;
using System.Threading.Tasks;

namespace DriveDrop.Core.Services.Identity
{
    public interface IIdentityService
    {
        //Thinktecture.IdentityModel.Client.TokenResponse GetToken();
        string CreateAuthorizationRequest();
        string CreateLogoutRequest(string token);
        Task<UserToken> GetTokenAsync(string code);
    }
}