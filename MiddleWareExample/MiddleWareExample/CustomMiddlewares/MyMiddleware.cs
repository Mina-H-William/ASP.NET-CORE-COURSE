using System.Security.Cryptography.X509Certificates;

namespace MiddleWareExample.CustomMiddlewares
{
    public class MyMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context,RequestDelegate next)
        {
            await context.Response.WriteAsync(" Hello from MyMiddleWare\n");
            await next(context);
        }
    }

    public static class CustomMiddlewareExtension
    {
        public static IApplicationBuilder UseMyCustomMiddleWare(this IApplicationBuilder app)
        {
            return app.UseMiddleware<MyMiddleware>();
        }
    }
}
