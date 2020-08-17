using System.Threading.Tasks;

namespace eShopOnContainers.Core.Services.Location
{    
    public interface ILocationService
    {
        Task UpdateUserLocation(eShopOnContainers.Core.Models.Location.Location newLocReq, string token);
    }
}