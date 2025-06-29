using System.Security.Claims;
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
            // Add DbContexts
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
            );
            services.AddDbContext<FirstDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("FirstDatabase"))
            );
            services.AddDbContext<SecondDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("SecondDatabase"))
            );

            // Register Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register repositories
            services.AddScoped<IProductRepository, ProductRepository>();

            // Register services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IUserAdminService, UserAdminService>();
            services.AddScoped<ProductService>();
            services.AddScoped<FileStorageService>();

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

            // Read the IdentityServer Authority URL from configuration
            var identityServerAuthority = configuration["IdentityServer:Authority"];
            if (string.IsNullOrEmpty(identityServerAuthority))
            {
                throw new InvalidOperationException(
                    "IdentityServer:Authority configuration is missing."
                );
            }

            // Authentication
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = identityServerAuthority;
                    options.Audience = configuration["Jwt:Audience"] ?? "api1"; // Default to "api1" if not set
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = true,
                        ValidIssuer = identityServerAuthority,
                        RoleClaimType = ClaimTypes.Role,
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
