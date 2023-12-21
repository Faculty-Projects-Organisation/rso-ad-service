using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RSO.Core.AdModels;
//using RSO.Core.BL;

namespace RSO.Core.Health
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly AdServicesRSOContext _context;

        public DatabaseHealthCheck(AdServicesRSOContext context)
        {
            _context = context;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
        {
            try
            {
                // connect to database and execute "select 1" query
                await _context.Database.OpenConnectionAsync(cancellationToken);
                await _context.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);

                //var ad = await adLogic.GetAdByIdAsync(1);
                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(ex.Message);
            }

        }
    }
}
