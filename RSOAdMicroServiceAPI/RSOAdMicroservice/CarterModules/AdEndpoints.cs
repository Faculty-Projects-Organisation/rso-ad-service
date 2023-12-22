using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RSO.Core.AdModels;
using RSO.Core.BL;
using System.Threading;

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

        group.MapPost("/", CreateAd).WithName(nameof(CreateAd)).
            Produces(StatusCodes.Status201Created).
            Produces(StatusCodes.Status400BadRequest).
            Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/healths", HealthCheck).WithName(nameof(HealthCheck)).
            Produces(StatusCodes.Status200OK);
    }

    public static async Task<Results<Created<Ad>, BadRequest<string>>> CreateAd(IAdLogic adLogic, Ad newAd)
    {
        try
        {
            var ad = await adLogic.CreateAdAsync(newAd);
            if (ad is null)
            {
                return TypedResults.BadRequest("Couldn't create the ad.");
            }
            return TypedResults.Created("/", ad);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    public static async Task<Results<Ok<List<Ad>>, BadRequest<string>>> GetAllAds(IAdLogic adLogic)
    {
        var ads = await adLogic.GetAllAdsAsync();
        if (ads is null)
        {
            return TypedResults.BadRequest("Couldn't find any ads.");
        }

        return TypedResults.Ok(ads);
    }

    public static async Task<Results<Ok<AdDetails>, BadRequest<string>>> GetAdById(IAdLogic adLogic, int id)
    {
        var ad = await adLogic.GetAdByIdAsync(id);
        if (ad is null)
        {
            return TypedResults.BadRequest("Couldn't find any ads.");
        }

        if(!ad.Price.HasValue)
            ad.Price = 0;

        var withHufConversion = new AdDetails(ad,await adLogic.GetEurosConvertedIntoForintsAsync(ad.Price.Value));

        return TypedResults.Ok(withHufConversion);
    }

    public static async Task<Results<Ok<string>, BadRequest<string>>> HealthCheck(IAdLogic adLogic)
    {
        try
        {
            var client = new HttpClient();
            var response = await client.GetAsync("https://api.lavbic.net/kraji/1000");
            if (response.IsSuccessStatusCode)
            {
                // return string healthy
                return TypedResults.Ok("Healthy");
            }
            else
            {
                return TypedResults.Ok("Unhealthy");
            }
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}
