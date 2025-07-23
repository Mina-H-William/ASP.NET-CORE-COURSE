using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.AuthorizationFilters
{
    public class TokenAuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Cookies.ContainsKey("Auth-Key") || context.HttpContext.Request.Cookies["Auth-Key"] != "A100")
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        }
    }
}
