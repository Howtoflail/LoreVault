using LoreVault.DAL;
using LoreVault.Domain.Authorization;
using LoreVault.Domain.Interfaces;
using LoreVault.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Auth0
var auth0Settings = builder.Configuration.GetSection("Auth0SPA");
var domain = auth0Settings["Domain"];
var audience = auth0Settings["Audience"];
var clientId = auth0Settings["ClientId"];
var clientSecret = auth0Settings["ClientSecret"];

//Frontend
var frontendSettings = builder.Configuration.GetSection("Frontend");
var frontendDomain = frontendSettings["Domain"];

// Define CORS policy
builder.Services.AddCors(options => 
{
    options.AddPolicy("AllowFrontendOrigin", policy => 
    {
        policy.WithOrigins("http://localhost:3000", frontendDomain!)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = $"https://{domain}/";
    options.Audience = audience;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidIssuer = $"https://{domain}/",
        ValidAudience = audience,
        NameClaimType = ClaimTypes.NameIdentifier
    };
});

builder.Services.AddAuthorization(options =>
{
    /*options.AddPolicy("read:messages",
        policy => policy.Requirements.Add(
            new HasScopeRequirement("read:messages", domain)
            )
        );*/

    options.AddPolicy("read:users",
        policy => policy.Requirements.Add(
            new HasScopeRequirement("read:users", $"https://{domain}/")
            )
        );
});

builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

// Bind the data sections from appsettings.json
builder.Services.Configure<CosmosDbSettings>(builder.Configuration.GetSection("CosmosDb"));
builder.Services.Configure<Auth0Settings>(builder.Configuration.GetSection("Auth0SPA"));

// Dependency injection
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDataService, DataService>();
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontendOrigin");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
