
namespace MinimalAPI.EndpointFilters
{
    public class CustomEndpointFilter : IEndpointFilter
    {
        private readonly ILogger<CustomEndpointFilter> _logger;

        public CustomEndpointFilter(ILogger<CustomEndpointFilter> logger)
        {
            _logger = logger;
        }

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            // before logic
            _logger.LogInformation("CustomEndpointFilter: Before processing the request.");

            var result = await next(context);

            // after logic
            _logger.LogInformation("CustomEndpointFilter: After processing the request.");

            return result;
        }
    }
}
