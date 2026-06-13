using Minimarket.Core.Engines;
using Minimarket.Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Minimarket.API.Services;

public class CartService(
    IOptions<Settings> settings,
    IMongoClient client,
    ProductService productService,
    PricingRuleService ruleService,
    ILogger<CartService> logger)
{
    private readonly IMongoCollection<Cart> _carts = client.GetDatabase(settings.Value.DatabaseName).GetCollection<Cart>(settings.Value.CartCollectionName);
    public async Task<List<Cart>> GetAsync() => await _carts.Find(_ => true).ToListAsync();
    public async Task<Cart?> GetAsync(string id) => await _carts.Find(c => c.ID == id).FirstOrDefaultAsync();
    public async Task CreateAsync(Cart cart) => await _carts.InsertOneAsync(cart);
    public async Task UpdateAsync(string id, Cart cart) => await _carts.ReplaceOneAsync(c => c.ID == id, cart);
    public async Task RemoveAsync(string id) => await _carts.DeleteOneAsync(c => c.ID == id);

    /// <summary>Adds qty units of productId to the cart, creating it if needed.</summary>
    public async Task<Cart> AddItemAsync(string cartId, string productId, int quantity)
    {
        var cart = await GetAsync(cartId) ?? new Cart { ID = cartId };
        var product = await productService.GetAsync(productId)
            ?? throw new KeyNotFoundException($"Product '{productId}' not found.");

        var existing = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is not null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                ProductId   = product.ID!,
                ProductName = product.Name,
                CategoryId  = product.CategoryId ?? string.Empty,
                UnitPrice   = product.Price,
                Quantity    = quantity,
            });
        }

        if (cart.IsCheckedOut) cart.IsCheckedOut = false;

        await _carts.ReplaceOneAsync(
            c => c.ID == cartId, cart,
            new ReplaceOptions { IsUpsert = true });

        return cart;
    }

    /// <summary>Removes all units of productId from the cart.</summary>
    public async Task<Cart> RemoveItemAsync(string cartId, string productId)
    {
        var cart = await GetAsync(cartId)
            ?? throw new KeyNotFoundException($"Cart '{cartId}' not found.");

        cart.Items.RemoveAll(i => i.ProductId == productId);
        await UpdateAsync(cartId, cart);
        return cart;
    }

    /// <summary>
    /// Applies discount and tax engines to the cart using the stored pricing rules,
    /// persists the priced cart, and marks it as checked out.
    /// </summary>
    public async Task<Cart> CheckoutAsync(string cartId)
    {
        var cart = await GetAsync(cartId)
            ?? throw new KeyNotFoundException($"Cart '{cartId}' not found.");

        // Reset pricing from previous checkout attempt.
        cart.DiscountAmount = 0;
        cart.TaxAmount = 0;
        foreach (var item in cart.Items) item.DiscountAmount = 0;

        // Load rules from DB.
        var allRules = await ruleService.GetAllAsync();

        // Subtotal (before discount/tax).
        cart.Subtotal = cart.Items.Sum(i => i.UnitPrice * i.Quantity);

        // Apply discounts.
        var discountRules = allRules
            .Where(r => r.RuleType is "DiscountPercentage" or "BuyXGetY")
            .Select(r => new DiscountRule
            {
                RuleType   = r.RuleType,
                Priority   = r.Priority,
                CategoryId = r.CategoryId,
                ProductId  = r.ProductId,
                Condition  = r.Condition,
                Value      = r.Value,
            });

        var discountEngine = new DiscountEngine(logger: null);
        cart = discountEngine.Compute(cart, discountRules);

        // Apply taxes.
        var taxRules = allRules
            .Where(r => r.RuleType == "TaxRate")
            .Select(r => new TaxRule
            {
                RuleType   = r.RuleType,
                Priority   = r.Priority,
                CategoryId = r.CategoryId ?? string.Empty,
                Rate       = r.Value,
            });

        var taxEngine = new TaxEngine(logger: null);
        cart = taxEngine.Compute(cart, taxRules);

        cart.Total = cart.Subtotal - cart.DiscountAmount + cart.TaxAmount;
        cart.IsCheckedOut = true;

        await UpdateAsync(cartId, cart);
        return cart;
    }
}
