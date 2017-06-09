namespace eShopOnContainers.Core.Services.Location
{
    using System.Threading.Tasks;
    using eShopOnContainers.Core.Models.Location;
    
    public interface ILocationService
    {
        Task UpdateUserLocation(LocationRequest newLocReq);
    }
}