using Microsoft.AspNetCore.Authorization;
using NetCoreWebApiDemo.Authorization.Requirement;

namespace NetCoreWebApiDemo.Authorization.Handler
{
    public class SameCompanyHandler : AuthorizationHandler<SameCompanyRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SameCompanyHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameCompanyRequirement requirement)
        {
            var companyId = context.User.FindFirst("companyId")?.Value;
            if (companyId == null)
                return Task.CompletedTask;
            var routeCompanyId = _httpContextAccessor.HttpContext?.Request.RouteValues["companyId"]?.ToString();
            if (routeCompanyId == null)
                return Task.CompletedTask;
            if (companyId == routeCompanyId)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
