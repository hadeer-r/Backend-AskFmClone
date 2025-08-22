using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AskFm.DAL;
using AskFm.DAL.Models;
using AskFm.DAL.Interfaces;
using AskFm.DAL.Repositories;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore.Proxies;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using AskFm.BLL.Services.UserIdentityService;
using Castle.Components.DictionaryAdapter.Xml;

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

        JwtOptions jwtOptions = new JwtOptions
        {
            Issuer = Environment.GetEnvironmentVariable("ISSUER"),
            Audience = Environment.GetEnvironmentVariable("AUDIENCE"),
            SigningKey = Environment.GetEnvironmentVariable("SIGNINGKEY"),
        };
        if (jwtOptions == null)
        {
            throw new Exception("jwtOptions is null");
        }
        
        Console.WriteLine(jwtOptions.Issuer +  " " + jwtOptions.Audience +  " " + jwtOptions.SigningKey);
        builder.Services.AddAuthentication()
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, Options =>
            {
                Options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey))
                };

            });
            
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