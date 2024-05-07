using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers
{
    [ApiVersion(1.0)]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        /*private readonly IRatingService ratingService;

        public RatingsController(IRatingService ratingService) 
        {
            this.ratingService = ratingService;
        }

        [Authorize]
        [HttpPut(ApiEndpoints.Movies.Rate)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RateMovie([FromRoute] Guid id, [FromBody] RateMovieRequest request, CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();

            var success = await ratingService.RateMovieAsync(id, request.Rating, userId!.Value, cancellationToken);

            return success ? Ok() : NotFound();
        }

        [Authorize]
        [HttpDelete(ApiEndpoints.Movies.DeleteRating)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRating([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();

            var success = await ratingService.DeleteRatingAsync(id, userId!.Value, cancellationToken); 

            return success ? Ok() : NotFound();
        }

        [Authorize]
        [HttpGet(ApiEndpoints.Ratings.GetUserRatings)]
        [ProducesResponseType(typeof(MovieRatingsResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserRatings(CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();

            var movieRatings = await ratingService.GetRatingsForUserAsync(userId!.Value, cancellationToken);

            return Ok(movieRatings.ToResponse()); 
        }*/

    }
}
