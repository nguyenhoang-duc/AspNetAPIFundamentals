using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Endpoints.Movies
{
    public static class GetAllMoviesEndpoint
    {
        public const string Name = "GetAllMovies";

        public static IEndpointRouteBuilder MapGetAllMovies(this IEndpointRouteBuilder app)
        {
            app.MapGet(ApiEndpoints.Movies.GetAll, async (
                [AsParameters]GetAllMoviesRequest getAllMoviesRequest,
                HttpContext httpContext,
                IMovieService movieService, 
                CancellationToken cancellationToken
            ) =>
            {
                var userId = httpContext.GetUserId();

                var options = getAllMoviesRequest.ToOptions().WithUserId(userId!.Value);

                var movies = await movieService.GetAllAsync(options, cancellationToken);

                var movieCount = await movieService.GetCountAsync(options.Title, options.YearOfRelease, cancellationToken);

                var response = movies.ToResponse(getAllMoviesRequest.Page.GetValueOrDefault(PagedRequest.DefaultPage), getAllMoviesRequest.PageSize.GetValueOrDefault(PagedRequest.DefaultPageSize), movieCount);

                return TypedResults.Ok(response);
            }).WithName(Name);

            return app;
        }
    }
}
