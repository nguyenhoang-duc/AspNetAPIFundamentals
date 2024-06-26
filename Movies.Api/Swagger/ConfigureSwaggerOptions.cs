﻿using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Movies.Api.Swagger
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        // contains description about all api versions 
        private readonly IApiVersionDescriptionProvider provider;
        private readonly IHostEnvironment environment;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IHostEnvironment environment)
        {
            this.provider = provider;
            this.environment = environment;
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach(var description in provider.ApiVersionDescriptions) 
            {
                options.SwaggerDoc(description.GroupName, new OpenApiInfo
                {
                    Title = environment.ApplicationName,
                    Version = description.ApiVersion.ToString(),
                });
            }

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header, 
                Description = "Please provide a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        }
    }
}
