using Microsoft.AspNetCore.Identity;
using Microsoft.eShopOnContainers.Services.Identity.API.Models;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Identity.API.Services
{
    public class EFLoginService : ILoginService<ApplicationUser>
    {
        readonly UserManager<ApplicationUser> _userManager;
        readonly SignInManager<ApplicationUser> _signInManager;

        public EFLoginService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public Task<ApplicationUser> FindByUsernameAsync(string user)
        {
            return _userManager.FindByEmailAsync(user);
        }

        public Task<bool> ValidateCredentialsAsync(ApplicationUser user, string password)
        {
            return _userManager.CheckPasswordAsync(user, password);
        }

        public Task SignInAsync(ApplicationUser user) {
            return _signInManager.SignInAsync(user, true);
        }
    }
}
