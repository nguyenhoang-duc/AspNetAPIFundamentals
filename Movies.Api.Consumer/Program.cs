using Microsoft.Extensions.DependencyInjection;
using Movies.Api.Sdk;
using Movies.Contracts.Requests;
using Refit;
using System.Text.Json;

var services = new ServiceCollection();

services.AddRefitClient<IMoviesApi>(x => new RefitSettings
{
    AuthorizationHeaderValueGetter = (_, _) => Task.FromResult("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyYmM4MzdmYy00MzAxLTQxYTItYWJjMi1hYzZjYzYyZmI3YWEiLCJzdWIiOiJob2FuZy5kMTk5N0BnbWFpbC5jb20iLCJlbWFpbCI6ImhvYW5nLmQxOTk3QGdtYWlsLmNvbSIsInVzZXJpZCI6ImQ4NTY2ZGUzLWIxYTYtNGE5Yi1iODQyLThlMzg4N2E4MmU0MSIsImFkbWluIjp0cnVlLCJ0cnVzdGVkX21lbWJlciI6dHJ1ZSwibmJmIjoxNzE1MDI2MjYyLCJleHAiOjE3MTUwNTUwNjIsImlhdCI6MTcxNTAyNjI2MiwiaXNzIjoiaHR0cHM6Ly9pZC5uaWNrY2hhcHNhcy5jb20iLCJhdWQiOiJodHRwczovL21vdmllcy5uaWNrY2hhcHNhcy5jb20ifQ.lwq7C9QeAxqrljbYtZ7D08jDDlkFjQ2xvEjRAr_38AY")
})
.ConfigureHttpClient(x => x.BaseAddress = new Uri("https://localhost:5001")); 

var provider = services.BuildServiceProvider();

var moviesApi = provider.GetRequiredService<IMoviesApi>();

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