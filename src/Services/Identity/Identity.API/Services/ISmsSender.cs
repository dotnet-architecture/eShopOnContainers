using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Identity.API.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
