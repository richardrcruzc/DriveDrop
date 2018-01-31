using System.Security.Principal;

namespace DriveDrop.Bl.Services
{
    public interface IIdentityParser<T>
    {
        T Parse(IPrincipal principal);
    }
}
