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

    private const string RulesCacheFile = "pricing_rules_cache.json";
    private const string StatesCacheFile = "machine_states_cache.json";

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

            // Save to local cache files
            try
            {
                var rulesJson = System.Text.Json.JsonSerializer.Serialize(PricingRules);
                var statesJson = System.Text.Json.JsonSerializer.Serialize(MachineStates);
                await File.WriteAllTextAsync(RulesCacheFile, rulesJson);
                await File.WriteAllTextAsync(StatesCacheFile, statesJson);
            }
            catch { /* Ignore disk write errors */ }
        }
        catch (Exception)
        {
            IsConnected = false;
            // Fallback to local cache files
            try
            {
                if (File.Exists(RulesCacheFile) && File.Exists(StatesCacheFile))
                {
                    var rulesJson = await File.ReadAllTextAsync(RulesCacheFile);
                    var statesJson = await File.ReadAllTextAsync(StatesCacheFile);
                    PricingRules = System.Text.Json.JsonSerializer.Deserialize<List<PricingRule>>(rulesJson) ?? new();
                    MachineStates = System.Text.Json.JsonSerializer.Deserialize<List<MachineStateTransition>>(statesJson) ?? new();
                }
                else
                {
                    PricingRules = new();
                    MachineStates = new();
                }
            }
            catch
            {
                PricingRules = new();
                MachineStates = new();
            }
        }
    }

    /// <summary>Creates a fresh TransactionFSM from the cached transition table.</summary>
    public TransactionFSM CreateFsm() => new TransactionFSM(MachineStates);
}
