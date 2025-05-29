using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Core.Interfaces;
using Infrastructure.Repositories;
using Application.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.File("Logs/Error/error-.txt", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error, rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure database contexts
builder.Services.AddDbContext<FirstDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FirstDatabase")));
builder.Services.AddDbContext<SecondDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SecondDatabase")));

// Register unit of work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

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

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ABC API v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();