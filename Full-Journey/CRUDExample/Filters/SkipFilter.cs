using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters
{
    public class SkipFilter:Attribute, IFilterMetadata //this class can access as a filter
    {

    }
}
