using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth;
using Movies.Application.Services;

namespace Movies.Api.Endpoints.Movies
{
    public static class DeleteMovieEndpoint
    {
        public const string Name = "DeleteMovie";

        public static IEndpointRouteBuilder MapDeleteMovie(this IEndpointRouteBuilder app)
        {
            app.MapDelete(ApiEndpoints.Movies.Delete, async 
            (
                Guid id, 
                IMovieService movieService,
                IOutputCacheStore outputCacheStore, 
                HttpContext httpContext, 
                CancellationToken token
            ) =>
            {
                var deleted = await movieService.DeleteByIdAsync(id, token);
                await outputCacheStore.EvictByTagAsync("movies", token);

                return deleted ? TypedResults.Ok() : Results.NotFound();
            })
                .WithName(Name)
                .RequireAuthorization(AuthConstants.AdminUserPolicyName)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            return app; 
        }
    }
}
