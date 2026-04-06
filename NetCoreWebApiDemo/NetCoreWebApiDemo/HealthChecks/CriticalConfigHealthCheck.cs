using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NetCoreWebApiDemo.HealthChecks
{
    public class CriticalConfigHealthCheck : IHealthCheck
    {
        private readonly IConfiguration _configuration;

        public CriticalConfigHealthCheck(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var apiKey = _configuration["ExternalService:ApiKey"];
            if(string.IsNullOrWhiteSpace(apiKey))
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("External ApiKey missing."));
            }
            return Task.FromResult(HealthCheckResult.Healthy("Config OK"));
        }
    }
}
