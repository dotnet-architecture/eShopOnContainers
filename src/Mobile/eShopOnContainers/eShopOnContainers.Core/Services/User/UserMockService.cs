using System.Threading.Tasks;

namespace eShopOnContainers.Core.Services.User
{
    public class UserMockService : IUserService
    {
        private Models.User.User MockUser = new Models.User.User
        {
            Name = "Jhon",
            LastName = "Doe",
            City = "Seattle, WA",
            Street = "120 E 87th Street",
            CountryCode = "98122",
            Country = "United States"
        };

        public async Task<Models.User.User> GetUserAsync()
        {
            await Task.Delay(500);

            return MockUser;
        }
    }
}