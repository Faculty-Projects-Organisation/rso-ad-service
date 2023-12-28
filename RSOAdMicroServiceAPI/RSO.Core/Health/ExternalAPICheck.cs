using Microsoft.Extensions.Diagnostics.HealthChecks;
using RestSharp;
using RSO.Core.BL;
using System.Net;

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
            try
            {
                var client = new RestClient($"https://currency-converter5.p.rapidapi.com/currency/convert?format=json&from=EUR&to=HUF&amount=100");
                var request = new RestRequest();
                request.AddHeader("X-RapidAPI-Key", _apicredentialsConfiguration.Token);
                request.AddHeader("X-RapidAPI-Host", "currency-converter5.p.rapidapi.com");
                var response = client.ExecuteAsync(request);
                response.Wait();
                var restResponse = await response;

                // is smaller than 500
                if (restResponse.StatusCode.CompareTo(HttpStatusCode.InternalServerError) < 0)
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
