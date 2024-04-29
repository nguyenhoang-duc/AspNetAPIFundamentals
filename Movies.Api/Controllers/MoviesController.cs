using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Repositories;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers
{
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService movieService;

        public MoviesController(IMovieService movieService)
        {
            this.movieService = movieService;
        }

        // Allow only admins to update a movie
        [Authorize(AuthConstants.TrustedMemberPolicyName)]
        [HttpPost(ApiEndpoints.Movies.Create)]
        public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken cancellationToken)
        {
            var movie = request.ToMovie();

            _ = await movieService.CreateAsync(movie, cancellationToken);

            var response = movie.ToResponse();

            return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie); 
        }

        [HttpGet(ApiEndpoints.Movies.Get)]
        public async Task<IActionResult> Get([FromRoute]string idOrSlug, [FromServices] LinkGenerator linkGenerator, CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();

            var movie = Guid.TryParse(idOrSlug, out var id) ? await movieService.GetByIdAsync(id, userId, cancellationToken) 
                                                            : await movieService.GetBySlugAsync(idOrSlug, userId, cancellationToken);

            if (movie is null)
            {
                return NotFound(); 
            }

            var movieResponse = movie.ToResponse();

            movieResponse.Links.Add(new Link
            {
                Href = linkGenerator.GetPathByAction(HttpContext, nameof(Get), values: new { idOrSlug = movie.Id }) ?? string.Empty,
                Rel = "self",
                Type = "GET"
            });

            movieResponse.Links.Add(new Link
            {
                Href = linkGenerator.GetPathByAction(HttpContext, nameof(Update), values: new { idOrSlug = movie.Id }) ?? string.Empty,
                Rel = "self",
                Type = "PUT"
            });

            movieResponse.Links.Add(new Link
            {
                Href = linkGenerator.GetPathByAction(HttpContext, nameof(Delete), values: new { idOrSlug = movie.Id }) ?? string.Empty,
                Rel = "self",
                Type = "DELETE"
            });

            return Ok(movieResponse); 
        }

        [Authorize]
        [HttpGet(ApiEndpoints.Movies.GetAll)]
        public async Task<IActionResult> GetAll([FromQuery]GetAllMoviesRequest getAllMoviesRequest, CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();

            var options = getAllMoviesRequest.ToOptions().WithUserId(userId!.Value);

            var movies = await movieService.GetAllAsync(options, cancellationToken);

            var movieCount = await movieService.GetCountAsync(options.Title, options.YearOfRelease, cancellationToken);

            return Ok(movies.ToResponse(getAllMoviesRequest.Page, getAllMoviesRequest.PageSize, movieCount));
        }
         
        [Authorize(AuthConstants.TrustedMemberPolicyName)]
        [HttpPut(ApiEndpoints.Movies.Update)]
        public async Task<IActionResult> Update([FromRoute]Guid id, [FromBody]UpdateMovieRequest movieRequest, CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();

            var movie = movieRequest.ToMovie(id); 

            var updatedMovie = await movieService.UpdateAsync(movie, userId, cancellationToken);

            return updatedMovie is not null ? Ok(movie.ToResponse()) : NotFound(); 
        }

        [Authorize(AuthConstants.AdminUserPolicyName)]
        [HttpDelete(ApiEndpoints.Movies.Delete)]
        public async Task<IActionResult> Delete([FromRoute]Guid id, CancellationToken cancellationToken)
        {
            var deleted = await movieService.DeleteByIdAsync(id, cancellationToken);

            return deleted ? Ok() : NotFound(); 
        }
    }
}
