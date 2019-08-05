namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Filters
{
    using Microsoft.AspNetCore.Authorization;
    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using System.Collections.Generic;
    using System.Linq;

    namespace Basket.API.Infrastructure.Filters
    {
        public class AuthorizeCheckOperationFilter : IOperationFilter
        {
            public void Apply(Operation operation, OperationFilterContext context)
            {
                // Check for authorize attribute
                var hasAuthorize = context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() ||
                                   context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

                if (!hasAuthorize) return;

                operation.Responses.TryAdd("401", new Response { Description = "Unauthorized" });
                operation.Responses.TryAdd("403", new Response { Description = "Forbidden" });

                operation.Security = new List<IDictionary<string, IEnumerable<string>>>
                {
                    new Dictionary<string, IEnumerable<string>>
                    {
                        { "oauth2", new [] { "Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator" } }
                    }
                };
            }
        }
    }
}
