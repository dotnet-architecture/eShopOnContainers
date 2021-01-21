using System.Threading.Tasks;

namespace eShopOnContainers.Services
{
    public interface IDialogService
    {
        Task ShowAlertAsync(string message, string title, string buttonLabel);
    }
}
