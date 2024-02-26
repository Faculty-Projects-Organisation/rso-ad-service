using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RSO.Core.AdModels;
using Serilog;
using RSO.Core.Configurations;

namespace RSO.Core.Health
{
    public class UsersHealthCheck : IHealthCheck
    {
        private readonly UserServicesSettingsConfiguration _userServicesSettings;
        private readonly AdServicesRSOContext _context;
        int timeToHalfOpen = 10;
        int threshold = 3;
        int delay = 3;

        private static int errorCounter = 0;
        private static int circuitState = 0; // 0 - closed, 1 - open, 2 - half-open


        public UsersHealthCheck(AdServicesRSOContext context, UserServicesSettingsConfiguration userServicesSettings)
        {
            _context = context;
            _userServicesSettings = userServicesSettings;
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

                var userTask = PingUser();

                // Set a timeout
                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(delay));
                var completedTask = await Task.WhenAny(userTask, timeoutTask);

                if (completedTask == timeoutTask)
                {
                    if (circuitState == 2)
                    {
                        circuitState = 1;
                        logger.Information("ad-service: UserService HealthCheck failed with message: Circuit Opened");
                        _ = HalfOpenCircut(logger);
                        return HealthCheckResult.Unhealthy("Circuit Opened");
                    }

                    errorCounter++;
                    logger.Information("Error counter: {err}", errorCounter);
                    if (errorCounter < threshold)
                    {
                        logger.Information("ad-service: UserService HealthCheck failed {err} times.", errorCounter);
                        return HealthCheckResult.Unhealthy("UserService HealthCheck failed");
                    }
                    else
                    {
                        circuitState = 1;
                        logger.Information("ad-service: UserService HealthCheck failed with message: Circuit Opened");
                        _ = HalfOpenCircut(logger);
                        cancellationToken.ThrowIfCancellationRequested();
                        throw new TimeoutException("Circuit Opened");
                    }
                }
                else if (userTask.Result != "OK")
                {
                    if (circuitState == 2)
                    {
                        circuitState = 1;
                        logger.Information("ad-service: UserService HealthCheck failed with message: Circuit Opened");
                        _ = HalfOpenCircut(logger);
                        return HealthCheckResult.Unhealthy("Circuit Opened");
                    }

                    errorCounter++;
                    logger.Information("Error counter: {err}", errorCounter);
                    if (errorCounter < threshold)
                    {
                        logger.Information("ad-service: UserService HealthCheck failed  {err} times: {Message}", errorCounter, userTask.Result);
                        return HealthCheckResult.Unhealthy(userTask.Result);
                    } 
                    else
                    {
                        circuitState = 1;
                        logger.Information("ad-service: UserService HealthCheck failed with message: {Message}", userTask.Result);
                        logger.Information("ad-service: Circuit opened.");
                        _ = HalfOpenCircut(logger);
                        return HealthCheckResult.Unhealthy(userTask.Result);
                    }
                    
                }
                circuitState = 0;
                errorCounter = 0;
                logger.Information("ad-service: UserService HealthCheck succeeded");
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

        public async Task<string> PingUser()
        {
            try
            {
                var userService = _userServicesSettings.UserServiceEndpoint;
                // ping user service
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(userService);
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "OK";
        }
    }
}
