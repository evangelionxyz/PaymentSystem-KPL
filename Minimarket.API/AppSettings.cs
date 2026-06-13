namespace Minimarket.API;

public class PricingSettings
{
    public decimal DefaultDiscountPercent { get; set; } = 0;
    public string CurrencyCode { get; set; } = "IDR";
}

public class TaxSettings
{
    public Dictionary<string, decimal> Rates { get; set; } = new();
}

public class PaymentFeeSettings
{
    public Dictionary<string, decimal> Fees { get; set; } = new();
}
