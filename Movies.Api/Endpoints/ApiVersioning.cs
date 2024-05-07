using Asp.Versioning;
using Asp.Versioning.Builder;
using System.Runtime.CompilerServices;

namespace Movies.Api.Endpoints
{
    public static class ApiVersioning
    {
        public static ApiVersionSet VersionSet { get; private set; }

        public static IEndpointRouteBuilder CreateApiVersion(this IEndpointRouteBuilder app)
        {
            VersionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1.0))
                .HasApiVersion(new ApiVersion(2.0))
                .ReportApiVersions()
                .Build();

            return app; 
        }
    }
}
