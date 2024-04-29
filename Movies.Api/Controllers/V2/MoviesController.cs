using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;

namespace Movies.Api.Controllers.V2
{
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService movieService;

        public MoviesController(IMovieService movieService)
        {
            this.movieService = movieService;
        }

        [HttpGet(ApiEndpoints.V2.Movies.Get)]
        public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();

            var movie = Guid.TryParse(idOrSlug, out var id) ? await movieService.GetByIdAsync(id, userId, cancellationToken)
                                                            : await movieService.GetBySlugAsync(idOrSlug, userId, cancellationToken);

            return movie is null ? NotFound() : Ok(movie.ToResponse());
        }
    }
}
