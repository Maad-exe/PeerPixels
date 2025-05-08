using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PeerPixels.Core.Entities;
using PeerPixels.Infrastructure.Data;
using PeerPixels.Infrastructure.Repositories;
using PeerPixels.Infrastructure.Repositories.Contracts;
using PeerPixels.Infrastructure.Services.Contracts;
using PeerPixels.Infrastructure.Services;
using PeerPixels.Infrastructure.UnitOfWork;
using PeerPixels.Infrastructure.UnitOfWork.Contracts;
using System.Text;
using PeerPixels;
using dotenv.net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;



// Load environment variables from .env file
DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { ".env" }));

// Top-level statements must come first
var builder = WebApplication.CreateBuilder(args);


// Add environment variables to the configuration
builder.Configuration.AddEnvironmentVariables();

// Add services to the container
builder.Services.AddControllers();

// Add API Explorer services - THIS IS CRITICAL FOR SWAGGER
builder.Services.AddEndpointsApiExplorer();

// Configure Entity Framework
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Connection String: {connectionString}");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configure Identity
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT Key not found in configuration.");
}

var key = Encoding.UTF8.GetBytes(jwtKey);
var issuer = jwtSettings["Issuer"] ?? "PeerPixels";
var audience = jwtSettings["Audience"] ?? "PeerPixelsClient";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
})
.AddGoogle(options =>
{
    var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
    var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

    if (string.IsNullOrEmpty(googleClientId) || string.IsNullOrEmpty(googleClientSecret))
    {
        throw new InvalidOperationException(
            "Google authentication is configured but client ID or client secret is missing. " +
            "Please check your configuration settings.");
    }

    options.ClientId = googleClientId;
    options.ClientSecret = googleClientSecret;
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:4200")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials());
});

// Register Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IFollowRepository, FollowRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IFollowService, FollowService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Register AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PeerPixels API",
        Version = "v1",
        Description = "API for the PeerPixels social media application",
        Contact = new OpenApiContact
        {
            Name = "Hammad Abbas",
            Email = "hammadabbas54@gmail.com"
        }
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Use the Constants class to avoid IDE0301 warning
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Constants.EmptyStringArray
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PeerPixels API v1");
        c.RoutePrefix = "swagger"; // Keep "swagger" as the route prefix
    });
}

app.UseHttpsRedirection();
app.UseHsts();
app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Place utility classes after all top-level statements
namespace PeerPixels
{
    /// <summary>
    /// Contains constant values used throughout the application.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// An empty string array used for security requirements.
        /// </summary>
        public static readonly string[] EmptyStringArray = Array.Empty<string>();
    }
}