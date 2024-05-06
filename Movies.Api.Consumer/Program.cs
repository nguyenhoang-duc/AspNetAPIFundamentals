using Movies.Api.Sdk;
using Refit;
using System.Text.Json;

var moviesApi = RestService.For<IMoviesApi>("https://localhost:5001");

var movie = await moviesApi.GetMoviesAsync("test-3-2023");

Console.WriteLine(JsonSerializer.Serialize(movie));