namespace Minimarket.Core.Engines;

/// <summary>
/// Marker interface for all pricing rule types consumed by PricingEngine&lt;TRule&gt;.
/// Satisfies the parameterization/generics requirement.
/// </summary>
public interface IPricingRule
{
    string RuleType { get; }
    int Priority { get; }
}
