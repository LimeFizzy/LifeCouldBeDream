using Serilog;
using API.Data;
using API.Services;
using API.Models;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features;
using System.Diagnostics.CodeAnalysis;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Warning()
    .WriteTo.Async(a => a.File("logs/app_log.txt", rollingInterval: RollingInterval.Day))
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 21))));

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024;
});

builder.Services.AddScoped<IUserScoreService, UserScoreService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPictureUploadService, PictureUploadService>();
builder.Services.AddScoped<IUnifiedGamesService<int>, UnifiedGamesService<int>>();
builder.Services.AddScoped<IUnifiedGamesService<Square>, UnifiedGamesService<Square>>();
builder.Services.AddScoped<IUnifiedGamesService<SquareChimp>, UnifiedGamesService<SquareChimp>>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontendApp");
app.MapControllers();
app.UseStaticFiles();

app.Run();

[ExcludeFromCodeCoverage]
public partial class Program { }
