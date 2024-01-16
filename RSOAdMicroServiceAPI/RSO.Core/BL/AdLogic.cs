using AdServiceRSO.Repository;
using Newtonsoft.Json;
using RestSharp;
using RSO.Core.AdModels;
using RSO.Core.BL.LogicModels;
using RSO.Core.Configurations;
using System.Linq.Expressions;
using System.Net;
using System.Text;

namespace RSO.Core.BL;

public class AdLogic : IAdLogic
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApiCredentialsConfiguration _apicredentialsConfiguration;
    private readonly CrossEndpointsFunctionalityConfiguration _crossEndpointsFunctionalityConfiguration;

    /// <summary>
    /// Initializes the <see cref="UserLogic"/> class.
    /// </summary>
    /// <param name="unitOfWork"><see cref="IUnitOfWork"/> instance.</param>
    /// <param name="apicredentialsConfiguration"></param>
    /// <param name="crossEndpointsFunctionalityConfiguration"><see cref="CrossEndpointsFunctionalityConfiguration"/> DI.</param>
    public AdLogic(IUnitOfWork unitOfWork, ApiCredentialsConfiguration apicredentialsConfiguration, CrossEndpointsFunctionalityConfiguration crossEndpointsFunctionalityConfiguration)
    {
        _unitOfWork = unitOfWork;
        _apicredentialsConfiguration = apicredentialsConfiguration;
        _crossEndpointsFunctionalityConfiguration = crossEndpointsFunctionalityConfiguration;
    }
    
    public async Task<TransactionDTO> CreateTransactionAsync(TransactionDTO transactionDto)
    {
        var jsonPayload = JsonConvert.SerializeObject(transactionDto);

        // Set the URL of the Transactions microservice endpoint
        var apiUrl = _crossEndpointsFunctionalityConfiguration.CreateTransactionEndpoint.ToString();

        // Create an instance of HttpClient
        using (var httpClient = new HttpClient())
        {
            // Set the content type to application/json
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            // Make a POST request to the Transactions microservice
            var response = await httpClient.PostAsync(apiUrl, content);

            // Check the response status
            if (response.IsSuccessStatusCode)
            {
                // Transaction created successfully
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Transaction created successfully: {result}");
                return transactionDto;
            }
            else
            {
                // Error in creating the transaction
                var errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error creating transaction: {errorMessage}");
                // not ok
                return transactionDto;
            }
        }
    }

    public async Task<bool> UpdateAdAsync(Ad ad)
    {
        try
        {
            await _unitOfWork.AdRepository.UpdateAdStatusAsync(ad);
            //await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return false;
        }
        return true;
    }

    public async Task<Ad> CreateAdAsync(Ad newAd)
    {
        try
        {
            //NO TIME FOR PROPER FIXES
            var existingAds = await _unitOfWork.AdRepository.GetAllAsync();
            if (existingAds.Count == 0)
            {
                newAd.ID = 1;
            }
            else
            {
                newAd.ID = existingAds.Max(ea => ea.ID) + 1;
            }
            
            var ad = await _unitOfWork.AdRepository.InsertAsync(newAd);
            await _unitOfWork.SaveChangesAsync();
            return ad;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<Ad> GetAdByIdAsync(int id)
    {
        try
        {
            return await _unitOfWork.AdRepository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<List<Ad>> GetAdsByUserIdAsync(int userId)
    {
        try
        {
            Expression<Func<Ad, bool>> filter = ad => ad.UserId == userId;

            return await _unitOfWork.AdRepository.GetManyAsync(filter);
        }
        catch (Exception ex)
        {
            return null;
        }
    }


    public async Task<List<Ad>> GetAllAdsAsync()
    {
        try
        {
            return await _unitOfWork.AdRepository.GetAllAsync();
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<string> GetEurosConvertedIntoForintsAsync(int price, string sourceCurrency = "EUR", string targetCurrency = "HUF")
    {
        if (price <= 0)
            return "0";

        var client = new RestClient($"https://currency-converter5.p.rapidapi.com/currency/convert?format=json&from={sourceCurrency}&to={targetCurrency}&amount={price}");
        var request = new RestRequest();
        request.AddHeader("X-RapidAPI-Key", _apicredentialsConfiguration.Token);
        request.AddHeader("X-RapidAPI-Host", "currency-converter5.p.rapidapi.com");
        var response = client.ExecuteAsync(request);
        response.Wait();
        var restResponse = await response;

        if (!restResponse.StatusCode.Equals(HttpStatusCode.OK))
            return null;

        var result = JsonConvert.DeserializeObject<Conversion>(restResponse.Content);

        return result.Rates[targetCurrency].RateForAmount;
    }
}