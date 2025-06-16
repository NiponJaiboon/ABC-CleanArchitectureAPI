using API.Extensions;
using API.Middlewares;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        Path.Combine("Logs", "Information", "log-.txt"),
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
        rollingInterval: RollingInterval.Day
    )
    .WriteTo.File(
        Path.Combine("Logs", "Error", "error-.txt"),
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error,
        rollingInterval: RollingInterval.Day
    )
    .WriteTo.File(
        Path.Combine("Logs", "Warning", "warning-.txt"),
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning,
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

app.Use(
    async (context, next) =>
    {
        var token = context.Request.Cookies["access_token"];
        if (!string.IsNullOrEmpty(token) && !context.Request.Headers.ContainsKey("Authorization"))
        {
            context.Request.Headers.Append("Authorization", $"Bearer {token}");
        }
        await next();
    }
);

// Seed ข้อมูล IdentityServer และ Roles
app.Services.SeedIdentityServerAndRoles(); // SeedDataExtensions

// ใช้งาน middleware pipeline
app.UseCustomMiddlewares(); // MiddlewareExtensions

app.UseRouting();
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseIdentityServer();

// app.UseMiddleware<JwtFromCookieMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
