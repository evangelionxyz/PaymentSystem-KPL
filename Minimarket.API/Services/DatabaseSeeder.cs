using Minimarket.Core.Models;
using Minimarket.Core.States;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Minimarket.API.Services;

/// <summary>
/// Idempotent seeder that populates the pricingRules and machineStates
/// collections with defaults on first startup. Run before app.Run().
/// </summary>
public class DatabaseSeeder
{
    private readonly IMongoCollection<PricingRule> _rulesCollection;
    private readonly IMongoCollection<MachineStateTransition> _statesCollection;
    private readonly IMongoCollection<User> _usersCollection;

    public DatabaseSeeder(IMongoClient client, IOptions<Settings> settings)
    {
        var db = client.GetDatabase(settings.Value.DatabaseName);
        _rulesCollection = db.GetCollection<PricingRule>(settings.Value.PricingRuleCollectionName);
        _statesCollection = db.GetCollection<MachineStateTransition>(settings.Value.MachineStateCollectionName);
        _usersCollection = db.GetCollection<User>(settings.Value.UserCollectionName);
    }

    /// <summary>Inserts default pricing rules if the collection is empty.</summary>
    public async Task SeedPricingRulesAsync()
    {
        if (await _rulesCollection.CountDocumentsAsync(FilterDefinition<PricingRule>.Empty) > 0)
            return;

        var defaults = new List<PricingRule>
        {
            new() {
                RuleType   = "DiscountPercentage",
                CategoryId = null,   // will be updated when categories seeded
                ProductId  = null,
                Condition  = "CategoryName=Snack",
                Value      = 10,
                Priority   = 1
            },
            new() {
                RuleType   = "BuyXGetY",
                ProductId  = null,
                CategoryId = null,
                Condition  = "X=2,Y=1",
                Value      = 0,
                Priority   = 2
            },
        };

        await _rulesCollection.InsertManyAsync(defaults);
    }

    /// <summary>Ensures the FSM transition table matches the current payment flow.</summary>
    public async Task SeedMachineStatesAsync()
    {
        var expectedTransitions = GetDefaultMachineStates();
        var existingTransitions = await _statesCollection.Find(FilterDefinition<MachineStateTransition>.Empty).ToListAsync();

        if (IsSameTransitionTable(existingTransitions, expectedTransitions))
            return;

        if (existingTransitions.Count > 0)
        {
            await _statesCollection.DeleteManyAsync(FilterDefinition<MachineStateTransition>.Empty);
        }

        await _statesCollection.InsertManyAsync(expectedTransitions);
    }
    private static bool IsSameTransitionTable(IEnumerable<MachineStateTransition> existing, IEnumerable<MachineStateTransition> expected)
    {
        var existingNormalized = existing
            .Select(t => $"{(uint)t.From}|{t.Trigger}|{(uint)t.To}")
            .OrderBy(x => x)
            .ToList();

        var expectedNormalized = expected
            .Select(t => $"{(uint)t.From}|{t.Trigger}|{(uint)t.To}")
            .OrderBy(x => x)
            .ToList();

        return existingNormalized.SequenceEqual(expectedNormalized);
    }

    private static List<MachineStateTransition> GetDefaultMachineStates() =>
    [
        new() { From = TransactionState.Idle,              To = TransactionState.AwaitingPayment,   Trigger = "CartConfirmed" },
        new() { From = TransactionState.AwaitingPayment,   To = TransactionState.ProcessingPayment, Trigger = "PaymentSelected" },
        new() { From = TransactionState.ProcessingPayment, To = TransactionState.Completed,         Trigger = "PaymentConfirmed" },
        new() { From = TransactionState.ProcessingPayment, To = TransactionState.Cancelled,         Trigger = "PaymentFailed" },
        new() { From = TransactionState.Completed,         To = TransactionState.Idle,              Trigger = "Reset" },
        new() { From = TransactionState.Cancelled,         To = TransactionState.Idle,              Trigger = "Reset" },
    ];

    /// <summary>Inserts default customer and cashier users if the collection is empty.</summary>
    public async Task SeedUsersAsync()
    {
        if (await _usersCollection.CountDocumentsAsync(FilterDefinition<User>.Empty) > 0)
            return;

        var defaults = new List<User>
        {
            new() { Username = "customer", Password = "password" },
            new() { Username = "cashier", Password = "password" }
        };

        await _usersCollection.InsertManyAsync(defaults);
    }
}
