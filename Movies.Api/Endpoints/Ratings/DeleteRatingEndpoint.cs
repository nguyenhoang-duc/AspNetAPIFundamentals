using Movies.Api.Auth;
using Movies.Application.Services;

namespace Movies.Api.Endpoints.Ratings
{
    public static class DeleteRatingEndpoint
    {
        public const string Name = "DeleteRating";

        public static IEndpointRouteBuilder MapDeleteRating(this IEndpointRouteBuilder app)
        {
            app.MapDelete(ApiEndpoints.Movies.DeleteRating, async 
            (
                Guid id, 
                HttpContext httpContext, 
                IRatingService ratingService, 
                CancellationToken cancellationToken
            ) =>
            {
                var userId = httpContext.GetUserId();

                var success = await ratingService.DeleteRatingAsync(id, userId!.Value, cancellationToken);

                return success ? TypedResults.Ok() : Results.NotFound();
            })
                .WithName(Name)
                .RequireAuthorization()
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            return app;
        }
    }
}
