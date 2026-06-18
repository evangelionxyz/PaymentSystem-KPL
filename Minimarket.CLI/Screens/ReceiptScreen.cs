using Minimarket.Core.Models;

namespace Minimarket.CLI.Screens;

/// <summary>
/// Handles the receipt screen after a successful checkout.
/// Displays a formatted invoice receipt and handles resetting the session for a new purchase.
/// </summary>
public class ReceiptScreen
{
    private readonly CLISession _session;

    /// <summary>
    /// Represents the navigation option after viewing the receipt.
    /// </summary>
    public enum Result { NewTransaction, Exit }

    /// <summary>Initializes the receipt screen with the given session.</summary>
    public ReceiptScreen(CLISession session) => _session = session;

    /// <summary>
    /// Runs the receipt screen display loop.
    /// </summary>
    public Task<Result> RunAsync()
    {
        var receipt = _session.LastReceipt;
        if (receipt is null)
        {
            Console.WriteLine("  Error: No receipt data found.");
            Console.WriteLine("  Press Enter/Key to return...");
            if (Console.IsInputRedirected) Console.ReadLine();
            else Console.ReadKey(intercept: true);
            return Task.FromResult(Result.NewTransaction);
        }

        while (true)
        {
            PrintReceipt(receipt);

            Console.Write("  [N] New transaction  [X] Exit application > ");
            var input = Console.ReadLine()?.Trim() ?? string.Empty;

            if (input.Equals("X", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(Result.Exit);
            }

            if (input.Equals("N", StringComparison.OrdinalIgnoreCase))
            {
                _session.StartNewTransaction();
                return Task.FromResult(Result.NewTransaction);
            }

            Console.WriteLine("  Invalid choice. Press Enter/Key.");
            if (Console.IsInputRedirected) Console.ReadLine();
            else Console.ReadKey(intercept: true);
        }
    }

    private void PrintReceipt(Receipt receipt)
    {
        ConsoleHelper.Clear();
        Console.WriteLine("╔══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  TRANSACTION RECEIPT                                 ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine($"  Receipt ID:   {receipt.ID}");
        Console.WriteLine($"  Date/Time:    {receipt.Date.ToLocalTime():yyyy-MM-dd HH:mm:ss}");
        if (!string.IsNullOrEmpty(receipt.CustomerName))
        {
            Console.WriteLine($"  Customer:     {receipt.CustomerName} (VIP: {(_session.IsVip ? "Yes" : "No")})");
        }
        Console.WriteLine($"  Payment:      {receipt.PaymentMethod}");
        Console.WriteLine($"  Status:       {_session.Fsm.CurrentState}");
        Console.WriteLine();
        Console.WriteLine("  ITEMS PURCHSED:");
        Console.WriteLine($"  {new string('-', 50)}");
        Console.WriteLine($"  {"Product Name",-25} {"Qty",3} {"Unit Price",10} {"Total",9}");
        Console.WriteLine($"  {new string('-', 50)}");

        foreach (var item in receipt.Items)
        {
            var displayName = item.ProductName;
            if (displayName.Length > 25)
                displayName = displayName.Substring(0, 22) + "...";

            Console.WriteLine($"  {displayName,-25} {item.Quantity,3} {item.UnitPrice,10:N0} {item.LineTotal,9:N0}");
        }

        Console.WriteLine($"  {new string('-', 50)}");
        Console.WriteLine($"  {"Subtotal:",-35} {receipt.Subtotal,13:N0}");
        if (receipt.DiscountAmount > 0)
        {
            Console.WriteLine($"  {"Discounts:",-35} -{receipt.DiscountAmount,12:N0}");
        }
        if (receipt.TaxAmount > 0)
        {
            Console.WriteLine($"  {"Tax:",-35} {receipt.TaxAmount,13:N0}");
        }
        if (receipt.FeeAmount > 0)
        {
            Console.WriteLine($"  {"Payment Surcharge:",-35} {receipt.FeeAmount,13:N0}");
        }
        Console.WriteLine($"  {new string('=', 50)}");
        Console.WriteLine($"  {"GRAND TOTAL:",-35} {receipt.Total,13:N0}");
        Console.WriteLine();
    }
}
