using System.Text;
using Application.DependencyInjection;
using Application.Dtos.Settings;
using Infrastructure;
using Infrastructure.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Persistence;
using Api.Helpers;
using Api.Helpers.Interfaces;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));
builder.Services.Configure<AuthOptions>(
    builder.Configuration.GetSection(AuthOptions.SectionName));
builder.Services.Configure<UserProfileClientSettings>(builder.Configuration.GetSection(UserProfileClientSettings.SectionName));
builder.Services.Configure<NotificationClientSettings>(builder.Configuration.GetSection(NotificationClientSettings.SectionName));
var connStr = builder.Configuration.GetConnectionString("DefaultConnection");

var keyVaultName = builder.Configuration["KeyVault:Name"] ?? throw new InvalidOperationException("KeyVault:Name is not configured in appsettings.json");
var keyVaultUrl = new Uri($"https://{keyVaultName}.vault.azure.net/");
builder.Configuration.AddAzureKeyVault(keyVaultUrl, new DefaultAzureCredential(), new KeyVaultManager("secret"));

builder.Services.AddInfrastructure();   
builder.Services.AddPersistence(connStr);
builder.Services.AddApplication();

builder.Services.AddScoped<IAuthControllerHelper, AuthControllerHelper>();

/*builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://someurl")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});*/

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secretKey = builder.Configuration["JwtOptions:SecretKey"];
        var keyBytes = Encoding.UTF8.GetBytes(secretKey);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes), 
                    
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero 
        };
    });
        
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Write Jwt token in format: Bearer {access_token}"
    });

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
            Array.Empty<string>()
        },
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "RefreshToken"
                }
            },
            Array.Empty<string>()
        }
    });
});



builder.Services.AddControllers();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", async (IConfiguration configuration, ILogger<Program> logger) =>
{
    try
    {
        var keyVaultName = configuration["KeyVault:Name"];
        if (string.IsNullOrEmpty(keyVaultName))
        {
            throw new InvalidOperationException("KeyVault:Name is not configured");
        }
        
        var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
        var client = new SecretClient(keyVaultUri, new DefaultAzureCredential());
        var secret = await client.GetSecretAsync("secret--superSecret1234");
        
        return Results.Ok(new 
        { 
            Status = "Healthy",
            Secret = secret.Value.Value
        });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error retrieving secret from Key Vault");
        return Results.Problem("Failed to retrieve secret from Key Vault", statusCode: 500);
    }
}).WithName("HealthCheck");

app.Run();
