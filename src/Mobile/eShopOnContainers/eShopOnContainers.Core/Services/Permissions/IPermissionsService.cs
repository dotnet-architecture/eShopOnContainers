using System.Collections.Generic;
using System.Threading.Tasks;
using eShopOnContainers.Core.Models.Permissions;

namespace eShopOnContainers.Core.Services.Permissions
{
    public interface IPermissionsService
    {
        Task<PermissionStatus> CheckPermissionStatusAsync(Permission permission);
        Task<Dictionary<Permission, PermissionStatus>> RequestPermissionsAsync(params Permission[] permissions);
    }
}
