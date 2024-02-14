using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RSO.Core.AdModels;
using Serilog;
using System.Threading.Tasks;

namespace RSO.Core.Health
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly AdServicesRSOContext _context;
        int timeToHalfOpen = 10;
        int threshold = 3;

        private static int errorCounter = 0;
        private static int circuitState = 0; // 0 - closed, 1 - open, 2 - half-open


        public DatabaseHealthCheck(AdServicesRSOContext context)
        {
            _context = context;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
        {
            LoggerConfiguration loggerConfiguration = new();
            loggerConfiguration.WriteTo.Console();
            var logger = loggerConfiguration.CreateLogger();

            if (circuitState == 1)
            {
                return HealthCheckResult.Unhealthy("Circuit Open");
            }
            else
            {
                logger.Information("ad-service: Database HealthCheck started");

                var dbTask = ExecuteQuery(cancellationToken);

                // Set a timeout
                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(2));
                var completedTask = await Task.WhenAny(dbTask, timeoutTask);

                if (completedTask == timeoutTask)
                {
                    if (circuitState == 2)
                    {
                        circuitState = 1;
                        logger.Information("ad-service: Database HealthCheck failed with message: Circuit Opened");
                        _ = HalfOpenCircut(logger);
                        return HealthCheckResult.Unhealthy("Circuit Opened");
                    }

                    errorCounter++;
                    logger.Information("Error counter: {err}", errorCounter);
                    if (errorCounter < threshold)
                    {
                        logger.Information("ad-service: Database HealthCheck failed {err} times.", errorCounter);
                        return HealthCheckResult.Unhealthy("Database HealthCheck failed");
                    }
                    else
                    {
                        circuitState = 1;
                        logger.Information("ad-service: Database HealthCheck failed with message: Circuit Opened");
                        _ = HalfOpenCircut(logger);
                        cancellationToken.ThrowIfCancellationRequested();
                        throw new TimeoutException("Circuit Opened");
                    }
                }
                else if (dbTask.Result != "OK")
                {
                    if (circuitState == 2)
                    {
                        circuitState = 1;
                        logger.Information("ad-service: Database HealthCheck failed with message: Circuit Opened");
                        _ = HalfOpenCircut(logger);
                        return HealthCheckResult.Unhealthy("Circuit Opened");
                    }

                    errorCounter++;
                    logger.Information("Error counter: {err}", errorCounter);
                    if (errorCounter < threshold)
                    {
                        logger.Information("ad-service: Database HealthCheck failed  {err} times: {Message}", errorCounter, dbTask.Result);
                        return HealthCheckResult.Unhealthy(dbTask.Result);
                    } 
                    else
                    {
                        circuitState = 1;
                        logger.Information("ad-service: Database HealthCheck failed with message: {Message}", dbTask.Result);
                        logger.Information("ad-service: Circuit opened.");
                        _ = HalfOpenCircut(logger);
                        return HealthCheckResult.Unhealthy(dbTask.Result);
                    }
                    
                }
                circuitState = 0;
                errorCounter = 0;
                logger.Information("ad-service: Database HealthCheck succeeded");
                logger.Information("ad-service: Circuit back to closed state.");
                return HealthCheckResult.Healthy();
            }
        }

        public async Task<string> HalfOpenCircut(Serilog.ILogger logger)
        {
            await Task.Delay(TimeSpan.FromSeconds(timeToHalfOpen));
            circuitState = 2;
            logger.Information("ad-service: Circuit half opened.");
            return "ad-service: Circuit half opened.";
        }

        public async Task<string> ExecuteQuery(CancellationToken cancellationToken)
        {
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
