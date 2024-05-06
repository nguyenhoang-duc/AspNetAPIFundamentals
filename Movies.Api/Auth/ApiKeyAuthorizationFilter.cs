using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Movies.Api.Auth
{
    public class ApiKeyAuthorizationFilter : IAuthorizationFilter
    {
        private readonly IConfiguration configuration;

        public ApiKeyAuthorizationFilter(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Result = new UnauthorizedObjectResult("API Key missing.");
                return;
            }

            var apiKey = configuration["ApiKey"]!;

            if (apiKey != extractedApiKey)
            {
                context.Result = new UnauthorizedObjectResult("API Key incorrect.");
            }
        }
    }
}
