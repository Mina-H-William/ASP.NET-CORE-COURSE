namespace AssisgnmentMiddleware.CustomMiddleware
{
    public class CheckPathMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Method == "POST" && context.Request.Path == "/")
            {
                await next(context);
            }
        }
    }

    public static class CheckPathMiddlewareExtension 
    {
        public static IApplicationBuilder UseCheckPathMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CheckPathMiddleware>();
        }
    }

}
