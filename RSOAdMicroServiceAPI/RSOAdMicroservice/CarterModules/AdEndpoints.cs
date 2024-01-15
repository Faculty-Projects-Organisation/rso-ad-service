using Carter;
using Microsoft.AspNetCore.Http.HttpResults;
using RSO.Core.AdModels;
using RSO.Core.BL;
using RSO.Core.BL.LogicModels;

namespace RSOAdMicroservice.CarterModules;

public class AdEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapHealthChecks("/api/ad/health");

        var group = app.MapGroup("/api/ad/");

        group.MapGet("/all", GetAllAds).WithName(nameof(GetAllAds)).
            Produces(StatusCodes.Status200OK).WithTags("Ads");

        group.MapGet("{id}", GetAdById).WithName(nameof(GetAdById)).
            Produces(StatusCodes.Status200OK).
            Produces(StatusCodes.Status400BadRequest).
            Produces(StatusCodes.Status401Unauthorized).WithTags("Ads");

        group.MapGet("user/{id}", GetAdsByUserId).WithName(nameof(GetAdsByUserId)).
            Produces(StatusCodes.Status200OK).
            Produces(StatusCodes.Status400BadRequest).
            Produces(StatusCodes.Status401Unauthorized).WithTags("Ads");

        group.MapPost("/", CreateAd).WithName(nameof(CreateAd)).
            Produces(StatusCodes.Status201Created).
            Produces(StatusCodes.Status400BadRequest).
            Produces(StatusCodes.Status401Unauthorized).WithTags("Ads");
    }

    public static async Task<Results<Created<Ad>, BadRequest<string>>> CreateAd(IAdLogic adLogic, Ad newAd, Serilog.ILogger logger, HttpContext httpContext)
    {
        var requestId = httpContext?.TraceIdentifier ?? "Unknown";
        logger.Information("ad-service: Create method called. RequestID: {@requestId}", requestId);
        try
        {
            var ad = await adLogic.CreateAdAsync(newAd);
            if (ad is null)
            {
                return TypedResults.BadRequest("Couldn't create the ad.");
            }

            logger.Information("ad-service: Ad created: {@Ad}", ad);
            logger.Information("ad-service: Exiting method createAd");

            return TypedResults.Created("/", ad);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "ad-service: Error while creating ad: {@Ad}", newAd);
            return TypedResults.BadRequest(ex.Message);
        }
    }

    public static async Task<Results<Ok<List<Ad>>, BadRequest<string>>> GetAdsByUserId(IAdLogic adLogic, int id)
    {
        var ads = await adLogic.GetAdsByUserIdAsync(id);
        if (ads is null)
        {
            return TypedResults.BadRequest("Used doesn't have any ads.");
        }

        return TypedResults.Ok(ads);
    }

    public static async Task<Results<Ok<List<Ad>>, BadRequest<string>>> GetAllAds(IAdLogic adLogic, Serilog.ILogger logger, HttpContext httpContext)
    {
        var requestId = httpContext?.TraceIdentifier ?? "Unknown";
        logger.Information("ad-service: GetAllAds method called. RequestID: {@requestId}", requestId);

        var ads = await adLogic.GetAllAdsAsync();
        if (ads is null)
        {
            logger.Error("ad-service: Couldn't find any ads.");
            return TypedResults.BadRequest("Couldn't find any ads.");
        }

        logger.Information("ad-service: Exiting method GetAllAds");
        return TypedResults.Ok(ads);
    }

    public static async Task<Results<Ok<AdDetails>, BadRequest<string>>> GetAdById(IAdLogic adLogic, int id, Serilog.ILogger logger, HttpContext httpContext)
    {
        var requestId = httpContext?.TraceIdentifier ?? "Unknown";
        logger.Information("ad-service: GetAdById method called. RequestID: {@requestId}", requestId);

        var ad = await adLogic.GetAdByIdAsync(id);
        if (ad is null)
        {
            logger.Error("ad-service: Couldn't find any ads.");
            return TypedResults.BadRequest("Couldn't find any ads.");
        }

        if(!ad.Price.HasValue)
            ad.Price = 0;

        var withHufConversion = new AdDetails(ad,await adLogic.GetEurosConvertedIntoForintsAsync(ad.Price.Value));

        logger.Information("ad-service: Exiting method GetAdById");

        return TypedResults.Ok(withHufConversion);
    }
}
