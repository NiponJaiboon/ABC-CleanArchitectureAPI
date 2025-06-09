using API;
using Application.Services;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.AspNetCore.DataProtection;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/Information/log-.txt", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information, rollingInterval: RollingInterval.Day)
    .WriteTo.File("Logs/Error/error-.txt", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error, rollingInterval: RollingInterval.Day)
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
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<FirstDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("FirstDatabase")));
builder.Services.AddDbContext<SecondDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SecondDatabase")));

// Register unit of work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Register repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Register application services
builder.Services.AddScoped<ProductService>();

builder.Services.AddAutoMapper(typeof(Application.Mappings.MappingProfile).Assembly);

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ABC API", Version = "v1" });
});

// ใช้ Serilog กับ Host
builder.Host.UseSerilog();

// ถ้าผู้ใช้กรอกรหัสผิด 5 ครั้ง (ตาม MaxFailedAccessAttempts = 5)
// บัญชีจะถูกล็อก (login ไม่ได้) เป็นเวลา 5 นาที
// หลังจาก 5 นาที จะสามารถพยายามล็อกอินใหม่ได้
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
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

builder.Services.AddIdentityServer(options =>
{
    options.EmitStaticAudienceClaim = true;
    options.KeyManagement.Enabled = true; // <--- ปิด Automatic Key Management ที่นี่
})
.AddAspNetIdentity<ApplicationUser>() // ใช้ IdentityUser สำหรับการจัดการผู้ใช้
.AddInMemoryClients(Config.Clients)
.AddInMemoryIdentityResources(Config.IdentityResources)
.AddInMemoryApiScopes(Config.ApiScopes);
// .AddTestUsers(Config.TestUsers);

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ABC API v1"));
}

app.UseHttpsRedirection();
app.UseIdentityServer();
app.UseAuthorization();
app.MapControllers();
app.Run();