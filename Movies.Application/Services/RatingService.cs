﻿using FluentValidation.Results;
using Movies.Application.Repositories;

namespace Movies.Application.Services
{
    internal class RatingService : IRatingService
    {
        private readonly IRatingRepository ratingRepository;
        private readonly IMovieRepository movieRepository;

        public RatingService(IRatingRepository ratingRepository, IMovieRepository movieRepository)
        {
            this.ratingRepository = ratingRepository;
            this.movieRepository = movieRepository;
        }

        public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken cancellationToken = default)
        {
            if (rating is <= 0 or > 5)
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new ValidationFailure
                    {
                        PropertyName = "Rating",
                        ErrorMessage = "Rating must be between 1 and 5",
                    }
                }); 
            }

            if (!(await movieRepository.ExistsByIdAsync(movieId, cancellationToken)))
            {
                return false; 
            }

            return await ratingRepository.RateMovieAsync(movieId, rating, userId, cancellationToken);
        }
    }
}
