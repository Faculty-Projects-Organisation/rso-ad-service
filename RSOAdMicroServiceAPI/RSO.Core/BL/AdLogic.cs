using RSO.Core.AdModels;
using RSO.Core.Repository;


namespace RSO.Core.BL;

public class AdLogic : IAdLogic
{
    private readonly IAdRepository _adRepository;

    //public AdLogic(IAdRepository adRepository)
    //{
    //    _adRepository = adRepository;
    //}

    public async Task<Ad> GetAdByIdAsync(int id)
    {
        return await _adRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Ad>> GetAllAdsAsync()
    {
        return await _adRepository.GetAllAsync();
    }


}