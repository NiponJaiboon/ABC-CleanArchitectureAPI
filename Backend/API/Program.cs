using API.Extensions;
using API.Middlewares;
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

// Register services
builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration); // ServiceCollectionExtensions
builder.Services.AddSwaggerDocumentation(); // SwaggerServiceExtensions
builder.Services.AddIdentityServerConfiguration(builder.Configuration); // IdentityServerExtensions

// Configure Serilog
builder.Host.UseSerilog();

// Build the app
var app = builder.Build();

// Seed ข้อมูล IdentityServer และ Roles
app.Services.SeedIdentityServerAndRoles(); // SeedDataExtensions

// ใช้งาน middleware pipeline
app.UseCustomMiddlewares(); // MiddlewareExtensions

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseIdentityServer();
app.UseMiddleware<JwtFromCookieMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
