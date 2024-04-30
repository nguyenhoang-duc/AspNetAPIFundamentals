using Microsoft.Extensions.Diagnostics.HealthChecks;
using Movies.Application.Database;

namespace Movies.Api.Health
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly IDBConnectionFactory connectionFactory;
        private readonly ILogger logger;

        public DatabaseHealthCheck(IDBConnectionFactory connectionFactory, ILogger logger) 
        {
            this.connectionFactory = connectionFactory;
            this.logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try 
            {
                _ = await connectionFactory.CreateConnectionAsync();

                return HealthCheckResult.Healthy(); 
            }
            catch (Exception ex) 
            {
                logger.LogError("Database is unhealthy.", ex);

                return HealthCheckResult.Unhealthy("Database is unhealthy."); 
            }
        }
    }
}
