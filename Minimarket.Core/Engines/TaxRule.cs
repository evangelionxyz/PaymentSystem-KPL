using Microsoft.Extensions.Logging;

namespace Minimarket.Core.Engines;

/// <summary>
/// Concrete rule type consumed by TaxEngine.
/// Applies a percentage tax to cart lines whose CategoryId matches.
/// </summary>
public class TaxRule : IPricingRule
{
    public string RuleType { get; set; } = "TaxRate";
    public int Priority { get; set; }
    /// <summary>The category name or ID this rate applies to.</summary>
    public string CategoryId { get; set; } = string.Empty;
    /// <summary>Tax rate as a decimal fraction, e.g. 0.11 for 11%.</summary>
    public decimal Rate { get; set; }
    public string Condition { get; set; } = string.Empty;
}
