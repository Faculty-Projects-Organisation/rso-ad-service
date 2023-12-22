using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RSO.Core.Health
{
    public class ExternalAPICheck : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
        {
            try
            {
                var client = new HttpClient();
                var response = await client.GetAsync("https://api.lavbic.net/kraji/1000");
                if (response.IsSuccessStatusCode)
                {
                    return HealthCheckResult.Healthy();
                }
                else
                {
                    return HealthCheckResult.Unhealthy();
                }
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(ex.Message);
            }
        }
    }
}
