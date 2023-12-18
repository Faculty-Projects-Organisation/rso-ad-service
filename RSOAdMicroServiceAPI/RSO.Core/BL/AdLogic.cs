using AdServiceRSO.Repository;
using RSO.Core.AdModels;
using RSO.Core.Configurations;

namespace RSO.Core.BL;

public class AdLogic : IAdLogic
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtSecurityTokenConfiguration _jwtConfiguration;

    /// <summary>
    /// Initializes the <see cref="UserLogic"/> class.
    /// </summary>
    /// <param name="unitOfWork"><see cref="IUnitOfWork"/> instance.</param>
    /// <param name="jwtConfiguration"><see cref="JwtSecurityTokenConfiguration"/> dependency injection.</param>
    public AdLogic(IUnitOfWork unitOfWork, JwtSecurityTokenConfiguration jwtConfiguration)
    {
        _unitOfWork = unitOfWork;
        _jwtConfiguration = jwtConfiguration;
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
}