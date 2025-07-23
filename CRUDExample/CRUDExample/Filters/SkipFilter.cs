using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters
{
    // implement Attribute to make this class accessed in controller like attributes
    // implement IFilterMetadata to make this class act like filter so can be tracked by context.filters when applied on actions
    public class SkipFilter : Attribute, IFilterMetadata
    {
    }
}
