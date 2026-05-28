namespace Minimarket.API;

/// <summary>Bound from appsettings.json "Pricing" section via IOptions&lt;PricingSettings&gt;.</summary>
public class PricingSettings
{
    public decimal DefaultDiscountPercent { get; set; } = 0;
    public string CurrencyCode { get; set; } = "IDR";
}

/// <summary>Bound from appsettings.json "Tax" section via IOptions&lt;TaxSettings&gt;.</summary>
public class TaxSettings
{
    /// <summary>Maps category name → tax rate (0.11 = 11%).</summary>
    public Dictionary<string, decimal> Rates { get; set; } = new();
}

/// <summary>Bound from appsettings.json "PaymentFees" section via IOptions&lt;PaymentFeeSettings&gt;.</summary>
public class PaymentFeeSettings
{
    /// <summary>Maps PaymentMethod name → fee fraction (0.007 = 0.7%).</summary>
    public Dictionary<string, decimal> Fees { get; set; } = new();
}
