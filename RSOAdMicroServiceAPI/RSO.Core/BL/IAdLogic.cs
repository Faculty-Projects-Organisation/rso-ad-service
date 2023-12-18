using RSO.Core.AdModels;
using System.Linq.Expressions;

namespace RSO.Core.BL;

public interface IAdLogic
{
    public Task<Ad> GetAdByIdAsync(int id);
    public Task<List<Ad>> GetAllAdsAsync();
    public Task<Ad> CreateAdAsync(Ad newAd);
}