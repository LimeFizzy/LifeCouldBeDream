using API.Services;
using API.Data;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 2.3. Logging most exceptions 
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() // minimum logging level to Information
    .WriteTo.Console()          // log to console and to a file
    .WriteTo.File("logs/app_log.txt", rollingInterval: RollingInterval.Day) // new log file every day
    .CreateLogger();

// Serilog as the logging provider (replaces the default .NET logging provider)
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
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // Set to 10 MB
});

builder.Services.AddScoped<ILongNumberService, LongNumberService>();
builder.Services.AddScoped<ISequenceService, SequenceService>();
builder.Services.AddScoped<IUserScoreService, UserScoreService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPictureUploadService, PictureUploadService>();

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

Log.CloseAndFlush();

public partial class Program { }
