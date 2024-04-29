﻿namespace Movies.Contracts.Requests.V1
{
    public class CreateMovieRequest
    {
        public required string Title { get; init; } = string.Empty;

        public required int YearOfRelease { get; init; }

        public required IEnumerable<string> Genres { get; init; } = Enumerable.Empty<string>();
    }
}
