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

        [HttpPost(ApiEndpoints.Movies.Create)]
        public async Task<IActionResult> Create([FromBody] CreateMovieRequest request)
        {
            var movie = request.ToMovie();

            _ = await movieService.CreateAsync(movie);

            var response = movie.ToResponse();

            return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie); 
        }

        [HttpGet(ApiEndpoints.Movies.Get)]
        public async Task<IActionResult> Get([FromRoute]string idOrSlug)
        {
            var movie = Guid.TryParse(idOrSlug, out var id) ? await movieService.GetByIdAsync(id) 
                                                            : await movieService.GetBySlugAsync(idOrSlug);

            return movie is null ? NotFound() : Ok(movie.ToResponse()); 
        }

        [HttpGet(ApiEndpoints.Movies.GetAll)]
        public async Task<IActionResult> GetAll()
        { 
            var movies = await movieService.GetAllAsync();

            return Ok(movies.ToResponse());
        }

        [HttpPut(ApiEndpoints.Movies.Update)]
        public async Task<IActionResult> Update([FromRoute]Guid id, [FromBody]UpdateMovieRequest movieRequest)
        {
            var movie = movieRequest.ToMovie(id); 

            var updatedMovie = await movieService.UpdateAsync(movie);

            return updatedMovie is not null ? Ok(movie.ToResponse()) : NotFound(); 
        }

        [HttpDelete(ApiEndpoints.Movies.Delete)]
        public async Task<IActionResult> Delete([FromRoute]Guid id)
        {
            var deleted = await movieService.DeleteByIdAsync(id);

            return deleted ? Ok() : NotFound(); 
        }
    }
}
