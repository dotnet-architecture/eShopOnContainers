﻿namespace Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Filters
{
    namespace Basket.API.Infrastructure.Filters
    {
        public class AuthorizeCheckOperationFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                // Check for authorize attribute
                var hasAuthorize = context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() ||
                                   context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

                if (!hasAuthorize) return;

                operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

                var oAuthScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                };

                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        [ oAuthScheme ] = new [] { "Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator" }
                    }
                };
            }
        }
    }
}
