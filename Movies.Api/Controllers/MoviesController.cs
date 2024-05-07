using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Repositories;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers
{
    [ApiVersion(1.0)]
    [ApiVersion(2.0)]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService movieService;
        private readonly IOutputCacheStore outputCacheStore;

        public MoviesController(IMovieService movieService, IOutputCacheStore outputCacheStore)
        {
            this.movieService = movieService;
            this.outputCacheStore = outputCacheStore;   
        }

        // Allow only admins to update a movie
        [Authorize(AuthConstants.TrustedMemberPolicyName)]
        [HttpPost(ApiEndpoints.Movies.Create)]
        [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken cancellationToken)
        {
            var movie = request.ToMovie();

            _ = await movieService.CreateAsync(movie, cancellationToken);

            await outputCacheStore.EvictByTagAsync("movies", cancellationToken);

            var response = movie.ToResponse();

            return CreatedAtAction(nameof(GetV1), new { idOrSlug = movie.Id }, movie); 
        }

        [MapToApiVersion(1.0)]
        [HttpGet(ApiEndpoints.Movies.Get)]
        [OutputCache(PolicyName = "MoviesCache")]
        [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetV1([FromRoute]string idOrSlug, CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();

            var movie = Guid.TryParse(idOrSlug, out var id) ? await movieService.GetByIdAsync(id, userId, cancellationToken) 
                                                            : await movieService.GetBySlugAsync(idOrSlug, userId, cancellationToken);

            return movie is null ? NotFound() : Ok(movie.ToResponse()); 
        }

        [MapToApiVersion(2.0)]
        [HttpGet(ApiEndpoints.Movies.Get)]
        [OutputCache(PolicyName = "MoviesCache")]
        [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetV2([FromRoute] string idOrSlug, CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();

            var movie = Guid.TryParse(idOrSlug, out var id) ? await movieService.GetByIdAsync(id, userId, cancellationToken)
                                                            : await movieService.GetBySlugAsync(idOrSlug, userId, cancellationToken);

            return movie is null ? NotFound() : Ok(movie.ToResponse());
        }

        [Authorize]
        [HttpGet(ApiEndpoints.Movies.GetAll)]
        [OutputCache(PolicyName = "MoviesCache")]
        [ProducesResponseType(typeof(MoviesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery]GetAllMoviesRequest getAllMoviesRequest, CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();

            var options = getAllMoviesRequest.ToOptions().WithUserId(userId!.Value);

            var movies = await movieService.GetAllAsync(options, cancellationToken);

            var movieCount = await movieService.GetCountAsync(options.Title, options.YearOfRelease, cancellationToken);


        return Ok(movies);
            //return Ok(movies.ToResponse(getAllMoviesRequest.Page, getAllMoviesRequest.PageSize, movieCount));
        }
         
        [Authorize(AuthConstants.TrustedMemberPolicyName)]
        [HttpPut(ApiEndpoints.Movies.Update)]
        [ProducesResponseType(typeof(MoviesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromRoute]Guid id, [FromBody]UpdateMovieRequest movieRequest, CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();

            var movie = movieRequest.ToMovie(id); 

            var updatedMovie = await movieService.UpdateAsync(movie, userId, cancellationToken);

            await outputCacheStore.EvictByTagAsync("movies", cancellationToken);

            return updatedMovie is not null ? Ok(movie.ToResponse()) : NotFound(); 
        }

        [Authorize(AuthConstants.AdminUserPolicyName)]
        [HttpDelete(ApiEndpoints.Movies.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute]Guid id, CancellationToken cancellationToken)
        {
            var deleted = await movieService.DeleteByIdAsync(id, cancellationToken);

            await outputCacheStore.EvictByTagAsync("movies", cancellationToken);

            return deleted ? Ok() : NotFound(); 
        }
    }
}
