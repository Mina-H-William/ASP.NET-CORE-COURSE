using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ResourceFilters
{
    public class FeatureDisableResourceFilter : IAsyncResourceFilter
    {
        private readonly ILogger<FeatureDisableResourceFilter> _logger;
        private readonly bool _isDisabled;

        public FeatureDisableResourceFilter(ILogger<FeatureDisableResourceFilter> logger, bool isDisabled = true)
        {
            _logger = logger;
            _isDisabled = isDisabled;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            // to do before
            _logger.LogInformation("{FilterName}.{MethodName} - Before", nameof(FeatureDisableResourceFilter),
                                    nameof(OnResourceExecutionAsync));

            if (_isDisabled)
            {
                //context.Result = new NotFoundResult(); // 404 - Not Found

                context.Result = new StatusCodeResult(501); // 404 - Not Found
                return;
            }

            await next();

            // to do after
            _logger.LogInformation("{FilterName}.{MethodName} - After", nameof(FeatureDisableResourceFilter),
                                    nameof(OnResourceExecutionAsync));
        }
    }
}
