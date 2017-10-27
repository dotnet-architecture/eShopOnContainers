using System.Threading.Tasks;
using WebMVC.Models;

namespace WebMVC.Services
{
    public interface ILocationService
    {
        Task CreateOrUpdateUserLocation(LocationDTO location);
    }
}
