using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.WebMVC.Models;
using Microsoft.eShopOnContainers.WebMVC.Services;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.WebMVC.ViewComponents
{
    public class Header : ViewComponent
    {
        public Header()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(string controller, string text = "Back")
        {
            var model = new Models.Header()
            {
                Controller = controller,
                Text = text
            };

            return Task.FromResult<IViewComponentResult>(View(model));
        }

    }
}
