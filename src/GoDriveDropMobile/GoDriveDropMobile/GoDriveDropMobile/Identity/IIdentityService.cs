using GoDriveDrop.Core.Models;
using GoDriveDrop.Core.Models.Commons;
using IdentityModel.Client;
using System.Threading.Tasks;

namespace GoDriveDrop.Core.Identity
{
    public interface IIdentityService
    {
        //Thinktecture.IdentityModel.Client.TokenResponse GetToken();
        string CreateAuthorizationRequest();
        string CreateLogoutRequest(string token);
        Task<UserToken> GetTokenAsync(string code);
        Task<LoginToken> GetLoginTokenAsync(string password, string username);
        
    }
}
