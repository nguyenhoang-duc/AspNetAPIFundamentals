using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly IDBConnectionFactory dbConnectionFactory;

        public RatingRepository(IDBConnectionFactory dbConnectionFactory) 
        {
            this.dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken cancellationToken = default)
        {
            using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);

            var affectedRows = await connection.ExecuteAsync(new CommandDefinition("""
                delete from ratings 
                where movieid = @movieid 
                and userid = @userid; 
                """
            , new { movieId, userId }, cancellationToken: cancellationToken));

            return affectedRows > 0; 
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

        public async Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);

            return await connection.QueryAsync<MovieRating>(new CommandDefinition("""
                    select r.movieid, m.slug, r.rating
                    from ratings r 
                    inner join movies m on r.movieid = m.id 
                    where r.userid = @userid
                """, new { userId }, cancellationToken: cancellationToken));
        }

        public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken cancellationToken = default)
        {
            using var connection = await dbConnectionFactory.CreateConnectionAsync();

            var affectedRows = await connection.ExecuteAsync(new CommandDefinition("""
                INSERT INTO ratings(userid, movieid, rating)
                VALUES (@userId, @movieId, @rating)
                on conflict (userid, movieid) do update
                    set rating = @rating
                """, new { userId, movieId, rating }, cancellationToken: cancellationToken));

            return affectedRows > 0; 
        }
    }
}
