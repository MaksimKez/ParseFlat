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
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));
builder.Services.Configure<AuthOptions>(
    builder.Configuration.GetSection(AuthOptions.SectionName));
builder.Services.Configure<UserProfileClientSettings>(builder.Configuration.GetSection(UserProfileClientSettings.SectionName));
builder.Services.Configure<NotificationClientSettings>(builder.Configuration.GetSection(NotificationClientSettings.SectionName));

builder.Services.AddInfrastructure();
builder.Services.AddPersistence(builder.Configuration);
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
