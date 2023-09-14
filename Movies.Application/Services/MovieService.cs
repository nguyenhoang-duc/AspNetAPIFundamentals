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

        public async Task<bool> CreateAsync(Movie movie)
        {
            await movieValidator.ValidateAndThrowAsync(movie);

            return await movieRepository.CreateAsync(movie);
        }

        public Task<bool> DeleteByIdAsync(Guid id)
        {
            return movieRepository.DeleteByIdAsync(id);
        }

        public Task<IEnumerable<Movie>> GetAllAsync()
        {
            return movieRepository.GetAllAsync();
        }

        public Task<Movie?> GetByIdAsync(Guid id)
        {
            return movieRepository.GetByIdAsync(id);    
        }

        public Task<Movie?> GetBySlugAsync(string slug)
        {
            return movieRepository.GetBySlugAsync(slug);
        }

        public async Task<Movie?> UpdateAsync(Movie movie)
        {
            await movieValidator.ValidateAndThrowAsync(movie);

            var movieExists = await movieRepository.ExistsByIdAsync(movie.Id);

            if (!movieExists)
            {
                return null; 
            }

            var success = await movieRepository.UpdateAsync(movie);

            return success ? movie : null; 
        }
    }
}
