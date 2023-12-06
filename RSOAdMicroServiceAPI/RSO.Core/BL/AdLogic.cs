using AdServiceRSO.Repository;
using LazyCache;
using RSO.Core.AdModels;
using RSO.Core.Configurations;

namespace RSO.Core.BL;

public class AdLogic : IAdLogic
{
    private readonly IAppCache _appcache;
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtSecurityTokenConfiguration _jwtConfiguration;

    /// <summary>
    /// Initializes the <see cref="UserLogic"/> class.
    /// </summary>
    /// <param name="appcache"><see cref="IAppCache"/> instance.</param>
    /// <param name="unitOfWork"><see cref="IUnitOfWork"/> instance.</param>
    /// <param name="jwtConfiguration"><see cref="JwtSecurityTokenConfiguration"/> dependency injection.</param>
    public AdLogic(IAppCache appcache, IUnitOfWork unitOfWork, JwtSecurityTokenConfiguration jwtConfiguration)
    {
        _appcache = appcache;
        _unitOfWork = unitOfWork;
        _jwtConfiguration = jwtConfiguration;
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