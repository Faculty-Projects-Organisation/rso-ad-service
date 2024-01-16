using Carter;
using Microsoft.AspNetCore.Http.HttpResults;
using RSO.Core.AdModels;
using RSO.Core.BL;
using RSO.Core.BL.LogicModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace RSOAdMicroservice.CarterModules;

public class AdEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapHealthChecks("/ads/api/health");

        var group = app.MapGroup("/ads/api/");

        group.MapGet("/", GetAllAds).WithName(nameof(GetAllAds)).
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

        group.MapPatch("initiateTransaction", FinalizeAd).WithName(nameof(FinalizeAd)).
            Produces(StatusCodes.Status204NoContent).
            Produces(StatusCodes.Status400BadRequest).
            Produces(StatusCodes.Status401Unauthorized).
            WithTags("Ads").
            WithDescription("Updates the ad and creates a transaction");
    }

    public static async Task<Results<Created<TransactionDTO>, Ok<string>, BadRequest<string>>> FinalizeAd([FromBody] AdData adData, IAdLogic adLogic, Serilog.ILogger logger, HttpContext httpContext)
    {
        var requestId = httpContext?.TraceIdentifier ?? "Unknown";
        logger.Information("ad-service: FinalizeAd method called. RequestID: {@requestId}", requestId);
        try
        {
            var newTransactionDto = new TransactionDTO
            {
                SellerId = adData.UserId,
                BuyerId = 4,
                AdId = adData.ID,
                PriceActual = adData.Price,
                DateTime = DateTime.UtcNow
            };

            var transaction = await adLogic.CreateTransactionAsync(newTransactionDto);
            logger.Information("ad-service: Transaction created: {@Transaction}", transaction);

            var ad = await adLogic.GetAdByIdAsync(adData.ID);
            ad.Status = "sold";

            if (await adLogic.UpdateAdAsync(ad))
            {
                return TypedResults.Ok(transaction.AdId.ToString());
                //return TypedResults.Ok("suces");
            }
            logger.Information("ad-service: Exiting method FinalizeAd, ad updated: {@Ad}", ad);

            logger.Information("ad-service: Error updating the ad");
            return TypedResults.BadRequest("Failed to update the ad.");
        }
        catch (Exception ex)
        {
            logger.Error(ex, "ad-service: Error while creating ad: {@msg}", ex.Message);
            return TypedResults.BadRequest(ex.Message);
        }
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

    public static async Task<Results<Ok<List<Ad>>, BadRequest<string>>> GetAdsByUserId(IAdLogic adLogic, int id, Serilog.ILogger logger, HttpContext httpContext)
    {
        var requestId = httpContext?.TraceIdentifier ?? "Unknown";
        logger.Information("ad-service: GetAdsByUserId method called. RequestID: {@requestId}", requestId);

        var ads = await adLogic.GetAdsByUserIdAsync(id);
        if (ads is null)
        {
            logger.Error("ad-service: Used doesn't have any ads.");
            return TypedResults.BadRequest("Used doesn't have any ads.");
        }

        logger.Information("ad-service: Exiting method GetAdsByUserId");
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

        var withHufConversion = new AdDetails(ad, await adLogic.GetEurosConvertedIntoForintsAsync(ad.Price.Value));

        logger.Information("ad-service: Exiting method GetAdById");

        return TypedResults.Ok(withHufConversion);
    }
}
