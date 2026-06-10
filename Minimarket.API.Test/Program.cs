using System.Net.Http.Json;
using Minimarket.Core.Models;

var baseUrl = "https://localhost:4123";

using var handler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (_, _, _, _) => true
};

using var http = new HttpClient(handler)
{
    BaseAddress = new Uri(baseUrl)
};

Console.WriteLine($"API: {baseUrl}");

var pendingCarts = await http.GetFromJsonAsync<List<Cart>>("/api/cart/pending") ?? new();

if (pendingCarts.Count == 0)
{
    Console.WriteLine("No pending carts found.");
    return;
}

var cart = pendingCarts.First();

Console.WriteLine($"Using cart: {cart.ID}");
Console.WriteLine($"Subtotal: {cart.Subtotal}");
Console.WriteLine($"Total: {cart.Total}");
Console.WriteLine($"CustomerId: {cart.CustomerId ?? "(none)"}");

var request = new
{
    CartId = cart.ID,
    Method = (int)PaymentMethod.Cash,
    CustomerId = cart.CustomerId
};

var response = await http.PostAsJsonAsync("/api/payments", request);

if (!response.IsSuccessStatusCode)
{
    Console.WriteLine($"Payment failed: {(int)response.StatusCode} {response.ReasonPhrase}");
    Console.WriteLine(await response.Content.ReadAsStringAsync());
    return;
}

var receipt = await response.Content.ReadFromJsonAsync<Receipt>();

Console.WriteLine("Payment succeeded.");
Console.WriteLine($"Receipt ID: {receipt?.ID}");
Console.WriteLine($"Payment Method: {receipt?.PaymentMethod}");
Console.WriteLine($"Total: {receipt?.Total}");
Console.WriteLine($"Date: {receipt?.Date}");