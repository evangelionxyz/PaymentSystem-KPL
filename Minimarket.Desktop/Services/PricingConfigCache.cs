using Minimarket.Core.Models;
using Minimarket.Core.States;

namespace Desktop.Avalonia.Services;

/// <summary>
/// Singleton cache that fetches pricing rules and FSM transitions from the API
/// on startup and makes them available to all ViewModels for local computation.
/// Falls back gracefully if the API is unreachable.
/// </summary>
public class PricingConfigCache
{
    private readonly ApiClient _api;

    public List<PricingRule> PricingRules { get; private set; } = new();
    public List<MachineStateTransition> MachineStates { get; private set; } = new();
    public bool IsConnected { get; private set; } = false;

    public PricingConfigCache(ApiClient api)
    {
        _api = api;
    }

    /// <summary>Call once on app startup. Populates cached rules.</summary>
    public async Task LoadAsync()
    {
        try
        {
            PricingRules  = await _api.GetPricingRulesAsync();
            MachineStates = await _api.GetMachineStatesAsync();
            IsConnected = true;
        }
        catch (Exception)
        {
            // API unreachable — use empty rules (base prices, no discounts).
            PricingRules  = new();
            MachineStates = new();
            IsConnected = false;
        }
    }

    /// <summary>Creates a fresh TransactionFSM from the cached transition table.</summary>
    public TransactionFSM CreateFsm() => new TransactionFSM(MachineStates);
}
