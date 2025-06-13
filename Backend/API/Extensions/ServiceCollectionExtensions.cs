using Application.Services;
using Core.Constants;
using Core.Entities;
using Core.Interfaces;
using Duende.IdentityServer.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
            );
            services.AddDbContext<FirstDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("FirstDatabase"))
            );
            services.AddDbContext<SecondDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("SecondDatabase"))
            );

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IUserAdminService, UserAdminService>();
            services.AddScoped<ProductService>();

            services.AddAutoMapper(typeof(Application.Mappings.MappingProfile).Assembly);

            // Identity
            services
                .AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ManageUsers", policy => policy.RequireRole("Admin"));
                options.AddPolicy("ViewReports", policy => policy.RequireRole("Admin", "Manager"));
                options.AddPolicy(
                    "GeneralUser",
                    policy => policy.RequireRole("Admin", "Manager", "User", "Guest")
                );
            });

            // Authentication
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = configuration["IdentityServer:Authority"];
                    options.Audience = "api1"; // ชื่อตรงกับ ApiResource/ApiScope
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false, // หรือ true ถ้าต้องการตรวจสอบ audience
                    };
                });

            // IdentityServer
            services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowFrontend",
                    policy =>
                        policy
                            .WithOrigins(configuration["Cors:AllowedOrigins"])
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials() // Allow sending credentials (cookies, authorization headers, etc.)
                );
            });

            services
                .AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(configuration["Keys:Path"]));

            return services;
        }
    }
}
