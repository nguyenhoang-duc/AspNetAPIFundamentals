using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Movies.Api.Auth
{
    public class AdminAuthRequirement : IAuthorizationHandler, IAuthorizationRequirement
    {
        private readonly string apiKey;

        public AdminAuthRequirement(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            if (context.User.HasClaim(AuthConstants.AdminUserClaimName, "true"))
            {
                context.Succeed(this); 
                return Task.CompletedTask;
            }

            if (context.Resource is not HttpContext httpContext)
            {
                return Task.CompletedTask; 
            }

            if (!httpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            if (apiKey != extractedApiKey)
            {
                context.Fail(); 
                return Task.CompletedTask;  
            }

            // injecting the identify from the api key 
            var identity = (ClaimsIdentity)httpContext.User.Identity!;
            identity.AddClaim(new Claim("userid", "01172e39-442e-476c-8140-71129217d395"));

            context.Succeed(this);
            return Task.CompletedTask; 
        }
    }
}

