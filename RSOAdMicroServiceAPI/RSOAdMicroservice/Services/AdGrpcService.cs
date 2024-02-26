using Grpc.Core;
using RSO.Core.BL;

namespace RSOAdMicroservice.Services;

public class GAdsService : AdProto.AdProtoBase
{
    private readonly IAdLogic _adLogic;

    /// <summary>
    /// Ad grpc service constructor.
    /// </summary>
    /// <param name="adLogic"><see cref="IAdLogic"/> instance.</param>
    public GAdsService(IAdLogic adLogic) => _adLogic = adLogic;

    /// <summary>
    /// Get all the ads specified by the user id.
    /// </summary>
    /// <param name="request">The request object. See <see cref="AdByIdUserIdRequest"/> object.</param>
    /// <param name="ctx"><see cref="ServerCallContext"/></param>
    /// <returns><see cref="AdsByUserIdReply"/> with all of the user's add ads as an <see cref="AdItem"/> list.</returns>
    public override Task<AdsByUserIdReply> GetAdsByUserId(AdByIdUserIdRequest request, ServerCallContext ctx)
    {
        var reply = new AdsByUserIdReply();
        var ads = _adLogic.GetAdsByUserIdAsync(request.UserId).Result;

        var adItems = ads.Any() ? ads.ConvertAll(ad => new AdItem
        {
            Id = ad.ID,
            Thing = ad.Thing,
            Price = ad.Price ?? 0,
            Category = ad.Category,
            Status = ad.Status,
            PublishDate = ad.PostTime.ToString()
        }) : new List<AdItem>();

        reply.Ads.AddRange(adItems);
        return Task.FromResult(reply);
    }
}
