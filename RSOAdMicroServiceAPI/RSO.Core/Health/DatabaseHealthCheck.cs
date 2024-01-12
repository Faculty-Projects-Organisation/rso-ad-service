using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RSO.Core.AdModels;

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
            var dbTask = ExecuteQuery(cancellationToken);

            // Set a timeout
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(2));

            var completedTask = await Task.WhenAny(dbTask, timeoutTask);

            if (completedTask == timeoutTask)
            {
                // The operation timed out
                cancellationToken.ThrowIfCancellationRequested();
                throw new TimeoutException("Operation timed out");
            }
            else if (dbTask.Result != "OK")
            {
                return HealthCheckResult.Unhealthy(dbTask.Result);
            }
            return HealthCheckResult.Healthy();
        }

        public async Task<string> ExecuteQuery(CancellationToken cancellationToken)
        {
            //await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

            try
            {
                // connect to database and execute "select 1" query
                await _context.Database.OpenConnectionAsync(cancellationToken);
                await _context.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "OK";
        }
    }
}
