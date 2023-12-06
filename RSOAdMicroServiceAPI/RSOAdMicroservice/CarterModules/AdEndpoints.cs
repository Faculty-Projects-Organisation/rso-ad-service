using Carter;
using Microsoft.AspNetCore.Http.HttpResults;
using RSO.Core.AdModels;
using RSO.Core.BL;

namespace RSOAdMicroservice.CarterModules;

public class AdEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        //app.MapGet("/", () => "Hello from Carter!");

        var group = app.MapGroup("/api/ad/");

        group.MapGet("/all", GetAllAds).WithName(nameof(GetAllAds)).
            Produces(StatusCodes.Status200OK);

        //group.MapGet("{id}", GetAdById).WithName(nameof(GetAdById)).
        //    Produces(StatusCodes.Status200OK).
        //    Produces(StatusCodes.Status400BadRequest).
        //    Produces(StatusCodes.Status401Unauthorized);
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

    //private static async Task GetAllAds(HttpContext context)
    //{
    //    var adRepository = context.RequestServices.GetService<IAdRepository>();
    //    var ads = await adRepository.GetAllAdsAsync();
    //    await context.Response.WriteAsJsonAsync(ads);
    //}

    //private static async Task GetAdById(HttpContext context)
    //{
    //    var adRepository = context.RequestServices.GetService<IAdRepository>();
    //    var id = context.Request.RouteValues.As<int>("id");
    //    var ad = await adRepository.GetAdByIdAsync(id);
    //    await context.Response.WriteAsJsonAsync(ad);
    //}
}
