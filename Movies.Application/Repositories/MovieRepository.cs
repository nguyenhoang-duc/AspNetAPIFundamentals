using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Transactions;

namespace Movies.Application.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly IDBConnectionFactory dbConnectionFactory;

        public MovieRepository(IDBConnectionFactory dbConnectionFactory)
        {
            this.dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
        {
            using var connection = await dbConnectionFactory.CreateConnectionAsync(token);
            using var transcation = connection.BeginTransaction();

            var result = await connection.ExecuteAsync(new CommandDefinition("""
                insert into movies (id, slug, title, yearofrelease)
                values (@Id, @Slug, @Title, @YearOfRelease)
                """, movie));

            if (result > 0)
            {
                foreach (var genre in movie.Genres)
                {
                    await connection.ExecuteAsync(new CommandDefinition("""
                        insert into genres (movieId, name)
                        values (@MovieId, @Name)
                        """, new { MovieId = movie.Id, Name = genre }, cancellationToken: token));
                }
            }

            transcation.Commit();

            return result > 0;
        }

        public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            using var connection = await dbConnectionFactory.CreateConnectionAsync(token);

            var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("""Select * from movies where id = @id""", new { id }, cancellationToken: token));

            if (movie is null)
            {
                return null; 
            }

            var genres = await connection.QueryAsync<string>(new CommandDefinition("""Select name from genres where movieid = @id""", new { id }, cancellationToken: token));

            movie.Genres.AddRange(genres);

            return movie;
        }

        public async Task<Movie?> GetBySlugAsync(string slug, CancellationToken token = default)
        {
            using var connection = await dbConnectionFactory.CreateConnectionAsync(token);

            var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("""Select * from movies where slug = @slug""", new { slug }, cancellationToken: token));

            if (movie is null)
            {
                return null;
            }

            var genres = await connection.QueryAsync<string>(new CommandDefinition("""Select name from genres where genres.movieid = @id""", new { id = movie.Id }, cancellationToken: token));

            movie.Genres.AddRange(genres);

            return movie;
        }

        public async Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token = default)
        {
            using var connection = await dbConnectionFactory.CreateConnectionAsync(token);

            var result = await connection.QueryAsync(new CommandDefinition("""
                Select m.*, string_agg(g.name, ',') as genres 
                from movies m left join genres g on m.id = g.movieid
                group by id
                """, cancellationToken: token));

            return result.Select(x => new Movie
            {
                Id = x.id,
                Title = x.title,
                YearOfRelease = x.yearofrelease,
                Genres = Enumerable.ToList(x.genres.Split(',')),
            });
        }

        public async Task<bool> UpdateAsync(Movie movie, CancellationToken token = default)
        {
            using var connection = await dbConnectionFactory.CreateConnectionAsync(token);
            using var transaction = connection.BeginTransaction();

            await connection.ExecuteAsync(new CommandDefinition("Delete from genres where movieid = @id", new { id = movie.Id }, cancellationToken: token));

            foreach (var genre in movie.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition("Insert into genres (movieid, name) Values (@movieId, @name)", new { movieId = movie.Id, name = genre }, cancellationToken: token)); 
            }

            var result = await connection.ExecuteAsync(new CommandDefinition("Update movies Set slug=@slug, title=@title, yearofrelease=@yearofrelease where id=@id",
                                                       new { slug = movie.Slug, title = movie.Title, yearofrelease = movie.YearOfRelease, id = movie.Id }, cancellationToken: token)); 

            transaction.Commit();

            return result > 0; 
        }

        public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
        {
            using var connection = await dbConnectionFactory.CreateConnectionAsync(token);
            using var transaction = connection.BeginTransaction(); 

            await connection.ExecuteAsync(new CommandDefinition("Delete from genres where movieId = @id", new { id }, cancellationToken: token)); 
            
            var result = await connection.ExecuteAsync(new CommandDefinition("Delete from movies where id = @id", new { id }, cancellationToken: token));

            transaction.Commit(); 

            return result > 0; 
        }

        public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
        {
            using var connection = await dbConnectionFactory.CreateConnectionAsync(token);

            return await connection.ExecuteScalarAsync<bool>(new CommandDefinition("""
                Select count(1) from movies where id = @id
                """, new { id }, cancellationToken: token)); 
        }
    }
}
