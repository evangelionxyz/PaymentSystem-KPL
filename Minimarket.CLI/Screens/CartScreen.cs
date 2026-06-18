namespace Minimarket.CLI.Screens;

/// <summary>
/// Handles the shopping cart screen.
/// Displays cart contents with totals and allows item removal, checkout, or cancellation.
/// </summary>
public class CartScreen
{
    private readonly CLISession _session;

    /// <summary>Represents the navigation result from the cart screen.</summary>
    public enum Result { GoToPayment, BackToProducts, CancelTransaction }

    /// <summary>Initializes the cart screen with the given session.</summary>
    public CartScreen(CLISession session) => _session = session;

    /// <summary>Runs the cart screen loop until the user navigates away.</summary>
    public async Task<Result> RunAsync()
    {
        while (true)
        {
            PrintScreen();

            Console.Write("\nEnter item # to remove, [C] Checkout, [B] Back, [X] Cancel > ");
            var input = Console.ReadLine()?.Trim() ?? string.Empty;

            if (input.Equals("X", StringComparison.OrdinalIgnoreCase))
                return Result.CancelTransaction;

            if (input.Equals("B", StringComparison.OrdinalIgnoreCase))
                return Result.BackToProducts;

            if (input.Equals("C", StringComparison.OrdinalIgnoreCase))
            {
                if (_session.CartItems.Count == 0)
                {
                    Console.WriteLine("\n  ✗ Cart is empty. Add items first.");
                    System.Threading.Thread.Sleep(1000);
                    continue;
                }
                _session.TriggerFsm("CartConfirmed");
                return Result.GoToPayment;
            }

            if (int.TryParse(input, out var idx) && idx >= 1 && idx <= _session.CartItems.Count)
            {
                await RemoveItemAsync(idx - 1);
            }
            else
            {
                Console.WriteLine("  Invalid input. Press Enter/Key.");
                if (Console.IsInputRedirected) Console.ReadLine();
                else Console.ReadKey(intercept: true);
            }
        }
    }

    private void PrintScreen()
    {
        ConsoleHelper.Clear();
        Console.WriteLine("╔══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  CART                                                ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════╝");

        if (_session.CartItems.Count == 0)
        {
            Console.WriteLine("\n  (cart is empty)\n");
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine($"  {"#",-4} {"Product",-28} {"Qty",4} {"Price",10} {"Total",12}");
            Console.WriteLine($"  {new string('-', 60)}");
            for (int i = 0; i < _session.CartItems.Count; i++)
            {
                var item = _session.CartItems[i];
                Console.WriteLine($"  {i + 1,-4} {item.ProductName,-28} {item.Quantity,4} {item.UnitPrice,10:N0} {item.LineTotal,12:N0}");
            }

            Console.WriteLine($"\n  {new string('─', 60)}");
            Console.WriteLine($"  {"Subtotal:",-36} {_session.Subtotal,12:N0}");
            if (_session.DiscountAmount > 0)
                Console.WriteLine($"  {"Discount:",-36} -{_session.DiscountAmount,11:N0}");
            if (_session.TaxAmount > 0)
                Console.WriteLine($"  {"Tax:",-36} {_session.TaxAmount,12:N0}");
            Console.WriteLine($"  {"TOTAL:",-36} {_session.Total,12:N0}");
            Console.WriteLine($"  {new string('─', 60)}");
        }

        Console.WriteLine($"\n  FSM State: {_session.Fsm.CurrentState}");
    }

    private async Task RemoveItemAsync(int index)
    {
        var item = _session.CartItems[index];
        try
        {
            var cart = await _session.Api.RemoveFromCartAsync(_session.CartId, item.ProductId);
            if (cart is not null)
            {
                _session.RefreshCart(cart);
                Console.WriteLine($"\n  ✓ Removed \"{item.ProductName}\".");
            }
            else
            {
                Console.WriteLine("\n  ✗ Could not remove item.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n  ✗ Error: {ex.Message}");
        }
        System.Threading.Thread.Sleep(600);
    }
}

