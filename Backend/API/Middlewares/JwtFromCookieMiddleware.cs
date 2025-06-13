using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace API.Middlewares
{
    public class JwtFromCookieMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtFromCookieMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Cookies["access_token"];
            if (
                !string.IsNullOrEmpty(token)
                && !context.Request.Headers.ContainsKey("Authorization")
            )
            {
                Console.WriteLine($"JWT token added to Authorization header from cookie.{token}");
                context.Request.Headers.Append("Authorization", $"Bearer {token}");
            }
            await _next(context);
        }
    }
}
