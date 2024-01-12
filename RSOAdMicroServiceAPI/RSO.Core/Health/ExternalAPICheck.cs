using Microsoft.Extensions.Diagnostics.HealthChecks;
using RestSharp;
using RSO.Core.BL;
using System.Net;
using Serilog;

namespace RSO.Core.Health
{
    public class ExternalAPICheck : IHealthCheck
    {
        private readonly ApiCredentialsConfiguration _apicredentialsConfiguration;

        public ExternalAPICheck(ApiCredentialsConfiguration apiCredentialsConfiguration)
        {
            _apicredentialsConfiguration = apiCredentialsConfiguration;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
        {
            LoggerConfiguration loggerConfiguration = new();
            loggerConfiguration.WriteTo.Console();
            var logger = loggerConfiguration.CreateLogger();

            logger.Information("ad-service: ExternalAPI HealthCheck started");

            var testTask = TestApi(cancellationToken);
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(2));

            var completedTask = await Task.WhenAny(testTask, timeoutTask);

            if (completedTask == timeoutTask)
            {
                // The operation timed out
                cancellationToken.ThrowIfCancellationRequested();
                throw new TimeoutException("Operation timed out");
            }           
            else if (testTask.Result != null && HttpStatusCode.InternalServerError.CompareTo(testTask.Result.StatusCode) > 0) // response code is smaller than 500
            {
                logger.Information("ad-service: ExternalAPI HealthCheck succeeded");
                return HealthCheckResult.Healthy();
            }
            else
            {
                logger.Information("ad-service: ExternalAPI HealthCheck failed with message: {Message}", testTask.Result.ErrorMessage);
                return HealthCheckResult.Unhealthy();
            }
        }

        private async Task<RestResponse> TestApi(CancellationToken cancellationToken)
        {
            //await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            try
            {
                var client = new RestClient($"https://currency-converter5.p.rapidapi.com/currency/convert?format=json&from=EUR&to=HUF&amount=100");
                var request = new RestRequest();
                request.AddHeader("X-RapidAPI-Key", _apicredentialsConfiguration.Token);
                request.AddHeader("X-RapidAPI-Host", "currency-converter5.p.rapidapi.com");
                var response = client.ExecuteAsync(request);
                response.Wait();
                var restResponse = await response;
                return restResponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
