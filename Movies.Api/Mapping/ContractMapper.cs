using Movies.Application.Models;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Mapping
{
    public static class ContractMapper
    {
        public static Movie ToMovie(this CreateMovieRequest createMovieRequest)
        {
            return new Movie
            {
                Id = Guid.NewGuid(),
                Title = createMovieRequest.Title,
                YearOfRelease = createMovieRequest.YearOfRelease,
                Genres = createMovieRequest.Genres.ToList(),
            };
        }

        public static Movie ToMovie(this UpdateMovieRequest updateMovieRequest, Guid id)
        {
            return new Movie
            {
                Id = id, 
                Title = updateMovieRequest.Title,
                YearOfRelease = updateMovieRequest.YearOfRelease,
                Genres = updateMovieRequest.Genres.ToList(),
            };
        }

        public static MovieResponse ToResponse(this Movie movie)
        {
            return new MovieResponse
            {
                Id = movie.Id,
                Title = movie.Title,
                Slug = movie.Slug,
                YearOfRelease = movie.YearOfRelease,
                Rating = movie.Rating,
                UserRating = movie.UserRating,
                Genres = movie.Genres,
            };
        }

        public static MovieRatingResponse ToResponse(this MovieRating movieRating)
        {
            return new MovieRatingResponse
            {
                MovieId = movieRating.MovieId,
                Slug = movieRating.Slug,
                Rating = movieRating.Rating,
            };
        }

        public static MoviesResponse ToResponse(this IEnumerable<Movie> movies)
        {
            return new MoviesResponse
            {
                Items = movies.Select(m => m.ToResponse()),
            };
        }

        public static MovieRatingsResponse ToResponse(this IEnumerable<MovieRating> movieRatings)
        {
            return new MovieRatingsResponse
            {
                Ratings = movieRatings.Select(m => m.ToResponse()),
            };
        }

        public static GetAllMoviesOptions ToOptions(this GetAllMoviesRequest request)
        {
            var sortField = request.SortBy?.Trim('+', '-');
            var sortOrder = request.SortBy is null ? SortOrder.Unsorted :
                            request.SortBy.StartsWith("-") ? SortOrder.Descending : SortOrder.Ascending;

            return new GetAllMoviesOptions
            {
                YearOfRelease = request.Year, 
                Title = request.Title,
                SortField = sortField, 
                SortOrder = sortOrder,
            };
        }

        public static GetAllMoviesOptions WithUserId(this GetAllMoviesOptions options, Guid userId)
        { 
            options.UserId = userId;

            return options;
        }
    }
}
