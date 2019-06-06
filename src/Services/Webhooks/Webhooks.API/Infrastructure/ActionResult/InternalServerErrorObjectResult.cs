using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Webhooks.API.Infrastructure.ActionResult
{
    class InternalServerErrorObjectResult : ObjectResult
    {
        public InternalServerErrorObjectResult(object error) : base(error)
        {
            StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}
