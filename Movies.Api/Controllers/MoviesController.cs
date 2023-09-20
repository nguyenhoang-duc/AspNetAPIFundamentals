using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Application.Repositories;
using Movies.Application.Services;
using Movies.Contracts.Requests;

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

        [Authorize(AuthConstants.AdminUserPolicyName)]
        [HttpPost(ApiEndpoints.Movies.Create)]
        public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken cancellationToken)
        {
            var movie = request.ToMovie();

            _ = await movieService.CreateAsync(movie, cancellationToken);

            var response = movie.ToResponse();

            return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie); 
        }

        [HttpGet(ApiEndpoints.Movies.Get)]
        public async Task<IActionResult> Get([FromRoute]string idOrSlug, CancellationToken cancellationToken)
        {
            var movie = Guid.TryParse(idOrSlug, out var id) ? await movieService.GetByIdAsync(id, cancellationToken) 
                                                            : await movieService.GetBySlugAsync(idOrSlug, cancellationToken);

            return movie is null ? NotFound() : Ok(movie.ToResponse()); 
        }

        [HttpGet(ApiEndpoints.Movies.GetAll)]
        public async Task<IActionResult> GetAll()
        { 
            var movies = await movieService.GetAllAsync();

            return Ok(movies.ToResponse());
        }

        // Allow only admins to update a movie 
        [Authorize(AuthConstants.AdminUserPolicyName)]
        [HttpPut(ApiEndpoints.Movies.Update)]
        public async Task<IActionResult> Update([FromRoute]Guid id, [FromBody]UpdateMovieRequest movieRequest, CancellationToken cancellationToken)
        {
            var movie = movieRequest.ToMovie(id); 

            var updatedMovie = await movieService.UpdateAsync(movie, cancellationToken);

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
