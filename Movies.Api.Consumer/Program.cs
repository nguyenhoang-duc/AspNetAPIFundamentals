using Microsoft.Extensions.DependencyInjection;
using Movies.Api.Consumer;
using Movies.Api.Sdk;
using Movies.Contracts.Requests;
using Refit;
using System.Text.Json;

var services = new ServiceCollection();

services
    .AddHttpClient()
    .AddSingleton<AuthTokenProvider>()
    .AddRefitClient<IMoviesApi>(s => new RefitSettings
{
    AuthorizationHeaderValueGetter = (_, _) => s.GetRequiredService<AuthTokenProvider>().GetTokenAsync()
})
.ConfigureHttpClient(x => x.BaseAddress = new Uri("https://localhost:5001")); 

var provider = services.BuildServiceProvider();

var moviesApi = provider.GetRequiredService<IMoviesApi>();

var newMovie = await moviesApi.CreateMovieAsync(new CreateMovieRequest() 
{ 
    Title = "Spiderman 2",
    YearOfRelease = 2002,
    Genres = new[] { "Action" }
});

await moviesApi.UpdateMovieAsync(newMovie.Id, new UpdateMovieRequest
{
    Title = "Spiderman 2",
    YearOfRelease = 2002,
    Genres = new[] { "Action", "Adventure" }
});

var request = new GetAllMoviesRequest
{
    Title = null,
    Year = null,
    SortBy = null,
    Page = 1,
    PageSize = 5,
};

var movies = await moviesApi.GetMoviesAsync(request);

foreach (var movie in movies.Items)
{ 
    Console.WriteLine(JsonSerializer.Serialize(movie));
}