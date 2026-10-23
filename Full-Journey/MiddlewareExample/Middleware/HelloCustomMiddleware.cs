
namespace MiddlewareExample.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class HelloCustomMiddleware
    {
        private readonly RequestDelegate _next;

        public HelloCustomMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            await httpContext.Response.WriteAsync("FROM MIDDLEWARE [2] STARTS\n");
            if(httpContext.Request.Query.ContainsKey("firstName")
                && httpContext.Request.Query.ContainsKey("lastName"))
            {
                string fullName = httpContext.Request.Query["firstName"] + " "
                     + httpContext.Request.Query["lastName"];
                await httpContext.Response.WriteAsync($"Hello {fullName}\n");
            }
            await _next(httpContext);

            await httpContext.Response.WriteAsync("FROM MIDDLEWARE [2] ENDS\n");
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class HelloCustomMiddlewarecsExtensions
    {
        public static IApplicationBuilder UseHelloCustomMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HelloCustomMiddleware>();
        }
    }
}
