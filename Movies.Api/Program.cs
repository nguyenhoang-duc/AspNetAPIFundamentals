using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Movies.Api.Auth;
using Movies.Api.Endpoints;
using Movies.Api.Health;
using Movies.Api.Mapping;
using Movies.Api.Swagger;
using Movies.Application;
using Movies.Application.Database;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add authentication 
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x => 
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
        ValidateIssuerSigningKey = true, // has to be set other wise the signer is not validated 
        ValidateLifetime = true, // check the liftime of the JWT, if expired should also expire permissions 
        ValidAudience = config["Jwt:Audience"]!,
        ValidIssuer = config["Jwt:Issuer"],
        ValidateIssuer = true, // is this coming from the right person ?
        ValidateAudience = true, // is this token targeted to the right API ?
    };
});

builder.Services.AddAuthorization(x => 
{
    // add rules based on information inside the JWT
    // here a user is an admin, if the claim admin is set to true
    // x.AddPolicy(AuthConstants.AdminUserPolicyName, p => p.RequireClaim(AuthConstants.AdminUserClaimName, "true"));

    // Adding a requirement allows to have more compolex authorization 
    x.AddPolicy(AuthConstants.AdminUserPolicyName, p => p.AddRequirements(new AdminAuthRequirement(config["ApiKey"]!)));

    // Require Assertion allows for more complex policy description 
    // In this case a trusted member is an admin or a user with trusted member claim
    x.AddPolicy(AuthConstants.TrustedMemberPolicyName, p => p.RequireAssertion(c =>
        c.User.HasClaim(m => m is { Type: AuthConstants.TrustedMemberClaimName, Value: "true"}) ||
        c.User.HasClaim(m => m is { Type: AuthConstants.AdminUserClaimName, Value: "true" })
    ));
});

// Add services to the container.
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1.0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new MediaTypeApiVersionReader("api-version");
}).AddApiExplorer();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("Database");

builder.Services.AddOutputCache(x => 
{
    x.AddBasePolicy(c => c.Cache());
    x.AddPolicy("MoviesCache", c =>
    {
        c.Cache().Expire(TimeSpan.FromMinutes(1))
         .SetVaryByQuery(new[] { "title", "year", "sortBy", "page", "pageSize" })
         .Tag("movies");
    });
}); 
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen(x => x.OperationFilter<SwaggerDefaultValues>());

// adds the application layer 
// this method is defined in the application layer, because the API should not know which services to add 
// in order to use the application layer 
builder.Services.AddApplication();

builder.Services.AddDatabase(config["Database:ConnectionString"]!); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); 
    app.UseSwaggerUI(x => 
    {
        foreach (var description in app.DescribeApiVersions())
        {
            x.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName);
        }
    });
}

app.MapHealthChecks("_health");

app.UseHttpsRedirection();

app.UseAuthentication();    
app.UseAuthorization();

//app.UseCors(); 
app.UseOutputCache(); 

app.UseMiddleware<ValidationMappingMiddleware>();

app.MapApiEndpoints(); 

var dbInitializer = app.Services.GetRequiredService<DbInitializer>();   
await dbInitializer.InitializeAsync();

app.Run();
