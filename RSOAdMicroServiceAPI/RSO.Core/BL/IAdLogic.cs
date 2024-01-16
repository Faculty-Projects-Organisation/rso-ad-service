using RSO.Core.AdModels;

namespace RSO.Core.BL;

public interface IAdLogic
{
    public Task<Ad> GetAdByIdAsync(int id);
    public Task<List<Ad>> GetAllAdsAsync();
    public Task<Ad> CreateAdAsync(Ad newAd);
    public Task<TransactionDTO> CreateTransactionAsync(TransactionDTO transactionDto);
    public Task<bool> UpdateAdAsync(Ad ad);
    public Task<string> GetEurosConvertedIntoForintsAsync(int price, string sourceCurrency = "EUR", string targetCurrency = "HUF");
    public Task<List<Ad>> GetAdsByUserIdAsync(int userId);
}