using Application.DependencyInjection;
using Infrastructure;
using Infrastructure.JWT;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

//temporary
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));

builder.Services.AddInfrastructure();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddControllers();
builder.Services.AddAuthentication();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();