using Minimarket.Core.Models;
using Microsoft.Extensions.Logging;

namespace Minimarket.Core.Engines;

/// <summary>
/// Applies discount rules to a Cart. Supports two RuleTypes registered in
/// the parent's dispatch table:
///   - "DiscountPercentage": reduces matching line totals by Value%.
///   - "BuyXGetY": parses Condition="X=2,Y=1" and gives Y free units per X purchased.
/// </summary>
public class DiscountEngine : PricingEngine<DiscountRule>
{
    public DiscountEngine(ILogger<DiscountEngine>? logger = null) : base(logger)
    {
        Register("DiscountPercentage", ApplyDiscountPercentage);
        Register("BuyXGetY", ApplyBuyXGetY);
        Register("TimeDiscount", ApplyTimeDiscount);
        Register("VipDiscount", ApplyVipDiscount);
    }

    private static Cart ApplyDiscountPercentage(Cart cart, DiscountRule rule)
    {
        foreach (var item in cart.Items)
        {
            bool matchesCategory = rule.CategoryId is not null &&
                                   item.CategoryId == rule.CategoryId;
            bool matchesProduct = rule.ProductId is not null &&
                                  item.ProductId == rule.ProductId;
            bool appliesToAll = rule.CategoryId is null && rule.ProductId is null;

            if (matchesCategory || matchesProduct || appliesToAll)
            {
                var discount = item.UnitPrice * item.Quantity * (rule.Value / 100m);
                item.DiscountAmount += Math.Round(discount, 2);
            }
        }
        // Recompute cart-level discount total.
        cart.DiscountAmount = cart.Items.Sum(i => i.DiscountAmount);
        return cart;
    }

    private static Cart ApplyBuyXGetY(Cart cart, DiscountRule rule)
    {
        if (rule.Condition is null) return cart;

        // Parse "X=2,Y=1"
        var parts = rule.Condition
            .Split(',')
            .Select(p => p.Split('='))
            .Where(p => p.Length == 2)
            .ToDictionary(p => p[0].Trim(), p => int.Parse(p[1].Trim()));

        if (!parts.TryGetValue("X", out int x) || !parts.TryGetValue("Y", out int y))
            return cart;

        foreach (var item in cart.Items)
        {
            bool matches = (rule.ProductId is not null && item.ProductId == rule.ProductId) ||
                           (rule.CategoryId is not null && item.CategoryId == rule.CategoryId);

            if (!matches) continue;

            int freeSets = item.Quantity / (x + y);
            int remainder = item.Quantity % (x + y);
            int freeFromRemainder = Math.Max(0, remainder - x);
            int freeUnits = freeSets * y + freeFromRemainder;

            item.DiscountAmount += Math.Round(item.UnitPrice * freeUnits, 2);
        }

        cart.DiscountAmount = cart.Items.Sum(i => i.DiscountAmount);
        return cart;
    }

    private static Cart ApplyTimeDiscount(Cart cart, DiscountRule rule)
    {
        if (rule.Condition is null) return cart;

        var parts = rule.Condition
            .Split(',')
            .Select(p => p.Split('='))
            .Where(p => p.Length == 2)
            .ToDictionary(p => p[0].Trim().ToLower(), p => p[1].Trim());

        if (!parts.TryGetValue("starttime", out var startStr) || !parts.TryGetValue("endtime", out var endStr))
            return cart;

        if (!TimeSpan.TryParse(startStr, out var startTime) || !TimeSpan.TryParse(endStr, out var endTime))
            return cart;

        var nowTime = DateTime.Now.TimeOfDay;
        bool inRange = nowTime >= startTime && nowTime <= endTime;

        if (inRange)
        {
            foreach (var item in cart.Items)
            {
                bool matchesCategory = rule.CategoryId is not null && item.CategoryId == rule.CategoryId;
                bool matchesProduct = rule.ProductId is not null && item.ProductId == rule.ProductId;
                bool appliesToAll = rule.CategoryId is null && rule.ProductId is null;

                if (matchesCategory || matchesProduct || appliesToAll)
                {
                    var discount = item.UnitPrice * item.Quantity * (rule.Value / 100m);
                    item.DiscountAmount += Math.Round(discount, 2);
                }
            }
        }

        cart.DiscountAmount = cart.Items.Sum(i => i.DiscountAmount);
        return cart;
    }

    private static Cart ApplyVipDiscount(Cart cart, DiscountRule rule)
    {
        foreach (var item in cart.Items)
        {
            bool matchesCategory = rule.CategoryId is not null && item.CategoryId == rule.CategoryId;
            bool matchesProduct = rule.ProductId is not null && item.ProductId == rule.ProductId;
            bool appliesToAll = rule.CategoryId is null && rule.ProductId is null;

            if (matchesCategory || matchesProduct || appliesToAll)
            {
                var discount = item.UnitPrice * item.Quantity * (rule.Value / 100m);
                item.DiscountAmount += Math.Round(discount, 2);
            }
        }

        cart.DiscountAmount = cart.Items.Sum(i => i.DiscountAmount);
        return cart;
    }
}
