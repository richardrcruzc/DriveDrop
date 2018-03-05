using GoDriveDrop.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace GoDriveDrop.Core.Services
{
    public interface IPermissionsService
    {
        Task<PermissionStatus> CheckPermissionStatusAsync(Permission permission);
        Task<Dictionary<Permission, PermissionStatus>> RequestPermissionsAsync(params Permission[] permissions);
    }
}
