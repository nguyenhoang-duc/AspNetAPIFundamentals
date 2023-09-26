using Dapper;
using Movies.Application.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly IDBConnectionFactory dbConnectionFactory;

        public RatingRepository(IDBConnectionFactory dbConnectionFactory) 
        {
            this.dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<float?> GetRatingAsync(Guid movieId, CancellationToken cancellationToken = default)
        {
            using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);

            return await connection.QuerySingleOrDefaultAsync<float?>(new CommandDefinition("""
                    select round(avg(r.rating), 1) 
                    from ratings r 
                    where movieId = @movieId
                """, new { movieId }, cancellationToken: cancellationToken));
        }

        public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId, CancellationToken cancellationToken = default)
        {
            using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);

            return await connection.QuerySingleOrDefaultAsync<(float?, int?)>(new CommandDefinition("""
                    select round(avg(r.rating), 1), 
                         (select r.rating 
                          from ratings r 
                          where movieId = @movieId and userid = @userId
                          limit 1) 
                    from ratings r 
                    where movieId = @movieId
                """, new { movieId, userId }, cancellationToken: cancellationToken));
        }
    }
}
