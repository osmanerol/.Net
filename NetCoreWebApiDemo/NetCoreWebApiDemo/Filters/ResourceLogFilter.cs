using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace NetCoreWebApiDemo.Filters
{
    public class ActionLogFilter : IActionFilter
    {
        private Stopwatch _stopwatch;

        public ActionLogFilter()
        {
            _stopwatch = new Stopwatch();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _stopwatch = Stopwatch.StartNew();
            Console.WriteLine($"Action started: {context.ActionDescriptor.DisplayName}");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _stopwatch.Stop();
            Console.WriteLine($"Action ended: {_stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
