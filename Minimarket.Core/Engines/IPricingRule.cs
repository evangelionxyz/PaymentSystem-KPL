namespace Minimarket.Core.Engines;

public interface IPricingRule
{
    string RuleType { get; }
    int Priority { get; }
}
