using API;
using Application.Services;
using Core.Constants;
using Core.Entities;
using Core.Interfaces;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.EntityFramework.Storage;
using Duende.IdentityServer.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        "Logs/Information/log-.txt",
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
        rollingInterval: RollingInterval.Day
    )
    .WriteTo.File(
        "Logs/Error/error-.txt",
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error,
        rollingInterval: RollingInterval.Day
    )
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

//// Configure Entity Framework Core with SQL Server
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// builder.Services.AddDbContext<FirstDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("FirstDatabase")));
// builder.Services.AddDbContext<SecondDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("SecondDatabase")));

// Configure Entity Framework Core with PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddDbContext<FirstDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("FirstDatabase"))
);
builder.Services.AddDbContext<SecondDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SecondDatabase"))
);

// Register unit of work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProfileService, ProfileService>();

// Register application services
builder.Services.AddScoped<ProductService>();

builder.Services.AddAutoMapper(typeof(Application.Mappings.MappingProfile).Assembly);

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ABC API", Version = "v1" });

    // เพิ่มส่วนนี้เพื่อรองรับ Bearer Token
    c.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Description =
                "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
        }
    );

    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                },
                new string[] { }
            },
        }
    );
});

// ใช้ Serilog กับ Host
builder.Host.UseSerilog();

// Configure Identity
builder
    .Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // ล็อก 5 นาที
        options.Lockout.MaxFailedAccessAttempts = 5; // ผิด 5 ครั้งจะล็อก
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// var cert = new X509Certificate2("path-to-your-certificate.pfx", "your-password");
// builder.Services.AddDataProtection()
//     .ProtectKeysWithCertificate(cert);

builder
    .Services.AddIdentityServer(options =>
    {
        options.EmitStaticAudienceClaim = true;
        options.KeyManagement.Enabled = true; // <--- ปิด Automatic Key Management ที่นี่
    })
    .AddAspNetIdentity<ApplicationUser>() // ใช้ IdentityUser สำหรับการจัดการผู้ใช้
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = b =>
            b.UseNpgsql(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsAssembly("Infrastructure") // ระบุชื่อ Assembly ที่มี Migrations
            );
    })
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = b =>
            b.UseNpgsql(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsAssembly("Infrastructure") // ระบุชื่อ Assembly ที่มี Migrations
            );
    });

// .AddTestUsers(Config.TestUsers);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManageUsers", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ViewReports", policy => policy.RequireRole("Admin", "Manager"));
    options.AddPolicy(
        "GeneralUser",
        policy => policy.RequireRole("Admin", "Manager", "User", "Guest")
    );
});

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:7256";
        options.Audience = "api1";
        options.RequireHttpsMetadata = true;
    });

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = 403;
        return Task.CompletedTask;
    };
});

// Build the app
var app = builder.Build();

// Seed IdentityServer configuration data และ Seed Role
using (var scope = app.Services.CreateScope())
{
    // Seed IdentityServer Clients, ApiScopes, IdentityResources
    var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
    if (!context.Clients.Any())
    {
        foreach (var client in Config.Clients)
        {
            context.Clients.Add(client.ToEntity());
        }
        context.SaveChanges();
    }
    if (!context.IdentityResources.Any())
    {
        foreach (var resource in Config.IdentityResources)
        {
            context.IdentityResources.Add(resource.ToEntity());
        }
        context.SaveChanges();
    }
    if (!context.ApiScopes.Any())
    {
        foreach (var scopeItem in Config.ApiScopes)
        {
            context.ApiScopes.Add(scopeItem.ToEntity());
        }
        context.SaveChanges();
    }
    if (!context.ApiResources.Any())
    {
        foreach (var resource in Config.ApiResources)
        {
            context.ApiResources.Add(resource.ToEntity());
        }
        context.SaveChanges();
    }

    // Seed Roles
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var role in RoleConstants.Roles)
    {
        if (!roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
        {
            roleManager.CreateAsync(new IdentityRole(role)).GetAwaiter().GetResult();
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ABC API v1"));
}

app.UseHttpsRedirection();
app.UseIdentityServer();
app.UseAuthentication(); // << ต้องมาก่อน UseAuthorization
app.UseAuthorization();
app.MapControllers();
app.Run();
