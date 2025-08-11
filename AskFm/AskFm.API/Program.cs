using Microsoft.EntityFrameworkCore;
using AskFm.DAL;
using AskFm.DAL.Interfaces;
using AskFm.DAL.Repositories;
using DotNetEnv;
using Microsoft.EntityFrameworkCore.Proxies;
namespace AskFm.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        Env.Load();
        string ConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        if (ConnectionString is null)
        {
            throw new Exception("Connection string is null");
        }
        builder.Services.AddDbContext<AppDbContext>(options =>
            options
                .UseLazyLoadingProxies()
                .UseSqlServer(ConnectionString));
        
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) app.MapOpenApi();

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}