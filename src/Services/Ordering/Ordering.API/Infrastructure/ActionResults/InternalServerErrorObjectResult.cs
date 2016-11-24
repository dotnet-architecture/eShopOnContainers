namespace Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.ActionResults
{
    using AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class InternalServerErrorObjectResult : ObjectResult
    {
        public InternalServerErrorObjectResult(object error)
            : base(error)
        {
            StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}
