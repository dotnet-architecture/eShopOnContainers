namespace Microsoft.eShopOnContainers.Services.Identity.API.Devspaces
{
    using Microsoft.Extensions.Logging;

    public class DevspacesRedirectUriValidator : IRedirectUriValidator
    {
        private readonly ILogger _logger;
        public DevspacesRedirectUriValidator(ILogger<DevspacesRedirectUriValidator> logger)
        {
            _logger = logger;
        }

        public Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, IdentityServer4.Models.Client client)
        {

            _logger.LogInformation("Client {ClientName} used post logout uri {RequestedUri}.", client.ClientName, requestedUri);
            return Task.FromResult(true);
        }

        public Task<bool> IsRedirectUriValidAsync(string requestedUri, IdentityServer4.Models.Client client)
        {
            _logger.LogInformation("Client {ClientName} used post logout uri {RequestedUri}.", client.ClientName, requestedUri);
            return Task.FromResult(true);
        }

    }
}