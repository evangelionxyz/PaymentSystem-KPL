using Minimarket.Core.Models;
using Microsoft.Extensions.Logging;

namespace Minimarket.Core.Engines;

/// <summary>
/// Applies category-level tax rates to a post-discount cart.
/// The "TaxRate" handler is registered in the parent dispatch table.
/// Tax is computed on post-discount line totals and accumulated in cart.TaxAmount.
/// </summary>
public class TaxEngine : PricingEngine<TaxRule>
{
    public TaxEngine(ILogger<TaxEngine>? logger = null) : base(logger)
    {
        Register("TaxRate", ApplyTaxRate);
    }

    private static Cart ApplyTaxRate(Cart cart, TaxRule rule)
    {
        foreach (var item in cart.Items.Where(i => i.CategoryId == rule.CategoryId))
        {
            var taxableAmount = item.LineTotal; // post-discount
            cart.TaxAmount += Math.Round(taxableAmount * rule.Rate, 2);
        }
        return cart;
    }
}
