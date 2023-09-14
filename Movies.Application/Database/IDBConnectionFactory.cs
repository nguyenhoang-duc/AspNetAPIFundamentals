using Npgsql;
using System.Data;

namespace Movies.Application.Database
{
    public interface IDBConnectionFactory
    {
        Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default); 
    }

    public class NpgsqlConnectionFactory : IDBConnectionFactory
    {
        private readonly string connectionString; 

        public NpgsqlConnectionFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default)
        {
            var connection = new NpgsqlConnection(connectionString); 

            await connection.OpenAsync(token);

            return connection; 
        }
    }
}
