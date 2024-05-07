using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;

namespace Movies.Api.Endpoints.Ratings
{
    public static class GetUserRatingsEndpoint
    {
        public const string Name = "GetUserRating";

        public static IEndpointRouteBuilder MapGetUserRatings(this IEndpointRouteBuilder app)
        {
            app.MapGet(ApiEndpoints.Ratings.GetUserRatings, async 
            (
                HttpContext httpContext, 
                IRatingService ratingService, 
                CancellationToken token
            ) =>
            {
                var userId = httpContext.GetUserId();

                var movieRatings = await ratingService.GetRatingsForUserAsync(userId!.Value, token);

                return TypedResults.Ok(movieRatings.ToResponse());
            })
                .WithName(Name)
                .RequireAuthorization()
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);  

            return app; 
        }
    }
}
