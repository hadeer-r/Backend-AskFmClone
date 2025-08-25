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
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddDbContext<AppDbContext>(options =>
            options
                .UseLazyLoadingProxies()
                .UseSqlServer(ConnectionString));
        
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        

        JwtOptions jwtOptions = new JwtOptions
        {
            Issuer = Environment.GetEnvironmentVariable("ISSUER"),
            Audience = Environment.GetEnvironmentVariable("AUDIENCE"),
            SigningKey = Environment.GetEnvironmentVariable("SIGNINGKEY"),
            AccessExpiration = int.Parse(Environment.GetEnvironmentVariable("TOKEN_EXP")),
            AccessRefreshTokenExpiration = int.Parse(Environment.GetEnvironmentVariable("REFRESH_TOKEN_EXP")),
        };
        if (jwtOptions == null)
        {
            throw new Exception("jwtOptions is null");
        }

        builder.Services.Configure<JwtOptions>(Options =>
        {
            Options.Issuer = Environment.GetEnvironmentVariable("ISSUER");
            Options.Audience = Environment.GetEnvironmentVariable("AUDIENCE");
            Options.SigningKey = Environment.GetEnvironmentVariable("SIGNINGKEY");
            Options.AccessExpiration = int.Parse(Environment.GetEnvironmentVariable("TOKEN_EXP"));
            Options.AccessRefreshTokenExpiration = int.Parse(Environment.GetEnvironmentVariable("REFRESH_TOKEN_EXP"));
        });
        
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                
            })
            .AddJwtBearer( Options =>
            {
                Options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
                    ClockSkew = TimeSpan.FromMinutes(0)
                };

            });
        builder.Services.AddAuthorization();
        builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
            {
                //password configuration
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                
                //Email
                options.User.RequireUniqueEmail = true;
                
                // Lockout 
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.AllowedForNewUsers = true;
                
                // sign in options
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                /*
                 * close confirmed email imediatly in register,
                 * but in other scenario we will block some action untill the user verify his email
                 */
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
            
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        

        app.MapControllers();

        app.Run();
    }
}