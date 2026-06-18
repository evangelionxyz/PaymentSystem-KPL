using Minimarket.Core.Models;

namespace Minimarket.CLI.Screens;

/// <summary>
/// Handles the product browsing and search screen.
/// Allows the user to list products, search, add items to cart, and navigate to the cart.
/// </summary>
public class ProductScreen
{
    private readonly CLISession _session;
    private List<Product> _allProducts = new();
    private List<Product> _filtered = new();
    private string _searchText = string.Empty;

    /// <summary>
    /// Represents the navigation result from this screen.
    /// </summary>
    public enum Result { GoToCart, Exit }

    /// <summary>Initializes the product screen with the given session.</summary>
    public ProductScreen(CLISession session) => _session = session;

    /// <summary>
    /// Runs the product browsing loop. Returns when the user navigates to the cart or exits.
    /// </summary>
    public async Task<Result> RunAsync()
    {
        // Load products once when the screen is entered
        await LoadProductsAsync();

        while (true)
        {
            PrintScreen();

            Console.Write("\nEnter product # to add, [S] Search, [C] Cart, [X] Exit > ");
            var input = Console.ReadLine()?.Trim() ?? string.Empty;

            if (input.Equals("X", StringComparison.OrdinalIgnoreCase))
                return Result.Exit;

            if (input.Equals("C", StringComparison.OrdinalIgnoreCase))
                return Result.GoToCart;

            if (input.Equals("S", StringComparison.OrdinalIgnoreCase))
            {
                Console.Write("Search: ");
                _searchText = Console.ReadLine()?.Trim() ?? string.Empty;
                FilterProducts();
                continue;
            }

            if (int.TryParse(input, out var idx) && idx >= 1 && idx <= _filtered.Count)
            {
                await AddToCartAsync(_filtered[idx - 1]);
            }
            else
            {
                Console.WriteLine("  Invalid choice. Press Enter/Key.");
                if (Console.IsInputRedirected) Console.ReadLine();
                else Console.ReadKey(intercept: true);
            }
        }
    }

    private async Task LoadProductsAsync()
    {
        try
        {
            _allProducts = await _session.Api.GetProductsAsync();
            FilterProducts();
        }
        catch
        {
            _allProducts = new();
            _filtered = new();
        }
    }

    private void FilterProducts()
    {
        var q = _searchText.Trim().ToLowerInvariant();
        _filtered = string.IsNullOrEmpty(q)
            ? _allProducts.ToList()
            : _allProducts.Where(p =>
                p.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                (p.ID?.Contains(q) ?? false)).ToList();
    }

    private void PrintScreen()
    {
        ConsoleHelper.Clear();
        Console.WriteLine("╔══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  PRODUCTS                                            ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════╝");

        if (!_session.Cache.IsConnected)
            Console.WriteLine("  ⚠  API offline — operating with base prices only\n");

        if (!string.IsNullOrEmpty(_searchText))
            Console.WriteLine($"  Search: \"{_searchText}\"  ({_filtered.Count} results)\n");

        if (_filtered.Count == 0)
        {
            Console.WriteLine("  (no products found)");
        }
        else
        {
            Console.WriteLine($"  {"#",-4} {"Name",-30} {"Price",10}");
            Console.WriteLine($"  {new string('-', 46)}");
            for (int i = 0; i < _filtered.Count; i++)
            {
                var p = _filtered[i];
                Console.WriteLine($"  {i + 1,-4} {p.Name,-30} {p.Price,10:N0}");
            }
        }

        // Show mini cart summary
        if (_session.CartItems.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine($"  🛒 Cart: {_session.CartItems.Count} item(s) | Total: {_session.Total:N0}");
        }
    }

    private async Task AddToCartAsync(Product product)
    {
        try
        {
            var cart = await _session.Api.AddToCartAsync(_session.CartId, product.ID!, 1);
            if (cart is not null)
            {
                _session.RefreshCart(cart);
                Console.WriteLine($"\n  ✓ Added \"{product.Name}\" to cart.");
            }
            else
            {
                Console.WriteLine("\n  ✗ Could not add item.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n  ✗ Error: {ex.Message}");
        }
        System.Threading.Thread.Sleep(600);
    }
}
