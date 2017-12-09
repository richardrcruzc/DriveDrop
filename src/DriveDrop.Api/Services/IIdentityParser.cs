using System.Security.Principal;

namespace DriveDrop.Api.Services
{
    public interface IIdentityParser<T>
    {
        T Parse(IPrincipal principal);
    }
}
