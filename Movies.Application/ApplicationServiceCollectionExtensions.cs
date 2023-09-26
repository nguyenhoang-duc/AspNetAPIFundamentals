using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Database;
using Movies.Application.Repositories;
using Movies.Application.Services;

namespace Movies.Application
{
    public static class ApplicationServiceCollectionExtensions
    {
        // in order to tuse the IserviceCollection, the microsoft dependy injection nuget packages has to be used 
        public static IServiceCollection AddApplication(this IServiceCollection services)
        { 
            services.AddSingleton<IMovieRepository, MovieRepository>(); // defines connection to the database
            services.AddSingleton<IMovieService, MovieService>(); // defines the business logic for the movies
            services.AddSingleton<IRatingRepository, RatingRepository>();
            services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton);

            return services; 
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<IDBConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));
            services.AddSingleton<DbInitializer>();

            return services; 
        }
    }
}
