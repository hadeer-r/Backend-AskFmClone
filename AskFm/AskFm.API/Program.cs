using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AskFm.DAL;
using AskFm.DAL.Models;
using AskFm.DAL.Interfaces;
using AskFm.DAL.Repositories;
using DotNetEnv;
using Microsoft.EntityFrameworkCore.Proxies;
using Microsoft.OpenApi.Models;

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
      
        builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
            
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}