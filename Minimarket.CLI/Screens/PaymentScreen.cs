using Minimarket.Core.Models;

namespace Minimarket.CLI.Screens;

/// <summary>
/// Handles the payment selection screen.
/// Shows available payment methods, computes surcharge, and confirms payment via the API.
/// </summary>
public class PaymentScreen
{
    private readonly CLISession _session;

    /// <summary>Represents the navigation result from the payment screen.</summary>
    public enum Result { PaymentSucceeded, BackToCart }

    /// <summary>Initializes the payment screen with the given session.</summary>
    public PaymentScreen(CLISession session) => _session = session;

    /// <summary>Runs the payment screen loop.</summary>
    public async Task<Result> RunAsync()
    {
        var methods = Enum.GetValues<PaymentMethod>();

        while (true)
        {
            PrintMethodMenu(methods);

            Console.Write("Select payment method # > ");
            var input = Console.ReadLine()?.Trim() ?? string.Empty;

            if (input.Equals("B", StringComparison.OrdinalIgnoreCase))
                return Result.BackToCart;

            if (!int.TryParse(input, out var idx) || idx < 1 || idx > methods.Length)
            {
                Console.WriteLine("  Invalid choice. Press Enter/Key.");
                if (Console.IsInputRedirected) Console.ReadLine();
                else Console.ReadKey(intercept: true);
                continue;
            }

            var method = methods[idx - 1];
            _session.SelectPaymentMethod(method);

            // Show fee breakdown and ask for confirmation
            ConsoleHelper.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════╗");
            Console.WriteLine("║  PAYMENT CONFIRMATION                                ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine($"  Method:        {method}");
            Console.WriteLine($"  Cart Total:    {_session.Total,14:N0}");
            Console.WriteLine($"  Surcharge:     {_session.PaymentFee,14:N0}");
            Console.WriteLine($"  ─────────────────────────────");
            Console.WriteLine($"  FINAL TOTAL:   {_session.FinalTotal,14:N0}");
            Console.WriteLine();
            Console.Write("  [Y] Confirm  [N] Change method > ");
            var confirm = Console.ReadLine()?.Trim() ?? string.Empty;

            if (!confirm.Equals("Y", StringComparison.OrdinalIgnoreCase))
                continue;

            // Process payment
            var result = await ProcessPaymentAsync();
            if (result == Result.PaymentSucceeded) return result;
            // On failure the loop continues so user can retry or go back
        }
    }

    private void PrintMethodMenu(PaymentMethod[] methods)
    {
        ConsoleHelper.Clear();
        Console.WriteLine("╔══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  PAYMENT METHOD                                      ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine($"  Cart Total: {_session.Total:N0}");
        Console.WriteLine();
        for (int i = 0; i < methods.Length; i++)
            Console.WriteLine($"  [{i + 1}] {methods[i]}");
        Console.WriteLine("  [B] Back to cart");
        Console.WriteLine();
    }

    private async Task<Result> ProcessPaymentAsync()
    {
        Console.WriteLine("\n  Processing payment...");
        try
        {
            _session.TriggerFsm("PaymentSelected");
            var receipt = await _session.Api.ProcessPaymentAsync(
                _session.CartId,
                _session.SelectedPaymentMethod);

            if (receipt is not null)
            {
                _session.TriggerFsm("PaymentConfirmed");
                _session.LastReceipt = receipt;
                return Result.PaymentSucceeded;
            }
            else
            {
                _session.TriggerFsm("PaymentFailed");
                Console.WriteLine("\n  ✗ Payment was not processed. Please try again.");
                Pause();
                return Result.BackToCart;
            }
        }
        catch (Exception ex)
        {
            _session.TriggerFsm("PaymentFailed");
            Console.WriteLine($"\n  ✗ Payment failed: {ex.Message}");
            Console.WriteLine("\n  [R] Retry  [B] Back to cart");
            Console.Write("  > ");
            var retry = Console.ReadLine()?.Trim() ?? string.Empty;
            if (retry.Equals("B", StringComparison.OrdinalIgnoreCase))
                return Result.BackToCart;
            return Result.BackToCart; // Default: back to cart on error
        }
    }

    private static void Pause()
    {
        if (Console.IsInputRedirected)
        {
            Console.WriteLine("\n  [Redirected] Press Enter to continue...");
            Console.ReadLine();
        }
        else
        {
            Console.WriteLine("\n  Press any key to continue...");
            Console.ReadKey(intercept: true);
        }
    }
}
