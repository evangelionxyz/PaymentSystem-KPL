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
        Register("ServiceFee", ApplyServiceFee);
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

    private static Cart ApplyServiceFee(Cart cart, TaxRule rule)
    {
        foreach (var item in cart.Items)
        {
            bool matchesCategory = string.IsNullOrEmpty(rule.CategoryId) || item.CategoryId == rule.CategoryId;
            if (!matchesCategory) continue;

            decimal fee;
            if (rule.Condition == "Flat")
                fee = rule.Rate * item.Quantity;
            else
                fee = item.LineTotal * rule.Rate;
            cart.TaxAmount += Math.Round(fee, 2);
        }
        return cart;
    }
}
