using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests.V1;

namespace Movies.Api.Controllers.V1
{
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly IRatingService ratingService;

        public RatingsController(IRatingService ratingService)
        {
            this.ratingService = ratingService;
        }

        [Authorize]
        [HttpPut(ApiEndpoints.V1.Movies.Rate)]
        public async Task<IActionResult> RateMovie([FromRoute] Guid id, [FromBody] RateMovieRequest request, CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();

            var success = await ratingService.RateMovieAsync(id, request.Rating, userId!.Value, cancellationToken);

            return success ? Ok() : NotFound();
        }

        [Authorize]
        [HttpDelete(ApiEndpoints.V1.Movies.DeleteRating)]
        public async Task<IActionResult> DeleteRating([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();

            var success = await ratingService.DeleteRatingAsync(id, userId!.Value, cancellationToken);

            return success ? Ok() : NotFound();
        }

        [Authorize]
        [HttpGet(ApiEndpoints.V1.Ratings.GetUserRatings)]
        public async Task<IActionResult> GetUserRatings(CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();

            var movieRatings = await ratingService.GetRatingsForUserAsync(userId!.Value, cancellationToken);

            return Ok(movieRatings.ToResponse());
        }

    }
}
