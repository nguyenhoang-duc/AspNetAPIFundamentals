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

        public async Task<bool> CreateAsync(Movie movie)
        {
            using var connection = await dbConnectionFactory.CreateConnectionAsync();
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
                        """, new { MovieId = movie.Id, Name = genre }));
                }
            }

            transcation.Commit();

            return result > 0;
        }

        public async Task<Movie?> GetByIdAsync(Guid id)
        {
            using var connection = await dbConnectionFactory.CreateConnectionAsync();

            var movie = await connection.QuerySingleOrDefaultAsync<Movie>("""Select * from movies where id = @id""", new { id });

            if (movie is null)
            {
                return null; 
            }

            var genres = await connection.QueryAsync<string>("""Select name from genres where movieid = @id""", new { id });

            movie.Genres.AddRange(genres);

            return movie;
        }

        public async Task<Movie?> GetBySlugAsync(string slug)
        {
            using var connection = await dbConnectionFactory.CreateConnectionAsync();

            var movie = await connection.QuerySingleOrDefaultAsync<Movie>("""Select * from movies where slug = @slug""", new { slug });

            if (movie is null)
            {
                return null;
            }

            var genres = await connection.QueryAsync<string>("""Select name from genres where genres.movieid = @id""", new { id = movie.Id });

            movie.Genres.AddRange(genres);

            return movie;
        }

        public async Task<IEnumerable<Movie>> GetAllAsync()
        {
            using var connection = await dbConnectionFactory.CreateConnectionAsync();

            var result = await connection.QueryAsync(new CommandDefinition("""
                Select m.*, string_agg(g.name, ',') as genres 
                from movies m left join genres g on m.id = g.movieid
                group by id
                """));

            return result.Select(x => new Movie
            {
                Id = x.id,
                Title = x.title,
                YearOfRelease = x.yearofrelease,
                Genres = Enumerable.ToList(x.genres.Split(',')),
            });
        }

        public async Task<bool> UpdateAsync(Movie movie)
        {
            using var connection = await dbConnectionFactory.CreateConnectionAsync();
            using var transaction = connection.BeginTransaction();

            await connection.ExecuteAsync("Delete from genres where movieid = @id", new { id = movie.Id });

            foreach (var genre in movie.Genres)
            {
                await connection.ExecuteAsync("Insert into genres (movieid, name) Values (@movieId, @name)", new { movieId = movie.Id, name = genre }); 
            }

            var result = await connection.ExecuteAsync("Update movies Set slug=@slug, title=@title, yearofrelease=@yearofrelease where id=@id",
                                                       new { slug = movie.Slug, title = movie.Title, yearofrelease = movie.YearOfRelease, id = movie.Id }); 

            transaction.Commit();

            return result > 0; 
        }

        public async Task<bool> DeleteByIdAsync(Guid id)
        {
            using var connection = await dbConnectionFactory.CreateConnectionAsync();
            using var transaction = connection.BeginTransaction(); 

            await connection.ExecuteAsync("Delete from genres where movieId = @id", new { id }); 
            
            var result = await connection.ExecuteAsync("Delete from movies where id = @id", new { id });

            transaction.Commit(); 

            return result > 0; 
        }

        public async Task<bool> ExistsByIdAsync(Guid id)
        {
            using var connection = await dbConnectionFactory.CreateConnectionAsync();

            return await connection.ExecuteScalarAsync<bool>("""
                Select count(1) from movies where id = @id
                """, new { id }); 
        }
    }
}
