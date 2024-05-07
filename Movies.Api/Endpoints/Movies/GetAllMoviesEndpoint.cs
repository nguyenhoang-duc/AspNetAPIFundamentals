using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

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

                var options = getAllMoviesRequest.ToOptions().WithUserId(userId);

                var movies = await movieService.GetAllAsync(options, cancellationToken);

                var movieCount = await movieService.GetCountAsync(options.Title, options.YearOfRelease, cancellationToken);

                var response = movies.ToResponse(getAllMoviesRequest.Page.GetValueOrDefault(PagedRequest.DefaultPage), getAllMoviesRequest.PageSize.GetValueOrDefault(PagedRequest.DefaultPageSize), movieCount);

                return TypedResults.Ok(response);
            })
                .WithName($"{Name}V1")
                .Produces<MoviesResponse>(StatusCodes.Status200OK)
                .Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest)
                .WithApiVersionSet(ApiVersioning.VersionSet)
                .HasApiVersion(1.0);

            app.MapGet(ApiEndpoints.Movies.GetAll, async (
                [AsParameters] GetAllMoviesRequest getAllMoviesRequest,
                HttpContext httpContext,
                IMovieService movieService,
                CancellationToken cancellationToken
            ) =>
            {
                var userId = httpContext.GetUserId();

                var options = getAllMoviesRequest.ToOptions().WithUserId(userId);

                var movies = await movieService.GetAllAsync(options, cancellationToken);

                var movieCount = await movieService.GetCountAsync(options.Title, options.YearOfRelease, cancellationToken);

                var response = movies.ToResponse(getAllMoviesRequest.Page.GetValueOrDefault(PagedRequest.DefaultPage), getAllMoviesRequest.PageSize.GetValueOrDefault(PagedRequest.DefaultPageSize), movieCount);

                return TypedResults.Ok(response);
            })
                .WithName($"{Name}V2")
                .Produces<MoviesResponse>(StatusCodes.Status200OK)
                .Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest)
                .WithApiVersionSet(ApiVersioning.VersionSet)
                .HasApiVersion(2.0);

            return app;
        }
    }
}
