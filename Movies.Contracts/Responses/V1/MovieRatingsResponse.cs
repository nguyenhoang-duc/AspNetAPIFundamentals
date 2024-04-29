namespace Movies.Contracts.Responses.V1
{
    public class MovieRatingsResponse
    {
        public IEnumerable<MovieRatingResponse> Ratings { get; set; } = Enumerable.Empty<MovieRatingResponse>();
    }
}
