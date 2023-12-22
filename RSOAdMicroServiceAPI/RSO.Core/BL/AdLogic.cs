using AdServiceRSO.Repository;
using Newtonsoft.Json;
using RestSharp;
using RSO.Core.AdModels;
using RSO.Core.BL.LogicModels;
using System.Net;


namespace RSO.Core.BL;

public class AdLogic : IAdLogic
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApiCredentialsConfiguration _apicredentialsConfiguration;

    /// <summary>
    /// Initializes the <see cref="UserLogic"/> class.
    /// </summary>
    /// <param name="unitOfWork"><see cref="IUnitOfWork"/> instance.</param>
    /// <param name="apicredentialsConfiguration"></param>
    public AdLogic(IUnitOfWork unitOfWork, ApiCredentialsConfiguration apicredentialsConfiguration)
    {
        _unitOfWork = unitOfWork;
        _apicredentialsConfiguration = apicredentialsConfiguration;
    }

    public async Task<Ad> CreateAdAsync(Ad newAd)
    {
        try
        {
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