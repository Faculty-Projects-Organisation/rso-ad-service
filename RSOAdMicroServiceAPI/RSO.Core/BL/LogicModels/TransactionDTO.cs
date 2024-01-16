public class TransactionDTO
{
    public int SellerId { get; set; }
    public int BuyerId { get; set; }
    public int AdId { get; set; }
    public int? PriceActual { get; set; }
    public DateTime DateTime { get; set; }
}
