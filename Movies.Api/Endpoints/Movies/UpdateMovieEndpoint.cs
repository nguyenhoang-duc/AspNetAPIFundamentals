using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Endpoints.Movies
{
    public static class UpdateMovieEndpoint
    {
        public const string Name = "UpdateMovie";

        public static IEndpointRouteBuilder MapUpdateMovie(this IEndpointRouteBuilder app)
        {
            app.MapPut(ApiEndpoints.Movies.Update, async 
            (
                Guid id, 
                UpdateMovieRequest movieRequest, 
                HttpContext httpContext, 
                IMovieService movieService, 
                IOutputCacheStore outputCacheStore,
                CancellationToken cancellationToken
            ) =>
            {
                var userId = httpContext.GetUserId();

                var movie = movieRequest.ToMovie(id);

                var updatedMovie = await movieService.UpdateAsync(movie, userId, cancellationToken);

                await outputCacheStore.EvictByTagAsync("movies", cancellationToken);

                return updatedMovie is not null ? TypedResults.Ok(movie.ToResponse()) : Results.NotFound();
            })
                .WithName(Name)
                .RequireAuthorization(AuthConstants.TrustedMemberPolicyName);

            return app; 
        }
    }
}
