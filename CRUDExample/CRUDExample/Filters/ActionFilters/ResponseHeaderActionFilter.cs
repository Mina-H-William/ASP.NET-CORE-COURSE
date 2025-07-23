using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ActionFilters
{
    public class ResponseHeaderFilterFactoryAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => false;

        private string? _key { get; set; }
        private string? _value { get; set; }
        private int _order { get; set; }

        public ResponseHeaderFilterFactoryAttribute(string key, string value, int order)
        {
            _key = key;
            _value = value;
            _order = order;
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            // return filter object
            //var filter = new ResponseHeaderActionFilter();
            var filter = serviceProvider.GetRequiredService<ResponseHeaderActionFilter>();

            filter.key = _key;
            filter.value = _value;
            filter.Order = _order;

            return filter;
        }
    }

    // in action filter Attribute can inject services from DI 
    public class ResponseHeaderActionFilter : IAsyncActionFilter, IOrderedFilter // work for both action filter and result filter
    {
        private readonly ILogger<ResponseHeaderActionFilter> _logger;

        public string? key { get; set; }
        public string? value { get; set; }

        public int Order { get; set; } // no need for Order as it exist in action filter attribute

        public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("{FilterName}: {MethodName} called - Before",
                                   nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));

            if(string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
            {
                _logger.LogWarning("{FilterName}: {MethodName} called - Key or Value is null or empty",
                                   nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));
                return;
            }
            context.HttpContext.Response.Headers[key] = value;

            await next();

            _logger.LogInformation("{FilterName}: {MethodName} called - After",
                                   nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));

        }
    }
}
