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
        private readonly IValidator<Movie> movieValidator;

        public MovieService(IMovieRepository movieRepository, IValidator<Movie> movieValidator)
        {
            this.movieRepository = movieRepository;
            this.movieValidator = movieValidator;
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

        public Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token = default)
        {
            return movieRepository.GetAllAsync(token);
        }

        public Task<Movie?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return movieRepository.GetByIdAsync(id, token);    
        }

        public Task<Movie?> GetBySlugAsync(string slug, CancellationToken token = default)
        {
            return movieRepository.GetBySlugAsync(slug, token);
        }

        public async Task<Movie?> UpdateAsync(Movie movie, CancellationToken token = default)
        {
            await movieValidator.ValidateAndThrowAsync(movie, token);

            var movieExists = await movieRepository.ExistsByIdAsync(movie.Id, token);

            if (!movieExists)
            {
                return null; 
            }

            var success = await movieRepository.UpdateAsync(movie, token);

            return success ? movie : null; 
        }
    }
}
