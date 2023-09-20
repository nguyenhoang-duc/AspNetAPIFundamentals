using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Movies.Api.Mapping;
using Movies.Application;
using Movies.Application.Database;
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

builder.Services.AddAuthorization(); 

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();    
app.UseAuthorization();

app.UseMiddleware<ValidationMappingMiddleware>(); 
app.MapControllers();

var dbInitializer = app.Services.GetRequiredService<DbInitializer>();   
await dbInitializer.InitializeAsync();

app.Run();
