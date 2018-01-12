namespace eShopOnContainers.Core.Services.Location
{
    using System.Threading.Tasks;
    using Models.Location;
    
    public interface ILocationService
    {
        Task UpdateUserLocation(Location newLocReq, string token);
    }
}