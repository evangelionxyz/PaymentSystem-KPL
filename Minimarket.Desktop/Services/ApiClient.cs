using Minimarket.Core.Models;
using Minimarket.Core.States;
using System.Net.Http.Json;

namespace Desktop.Avalonia.Services;

/// <summary>
/// Typed HTTP client wrapping all API calls to Minimarket.API.
/// Base URL is configured from the appsettings or hardcoded for now.
/// </summary>
public class ApiClient
{
    private readonly HttpClient _http;
    public const string BaseUrl = "https://localhost:44378";

    public ApiClient()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };
        _http = new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) };
    }

    // ── Products ──────────────────────────────────────────────────────────────

    public async Task<List<Product>> GetProductsAsync() =>
        await _http.GetFromJsonAsync<List<Product>>("/api/product") ?? new();

    // ── Cart ──────────────────────────────────────────────────────────────────

    public async Task<Cart?> AddToCartAsync(string cartId, string productId, int qty)
    {
        var res = await _http.PostAsJsonAsync("/api/cart/add",
            new { CartId = cartId, ProductId = productId, Quantity = qty });
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<Cart>();
    }

    public async Task<Cart?> RemoveFromCartAsync(string cartId, string productId)
    {
        var res = await _http.PostAsJsonAsync("/api/cart/remove",
            new { CartId = cartId, ProductId = productId });
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<Cart>();
    }

    public async Task<Cart?> CheckoutAsync(string cartId)
    {
        var res = await _http.PostAsJsonAsync("/api/cart/checkout",
            new { CartId = cartId });
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<Cart>();
    }

    // ── Payment ───────────────────────────────────────────────────────────────

    public async Task<Receipt?> ProcessPaymentAsync(string cartId, PaymentMethod method, string? customerId = null)
    {
        var res = await _http.PostAsJsonAsync("/api/payment",
            new { CartId = cartId, Method = (int)method, CustomerId = customerId });
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<Receipt>();
    }

    // ── Config ────────────────────────────────────────────────────────────────

    public async Task<List<PricingRule>> GetPricingRulesAsync() =>
        await _http.GetFromJsonAsync<List<PricingRule>>("/api/rules/pricing") ?? new();

    public async Task<List<MachineStateTransition>> GetMachineStatesAsync() =>
        await _http.GetFromJsonAsync<List<MachineStateTransition>>("/api/config/machine-states") ?? new();

    // ── Receipts ──────────────────────────────────────────────────────────────

    public async Task<Receipt?> GetReceiptAsync(string id) =>
        await _http.GetFromJsonAsync<Receipt>($"/api/receipt/{id}");
}
