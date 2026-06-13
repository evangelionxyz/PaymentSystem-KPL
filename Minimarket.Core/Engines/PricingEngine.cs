using Minimarket.Core.Models;
using Microsoft.Extensions.Logging;

namespace Minimarket.Core.Engines;

/// <summary>
/// Abstract generic pricing engine. Concrete engines register rule handlers
/// in a dispatch table (Dictionary&lt;string, Func&gt;), satisfying both the
/// parameterization/generics and table-driven construction requirements.
/// </summary>
public abstract class PricingEngine<TRule> where TRule : IPricingRule
{
    private readonly ILogger? _logger;

    // Table-driven dispatch: maps RuleType string → handler function.
    private readonly Dictionary<string, Func<Cart, TRule, Cart>> _handlers = new();

    protected PricingEngine(ILogger? logger = null)
    {
        _logger = logger;
    }

    /// <summary>
    /// Registers a handler for a given RuleType string.
    /// Call this in the concrete constructor to populate the dispatch table.
    /// </summary>
    protected void Register(string ruleType, Func<Cart, TRule, Cart> handler)
    {
        _handlers[ruleType] = handler;
    }

    /// <summary>
    /// Applies all rules in priority order, dispatching each via the handler table.
    /// Unknown rule types are logged and skipped (task 2.7).
    /// </summary>
    public Cart Compute(Cart cart, IEnumerable<TRule> rules)
    {
        foreach (var rule in rules.OrderBy(r => r.Priority))
        {
            if (_handlers.TryGetValue(rule.RuleType, out var handler))
            {
                cart = handler(cart, rule);
            }
            else
            {
                _logger?.LogWarning(
                    "PricingEngine: Unknown rule type '{RuleType}' (priority {Priority}) — skipped.",
                    rule.RuleType, rule.Priority);
            }
        }
        return cart;
    }
}
