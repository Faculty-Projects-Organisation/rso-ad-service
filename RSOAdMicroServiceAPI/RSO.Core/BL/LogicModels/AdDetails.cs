using RSO.Core.AdModels;
using System.Globalization;

namespace RSO.Core.BL.LogicModels;

public class AdDetails
{
    public AdDetails(Ad add, string hufPrice)
    {
        Id = add.ID;
        UserId = add.UserId;
        if (Price <= 0)
        {
            Price = 0;
            HufPrice = 0;
        }
        else
        {
            Price = add.Price;
            HufPrice = double.Parse(hufPrice,CultureInfo.InvariantCulture);
        }
        Status = add.Status;
        Category = add.Category;
        Thing = add.Thing;
        PostTime = add.PostTime;
    }

    public int Id { get; }
    public int UserId { get; }
    public int? Price { get; }
    public double HufPrice { get; }
    public string Category { get; }
    public string Thing { get; }
    public string Status { get; }
    public DateTime PostTime { get; }
}
