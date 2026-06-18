using Minimarket.Core.Engines;
using Minimarket.Core.Models;
using Minimarket.Core.Services;
using Minimarket.Core.States;
using MongoDB.Bson;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Minimarket.CLI;

/// <summary>
/// Holds all state for a single user session in the CLI application.
/// Mirrors the role played by the combined ViewModels in the Avalonia GUI.
/// </summary>
public class CLISession
{
    /// <summary>Gets the shared API client used for all server calls.</summary>
    public ApiClient Api { get; }

    /// <summary>Gets the cached pricing rules and FSM transitions fetched at startup.</summary>
    public PricingConfigCache Cache { get; }

    /// <summary>Gets or sets the authenticated user for this session.</summary>
    public User? AuthenticatedUser { get; set; }

    /// <summary>Gets or sets the current cart ID (MongoDB ObjectId string).</summary>
    public string CartId { get; set; } = ObjectId.GenerateNewId().ToString();

    /// <summary>Gets or sets whether the authenticated user is a VIP.</summary>
    public bool IsVip { get; set; }

    /// <summary>Gets the items currently in the cart.</summary>
    public List<CartItem> CartItems { get; private set; } = new();

    /// <summary>Gets the current cart subtotal (before discount and tax).</summary>
    public decimal Subtotal { get; private set; }

    /// <summary>Gets the total discount applied to the cart.</summary>
    public decimal DiscountAmount { get; private set; }

    /// <summary>Gets the total tax applied to the cart.</summary>
    public decimal TaxAmount { get; private set; }

    /// <summary>Gets the cart total (subtotal - discount + tax).</summary>
    public decimal Total { get; private set; }

    /// <summary>Gets the FSM controlling transaction state transitions.</summary>
    public TransactionFSM Fsm { get; private set; }

    /// <summary>Gets the last receipt from a successful payment, if any.</summary>
    public Receipt? LastReceipt { get; set; }

    /// <summary>Gets the payment surcharge applied during the last payment selection.</summary>
    public decimal PaymentFee { get; set; }

    /// <summary>Gets the final total including payment surcharge.</summary>
    public decimal FinalTotal { get; set; }

    /// <summary>Gets the selected payment method for the current transaction.</summary>
    public PaymentMethod SelectedPaymentMethod { get; set; } = PaymentMethod.Cash;

    private static readonly Dictionary<string, decimal> _feeRates = new()
    {
        ["Cash"] = 0m,
        ["EWallet"] = 0.005m,
        ["BankTransfer"] = 0.003m,
        ["QRIS"] = 0.007m,
        ["CreditCard"] = 0.015m,
    };

    /// <summary>Initializes a new session with the given API client and pricing cache.</summary>
    public CLISession(ApiClient api, PricingConfigCache cache)
    {
        Api = api;
        Cache = cache;
        Fsm = cache.CreateFsm();
    }

    /// <summary>
    /// Triggers a state machine transition, silently ignoring unavailable triggers.
    /// </summary>
    public void TriggerFsm(string trigger)
    {
        var available = Fsm.AvailableTransitions();
        if (!available.Any(t => string.Equals(t.Trigger, trigger, StringComparison.OrdinalIgnoreCase)))
            return;
        Fsm.Trigger(trigger);
    }

    /// <summary>
    /// Refreshes the in-memory cart items from a <see cref="Cart"/> object returned by the API.
    /// </summary>
    public void RefreshCart(Cart cart)
    {
        CartItems = cart.Items.ToList();
        RecomputeTotals();
    }

    /// <summary>
    /// Recomputes subtotal, discount, tax, and total locally using cached pricing rules.
    /// </summary>
    public void RecomputeTotals()
    {
        var cart = new Cart { ID = CartId };
        foreach (var i in CartItems) cart.Items.Add(i);

        cart.Subtotal = cart.Items.Sum(i => i.UnitPrice * i.Quantity);

        var discountRules = Cache.PricingRules
            .Where(r => r.RuleType is "DiscountPercentage" or "BuyXGetY")
            .Select(r => new DiscountRule
            {
                RuleType = r.RuleType,
                Priority = r.Priority,
                CategoryId = r.CategoryId,
                ProductId = r.ProductId,
                Condition = r.Condition,
                Value = r.Value,
            });
        cart = new DiscountEngine().Compute(cart, discountRules);

        var taxRules = Cache.PricingRules
            .Where(r => r.RuleType == "TaxRate")
            .Select(r => new TaxRule
            {
                RuleType = r.RuleType,
                Priority = r.Priority,
                CategoryId = r.CategoryId ?? string.Empty,
                Rate = r.Value,
            });
        cart = new TaxEngine().Compute(cart, taxRules);

        cart.Total = cart.Subtotal - cart.DiscountAmount + cart.TaxAmount;

        Subtotal = cart.Subtotal;
        DiscountAmount = cart.DiscountAmount;
        TaxAmount = cart.TaxAmount;
        Total = cart.Total;
    }

    /// <summary>
    /// Computes the payment fee and final total for the given payment method.
    /// </summary>
    public void SelectPaymentMethod(PaymentMethod method)
    {
        SelectedPaymentMethod = method;
        var rate = _feeRates.TryGetValue(method.ToString(), out var r) ? r : 0m;
        PaymentFee = Math.Round(Total * rate, 2);
        FinalTotal = Total + PaymentFee;
    }

    /// <summary>
    /// Resets the session for a new transaction: clears the cart, generates a new cart ID, resets the FSM.
    /// </summary>
    public void StartNewTransaction()
    {
        TriggerFsm("Reset");
        CartItems.Clear();
        Subtotal = DiscountAmount = TaxAmount = Total = PaymentFee = FinalTotal = 0;
        CartId = ObjectId.GenerateNewId().ToString();
        LastReceipt = null;
        Fsm = Cache.CreateFsm();
    }
}
