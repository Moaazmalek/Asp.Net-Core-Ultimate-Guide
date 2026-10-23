using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ActionFilters
{
    public class ResponseHeaderFilterFactoryAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => false; // in case of true : the filter object instance can be accessible from multiple requests in the application.
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
            ResponseHeaderActionFilter filter= new ResponseHeaderActionFilter(_key,_value,_order);
            //return filter object
            return filter; 
        }
    }
    //Controller -> FilterFactory -> Filter
    public class ResponseHeaderActionFilter : IAsyncActionFilter,IOrderedFilter
    {

        private readonly string _key;
        private readonly string _value;
        public int Order { get; }

        public ResponseHeaderActionFilter(string key,string value,int order)
        {
            _key = key;
            _value = value;
            Order = order;
        }
        //before
        //public void OnActionExecuting(ActionExecutingContext context)
        //{
        //    _logger.LogInformation("{FilterName}.{MethodName} method", nameof(ResponseHeaderActionFilter), nameof(OnActionExecuting));

        //}
        //after
        //public void OnActionExecuted(ActionExecutedContext context)
        //{
        //    _logger.LogInformation("{FilterName}.{MethodName} method", nameof(ResponseHeaderActionFilter), nameof(OnActionExecuted));
        //    context.HttpContext.Response.Headers[_key] = _value;
        //}

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            await next(); // calls the subsequent filter or action method

            context.HttpContext.Response.Headers[_key] = _value;
        }
    }
}
