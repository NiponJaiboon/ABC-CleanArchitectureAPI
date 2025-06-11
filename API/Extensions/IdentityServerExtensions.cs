using Application.Services;
using Core.Constants;
using Core.Constants;
using Core.Entities;
using Core.Entities;
using Core.Interfaces;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models;

namespace API.Extensions
{
    public static class IdentityServerExtensions
    {
        public static IServiceCollection AddIdentityServerConfiguration(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services
                .AddIdentityServer(options =>
                {
                    options.EmitStaticAudienceClaim = true;
                    options.KeyManagement.Enabled = true;
                })
                .AddAspNetIdentity<ApplicationUser>()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseNpgsql(
                            configuration.GetConnectionString("DefaultConnection"),
                            npgsql => npgsql.MigrationsAssembly("Infrastructure")
                        );
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseNpgsql(
                            configuration.GetConnectionString("DefaultConnection"),
                            npgsql => npgsql.MigrationsAssembly("Infrastructure")
                        );
                });

            return services;
        }
    }
}
