﻿namespace Webhooks.API.Infrastructure;

public class HttpGlobalExceptionFilter : IExceptionFilter
{
    private readonly IWebHostEnvironment env;
    private readonly ILogger<HttpGlobalExceptionFilter> logger;

    public HttpGlobalExceptionFilter(IWebHostEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
    {
        this.env = env;
        this.logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        logger.LogError(new EventId(context.Exception.HResult),
            context.Exception,
            context.Exception.Message);

        if (context.Exception.GetType() == typeof(WebhooksDomainException))
        {
            var problemDetails = new ValidationProblemDetails()
            {
                Instance = context.HttpContext.Request.Path,
                Status = StatusCodes.Status400BadRequest,
                Detail = "Please refer to the errors property for additional details."
            };

            problemDetails.Errors.Add("DomainValidations", new string[] { context.Exception.Message.ToString() });

            context.Result = new BadRequestObjectResult(problemDetails);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
        else
        {
            var json = new JsonErrorResponse
            {
                Messages = new[] { "An error ocurred." }
            };

            if (env.IsDevelopment())
            {
                json.DeveloperMeesage = context.Exception;
            }

            context.Result = new InternalServerErrorObjectResult(json);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
        context.ExceptionHandled = true;
    }

    private class JsonErrorResponse
    {
        public string[] Messages { get; set; }

        public object DeveloperMeesage { get; set; }
    }
}
