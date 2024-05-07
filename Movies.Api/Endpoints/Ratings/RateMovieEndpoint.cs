using Movies.Api.Auth;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Endpoints.Ratings
{
    public static class RateMovieEndpoint
    {
        public const string Name = "RateMovie";

        public static IEndpointRouteBuilder MapRateMovie(this IEndpointRouteBuilder app)
        {
            app.MapPut(ApiEndpoints.Movies.Rate, async 
            (
                Guid id, 
                HttpContext httpContext, 
                RateMovieRequest request, 
                IRatingService ratingService, 
                CancellationToken token
            ) =>
            {
                var userId = httpContext.GetUserId();
                var success = await ratingService.RateMovieAsync(id, request.Rating, userId!.Value, token);

                return success ? TypedResults.Ok() : Results.NotFound();
            })
                .WithName(Name)
                .RequireAuthorization()
                .Produces<MovieRatingResponse>(StatusCodes.Status200OK);

            return app;
        }
    }
}
