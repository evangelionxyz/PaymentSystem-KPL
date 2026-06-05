using Minimarket.Core.Models;
using Minimarket.Core.States;
using System.Diagnostics;
using System.Net.Http.Json;

namespace Desktop.Avalonia.Services;

/// <summary>
/// Typed HTTP client wrapping all API calls to Minimarket.API.
/// Base URL is configured from the appsettings or hardcoded for now.
/// </summary>
public class ApiClient
{
    private readonly HttpClient _http;
    public const string BaseUrl = "https://localhost:5241";

    public ApiClient()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };
        _http = new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) };
    }

    // ===========================
    //     Products
    // ===========================
    public async Task<List<Product>> GetProductsAsync() => 
        await _http.GetFromJsonAsync<List<Product>>("/api/products") ?? new();

    public async Task<Product?> CreateProductAsync(Product p)
    {
        var res = await _http.PostAsJsonAsync("/api/products", p);
        if (!res.IsSuccessStatusCode)
        {
            Trace.WriteLine(res.RequestMessage);
            return null;
        }
        return await res.Content.ReadFromJsonAsync<Product>();
    }

    public async Task<bool> UpdateProductAsync(string id, Product p)
    {
        var res = await _http.PutAsJsonAsync($"/api/products/{id}", p);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteProductAsync(string id)
    {
        var res = await _http.DeleteAsync($"/api/products/{id}");
        return res.IsSuccessStatusCode;
    }

    // ===========================
    //     Cart
    // ===========================
    public async Task<Cart?> AddToCartAsync(string cartId, string productId, int qty)
    {
        var res = await _http.PostAsJsonAsync("/api/cart/add", new { CartId = cartId, ProductId = productId, Quantity = qty });
        
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


    // ===========================
    //     Payment
    // ===========================
    public async Task<Receipt?> ProcessPaymentAsync(string cartId, PaymentMethod method, string? customerId = null)
    {
        var res = await _http.PostAsJsonAsync("/api/payments",
            new { CartId = cartId, Method = (int)method, CustomerId = customerId });
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<Receipt>();
    }

    // ===========================
    //     Config
    // ===========================
    public async Task<List<PricingRule>> GetPricingRulesAsync() =>
        await _http.GetFromJsonAsync<List<PricingRule>>("/api/rules/pricing") ?? new();

    public async Task<List<MachineStateTransition>> GetMachineStatesAsync() =>
        await _http.GetFromJsonAsync<List<MachineStateTransition>>("/api/config/machine-states") ?? new();

    // ===========================
    //     Receipts
    // ===========================
    public async Task<Receipt?> GetReceiptAsync(string id) =>
        await _http.GetFromJsonAsync<Receipt>($"/api/receipts/{id}");

    // ===========================
    //     Authentication
    // ===========================
    public async Task<User?> LoginAsync(string username, string password)
    {
        var res = await _http.PostAsJsonAsync("/api/auth/login", new { Username = username, Password = password });
        if (!res.IsSuccessStatusCode)
        {
            Trace.WriteLine(res.RequestMessage);
            return null;
        }
        return await res.Content.ReadFromJsonAsync<User>();
    }

    public async Task<User?> RegisterAsync(User user)
    {
        var res = await _http.PostAsJsonAsync("/api/auth/register", user);
        if (!res.IsSuccessStatusCode)
        {
            Trace.WriteLine(res.RequestMessage);
            return null;
        }
        return await res.Content.ReadFromJsonAsync<User>();
    }

    // ===========================
    //     Cashier & Audit
    // ===========================
    public async Task<List<Cart>> GetPendingCartsAsync() => await _http.GetFromJsonAsync<List<Cart>>("/api/cart/pending") ?? new();

    public async Task<List<AuditLog>> GetAuditLogsAsync(string transactionId) => await _http.GetFromJsonAsync<List<AuditLog>>($"/api/audit/logs/{transactionId}") ?? new();

    public async Task LogTransitionAsync(AuditLog log)
    {
        var res = await _http.PostAsJsonAsync("/api/audit/logs", log);
        res.EnsureSuccessStatusCode();
    }
}
