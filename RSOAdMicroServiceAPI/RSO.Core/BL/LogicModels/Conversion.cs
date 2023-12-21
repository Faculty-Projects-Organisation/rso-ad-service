using Newtonsoft.Json;

namespace  RSO.Core.BL.LogicModels;

public class Conversion
{
    [JsonProperty("base_currency_code")]
    public string BaseCurrencyCode { get; set; }

    [JsonProperty("base_currency_name")]
    public string BaseCurrencyName { get; set; }

    [JsonProperty("amount")]
    public string Amount { get; set; }

    [JsonProperty("updated_date")]
    public DateTime UpdatedDate { get; set; }

    [JsonProperty("rates")]
    public Dictionary<string, Rate> Rates { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }
}

public class Rate
{
    [JsonProperty("currency_name")]
    public string CurrencyName { get; set; }

    [JsonProperty("rate")]
    public string RateValue { get; set; }

    [JsonProperty("rate_for_amount")]
    public string RateForAmount { get; set; }
}
/*
 {
  "base_currency_code": "AUD",
  "base_currency_name": "Australian dollar",
  "amount": "1.0000",
  "updated_date": "2023-12-21",
  "rates": {
    "CAD": {
      "currency_name": "Canadian dollar",
      "rate": "0.9025",
      "rate_for_amount": "0.9025"
    }
  },
  "status": "success"
}
 
 
 */