using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Minimarket.Core.Engines;
using Minimarket.Core.Models;
using Minimarket.Core.States;
using Minimarket.Core.Services;
using Desktop.Avalonia.Services;
using System.Diagnostics;
using MongoDB.Bson;

namespace Desktop.Avalonia.ViewModels;

public class CartViewModel : INotifyPropertyChanged
{
    private readonly ApiClient _api;
    private readonly PricingConfigCache _cache;
    private TransactionFSM _fsm;

    // Stable cart ID for this session — must be a valid MongoDB ObjectId (24-char hex).
    // Using ObjectId.GenerateNewId() instead of Guid to satisfy BsonRepresentation(ObjectId).
    public string CartId { get; set; } = ObjectId.GenerateNewId().ToString();

    private bool _isVip;
    public bool IsVip
    {
        get => _isVip;
        set { _isVip = value; OnPropertyChanged(); }
    }

    public ObservableCollection<CartItem> Items { get; } = new();

    private decimal _subtotal, _discountAmount, _taxAmount, _total;
    public decimal Subtotal       { get => _subtotal;       set { _subtotal = value;       OnPropertyChanged(); } }
    public decimal DiscountAmount { get => _discountAmount; set { _discountAmount = value; OnPropertyChanged(); } }
    public decimal TaxAmount      { get => _taxAmount;      set { _taxAmount = value;      OnPropertyChanged(); } }
    public decimal Total          { get => _total;          set { _total = value;          OnPropertyChanged(); } }

    public string FsmStateDisplay => _fsm.CurrentState.ToString();
    public TransactionState FsmState => _fsm.CurrentState;

    public CartViewModel(ApiClient api, PricingConfigCache cache)
    {
        _api   = api;
        _cache = cache;
        _fsm   = cache.CreateFsm();
    }

    // ======================================
    // FSM
    // ======================================
    public void TriggerFsm(string trigger)
    {
        // Guard: silently ignore triggers that have no valid transition from the current state
        var available = _fsm.AvailableTransitions();
        if (!available.Any(t => string.Equals(t.Trigger, trigger, StringComparison.OrdinalIgnoreCase)))
            return;

        _fsm.Trigger(trigger);
        OnPropertyChanged(nameof(FsmStateDisplay));
        OnPropertyChanged(nameof(FsmState));
    }

    // ======================================
    // Cart Operations
    // ======================================
    public async Task AddItemAsync(Product product)
    {
        try
        {
            // Online flow: Idle → AwaitingPayment on first item add.
            // If already AwaitingPayment (items already in cart), don't fire any trigger.
            if (_fsm.CurrentState == TransactionState.Idle)
            {
                TriggerFsm("CartConfirmed");
            }

            var cart = await _api.AddToCartAsync(CartId, product.ID!, 1);
            if (cart is not null)
            {
                RefreshItemsFromCart(cart);
                RecomputeTotalsLocally();
            }
        }
        catch (Exception e)
        {
            Trace.Assert(false, e.Message);    
        }
    }

    public async Task RemoveItemAsync(CartItem item)
    {
        try
        {
            var cart = await _api.RemoveFromCartAsync(CartId, item.ProductId);
            if (cart is not null)
            {
                RefreshItemsFromCart(cart);
                RecomputeTotalsLocally();
            }
        }
        catch { }
    }

    /// <summary>Locally recomputes subtotal/discount/tax using cached pricing rules.</summary>
    public void RecomputeTotalsLocally()
    {
        var cart = BuildLocalCart();

        // Subtotal
        cart.Subtotal = cart.Items.Sum(i => i.UnitPrice * i.Quantity);

        // Discount
        var discountRules = _cache.PricingRules
            .Where(r => r.RuleType is "DiscountPercentage" or "BuyXGetY")
            .Select(r => new DiscountRule
            {
                RuleType = r.RuleType, Priority = r.Priority,
                CategoryId = r.CategoryId, ProductId = r.ProductId,
                Condition = r.Condition, Value = r.Value,
            });
        cart = new DiscountEngine().Compute(cart, discountRules);

        // Tax
        var taxRules = _cache.PricingRules
            .Where(r => r.RuleType == "TaxRate")
            .Select(r => new TaxRule
            {
                RuleType = r.RuleType, Priority = r.Priority,
                CategoryId = r.CategoryId ?? string.Empty, Rate = r.Value,
            });
        cart = new TaxEngine().Compute(cart, taxRules);

        cart.Total = cart.Subtotal - cart.DiscountAmount + cart.TaxAmount;

        Subtotal       = cart.Subtotal;
        DiscountAmount = cart.DiscountAmount;
        TaxAmount      = cart.TaxAmount;
        Total          = cart.Total;
    }

    private Cart BuildLocalCart()
    {
        var c = new Cart { ID = CartId };
        foreach (var i in Items) c.Items.Add(i);
        return c;
    }

    private void RefreshItemsFromCart(Cart cart)
    {
        Items.Clear();
        foreach (var i in cart.Items) Items.Add(i);
    }

    public void Reset()
    {
        Items.Clear();
        Subtotal = DiscountAmount = TaxAmount = Total = 0;
        CartId = ObjectId.GenerateNewId().ToString();
        _fsm = _cache.CreateFsm();
        OnPropertyChanged(nameof(FsmStateDisplay));
        OnPropertyChanged(nameof(CartId));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
