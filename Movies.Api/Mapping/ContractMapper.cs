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

        public static MoviesResponse ToResponse(this IEnumerable<Movie> movies)
        {
            return new MoviesResponse
            {
                Items = movies.Select(m => m.ToResponse()),
            };
        }
    }
}
