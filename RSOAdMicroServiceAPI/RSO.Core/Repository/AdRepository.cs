using AdServiceRSO.Repository;
using RSO.Core.AdModels;

namespace RSO.Core.Repository;

public class AdRepository : GenericRepository<Ad>, IAdRepository
{
    public AdRepository(AdServicesRSOContext context) : base(context)
    {
        
    }
}