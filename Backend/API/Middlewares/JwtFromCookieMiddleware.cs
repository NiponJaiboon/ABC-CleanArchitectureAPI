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
        private readonly ILogger<JwtFromCookieMiddleware> _logger;

        public JwtFromCookieMiddleware(
            RequestDelegate next,
            ILogger<JwtFromCookieMiddleware> logger
        )
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Cookies["access_token"];
            if (
                !string.IsNullOrEmpty(token)
                && !context.Request.Headers.ContainsKey("Authorization")
            )
            {
                context.Request.Headers.Append("Authorization", $"Bearer {token}");
                _logger.LogInformation(
                    "JWT token added to Authorization header from cookie.{Token}",
                    token
                );
            }
            await _next(context);
        }
    }
}
