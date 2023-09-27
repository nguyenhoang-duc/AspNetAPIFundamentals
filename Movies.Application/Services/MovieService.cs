using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Services
{
    internal class MovieService : IMovieService
    {
        private readonly IMovieRepository movieRepository;
        private readonly IRatingRepository ratingRepository;
        private readonly IValidator<Movie> movieValidator;
        private readonly IValidator<GetAllMoviesOptions> getAllMovieOptionsValidator;

        public MovieService(IMovieRepository movieRepository, IRatingRepository ratingRepository, IValidator<Movie> movieValidator, IValidator<GetAllMoviesOptions> getAllMovieOptionsValidator)
        {
            this.movieRepository = movieRepository;
            this.ratingRepository = ratingRepository;
            this.movieValidator = movieValidator;
            this.getAllMovieOptionsValidator = getAllMovieOptionsValidator;
        }

        public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
        {
            await movieValidator.ValidateAndThrowAsync(movie, token);

            return await movieRepository.CreateAsync(movie, token);
        }

        public Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
        {
            return movieRepository.DeleteByIdAsync(id, token);
        }

        public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token = default)
        {
            await getAllMovieOptionsValidator.ValidateAndThrowAsync(options, token);

            return await movieRepository.GetAllAsync(options, token);
        }

        public Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default)
        {
            return movieRepository.GetByIdAsync(id, userId, token);    
        }

        public Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default)
        {
            return movieRepository.GetBySlugAsync(slug, userId, token);
        }

        public async Task<Movie?> UpdateAsync(Movie movie, Guid? userId = default, CancellationToken token = default)
        {
            await movieValidator.ValidateAndThrowAsync(movie, token);

            var movieExists = await movieRepository.ExistsByIdAsync(movie.Id, token);

            if (!movieExists)
            {
                return null; 
            }

            var success = await movieRepository.UpdateAsync(movie, token);

            if (!userId.HasValue)
            {
                movie.Rating = await ratingRepository.GetRatingAsync(movie.Id, token);

                return movie; 
            }

            var (rating, userRating) = await ratingRepository.GetRatingAsync(movie.Id, userId.Value, token);

            movie.Rating = rating; 
            movie.UserRating = userRating;

            return success ? movie : null; 
        }
    }
}
