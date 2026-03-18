using Microsoft.AspNetCore.Mvc.Filters;

namespace NetCoreWebApiDemo.Filters
{
    public class ResourceLogFilter : IResourceFilter
    {
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            Console.WriteLine("Resource Filter: Request started.");
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            Console.WriteLine("Resource Filter: Response send.");
        }
    }
}
