using Api.DataAccess;
using Api.DataAccess.Interfaces;
using Api.DataAccess.Repositories;
using Api.Services.Interfaces;
using Api.Services.Services;
using Microsoft.EntityFrameworkCore;

namespace Notenmanagement.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                policy =>
                {
                    policy.WithOrigins("https://localhost:7199")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
        });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<AppDBContext>(options =>
                options.UseMySql(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                ServerVersion.AutoDetect(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                )
            )
        );

        builder.Services.AddScoped<IAuthRepository, AuthRepository>();
        builder.Services.AddScoped<IAuthService, AuthService>();


        builder.Services.AddScoped<IGradesRepository, GradeRepository>();
        builder.Services.AddScoped<IGradeService, GradeService>();


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseCors("AllowFrontend");

        app.MapControllers();

        app.Run();
    }
}
