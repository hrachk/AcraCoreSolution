using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace AcraValidatorWebService.Middlewares
{
    public class GetInfoBySNNMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Token _bearer;
        public GetInfoBySNNMiddleware(RequestDelegate requestDelegate, IOptions<Token> options)
        {
            _next = requestDelegate;
            _bearer = options.Value;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Path.StartsWithSegments("/Validator/GetInfoBySSN", StringComparison.OrdinalIgnoreCase))
            {
                var authHeader = httpContext.Request.Headers["Authorization"].ToString();

                if (string.IsNullOrWhiteSpace(authHeader) ||
                    !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await httpContext.Response.WriteAsync("Unauthorized");
                    return;
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();

                if (token != _bearer.Bearer)
                {
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await httpContext.Response.WriteAsync("Unauthorized");
                    return;
                }

            }
            await _next(httpContext);
        }
    }

    public static class GetInfoBySNNMiddlewareExtensions
    {
        public static IApplicationBuilder UseGetInfoBySNNMiddleware(this IApplicationBuilder build)
        {
            return build.UseMiddleware<GetInfoBySNNMiddleware>();
        }
    }

    public class Token
    {
        public string Bearer { get; set; }
    }
}
