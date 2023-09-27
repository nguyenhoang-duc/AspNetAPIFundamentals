namespace Movies.Contracts.Responses
{
    public class MovieRatingsResponse
    {
        public IEnumerable<MovieRatingResponse> Ratings { get; set; } = Enumerable.Empty<MovieRatingResponse>();
    }
}
