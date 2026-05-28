using Microsoft.Extensions.Logging;

namespace Minimarket.Core.Engines;

/// <summary>
/// Concrete rule type consumed by DiscountEngine.
/// Maps from a PricingRule document; only discount-relevant fields are used.
/// </summary>
public class DiscountRule : IPricingRule
{
    public string RuleType { get; set; } = string.Empty;
    public int Priority { get; set; }
    public string? CategoryId { get; set; }
    public string? ProductId { get; set; }
    /// <summary>e.g. "X=2,Y=1" for BuyXGetY</summary>
    public string? Condition { get; set; }
    /// <summary>Percentage value (0–100) for DiscountPercentage.</summary>
    public decimal Value { get; set; }
}
