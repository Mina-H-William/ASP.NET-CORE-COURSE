using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ResultFilters
{
    public class TokenResultFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            context.HttpContext.Response.Cookies.Append("Auth-Key", "A100",
                new CookieOptions
                {
                    HttpOnly = true, // to prevent client-side scripts from accessing the cookie
                    //Secure = true, // Set to true if using HTTPS
                    //SameSite = SameSiteMode.Strict, // Adjust as needed
                    //Expires = DateTimeOffset.UtcNow.AddDays(1) // Set expiration as needed
                }
            );

            await next();
        }
    }
}
