using Microsoft.EntityFrameworkCore;
using AdServiceRSO.Repository;
using RSO.Core.AdModels;

namespace RSO.Core.Repository;

public class AdRepository : GenericRepository<Ad>, IAdRepository
{
    public AdRepository(AdServicesRSOContext context) : base(context) { }

    // update the status of an ad
    //public async Task UpdateAdStatusAsync(Ad ad) => await _context.ads.Where(a => a.ID == ad.ID).ForEachAsync(a => a.status = ad.status);

    public async Task UpdateAdStatusAsync(Ad ad)
    {
        // Retrieve the ad from the database based on the ID
        var existingAd = await _context.ads.FirstOrDefaultAsync(a => a.ID == ad.ID);

        if (existingAd != null)
        {
            // Update the Status property
            existingAd.Status = ad.Status;

            // Save the changes to the database
            await _context.SaveChangesAsync();
        }
        else
        {
            // Handle the case where the entity with the specified ID is not found
            // You might throw an exception, log a message, or handle it in another way
            throw new InvalidOperationException($"Ad with ID {ad.ID} not found.");
        }
    }

}