using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace API.Extensions
{
    public static class MiddlewareExtensions
    {
        public static WebApplication UseCustomMiddlewares(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ABC API v1"));
            }

            app.UseHttpsRedirection();
            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            return app;
        }
    }
}
