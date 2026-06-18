using Minimarket.Core.Services;
using Minimarket.CLI.Screens;

namespace Minimarket.CLI;

/// <summary>
/// The main application coordinator for the Minimarket CLI app.
/// Handles session lifecycle, screen transition flow, and global exception/cancellation handling.
/// </summary>
public class CLIApp
{
    private enum Screen
    {
        Auth,
        Products,
        Cart,
        Payment,
        Receipt
    }

    private readonly ApiClient _api;
    private readonly PricingConfigCache _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="CLIApp"/> class.
    /// </summary>
    public CLIApp()
    {
        _api = new ApiClient();
        _cache = new PricingConfigCache(_api);
    }

    /// <summary>
    /// Asynchronously runs the application, handling startup config loading and the screen loops.
    /// </summary>
    public async Task RunAsync()
    {
        // Set up clean Ctrl+C handling
        Console.CancelKeyPress += (sender, e) =>
        {
            Console.ResetColor();
            Console.WriteLine("\n\n  [System] Interrupted by user. Exiting gracefully...");
            // Let the process exit naturally
        };

        try
        {
            ConsoleHelper.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════╗");
            Console.WriteLine("║  MINIMARKET SYSTEM STARTING...                       ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════╝");
            Console.WriteLine("\n  Fetching latest pricing rules and configurations...");

            await _cache.LoadAsync();

            if (!_cache.IsConnected)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n  ⚠ WARNING: API offline — operating in offline caching mode.");
                Console.ResetColor();
                Console.WriteLine("  Base prices and offline configurations will be used.");
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
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n  ✓ Pricing and system configuration successfully loaded!");
                Console.ResetColor();
                System.Threading.Thread.Sleep(800);
            }

            var session = new CLISession(_api, _cache);
            bool appRunning = true;

            while (appRunning)
            {
                // Start with the authentication flow
                var authScreen = new AuthScreen(session);
                var authenticated = await authScreen.RunAsync();

                if (!authenticated)
                {
                    appRunning = false;
                    break;
                }

                // Logged in — enter the main retail transaction loop
                bool userSessionActive = true;
                Screen currentScreen = Screen.Products;

                while (userSessionActive)
                {
                    switch (currentScreen)
                    {
                        case Screen.Products:
                            var prodScreen = new ProductScreen(session);
                            var prodRes = await prodScreen.RunAsync();
                            if (prodRes == ProductScreen.Result.Exit)
                            {
                                // Logout / exit user session
                                userSessionActive = false;
                                session.AuthenticatedUser = null;
                            }
                            else if (prodRes == ProductScreen.Result.GoToCart)
                            {
                                currentScreen = Screen.Cart;
                            }
                            break;

                        case Screen.Cart:
                            var cartScreen = new CartScreen(session);
                            var cartRes = await cartScreen.RunAsync();
                            if (cartRes == CartScreen.Result.BackToProducts)
                            {
                                currentScreen = Screen.Products;
                            }
                            else if (cartRes == CartScreen.Result.GoToPayment)
                            {
                                currentScreen = Screen.Payment;
                            }
                            else if (cartRes == CartScreen.Result.CancelTransaction)
                            {
                                session.StartNewTransaction();
                                currentScreen = Screen.Products;
                            }
                            break;

                        case Screen.Payment:
                            var paymentScreen = new PaymentScreen(session);
                            var paymentRes = await paymentScreen.RunAsync();
                            if (paymentRes == PaymentScreen.Result.BackToCart)
                            {
                                currentScreen = Screen.Cart;
                            }
                            else if (paymentRes == PaymentScreen.Result.PaymentSucceeded)
                            {
                                currentScreen = Screen.Receipt;
                            }
                            break;

                        case Screen.Receipt:
                            var receiptScreen = new ReceiptScreen(session);
                            var receiptRes = await receiptScreen.RunAsync();
                            if (receiptRes == ReceiptScreen.Result.Exit)
                            {
                                userSessionActive = false;
                                appRunning = false;
                            }
                            else if (receiptRes == ReceiptScreen.Result.NewTransaction)
                            {
                                currentScreen = Screen.Products;
                            }
                            break;
                    }
                }
            }

            ConsoleHelper.Clear();
            Console.WriteLine("\n  Thank you for using Minimarket CLI. Goodbye!\n");
        }
        catch (OperationCanceledException)
        {
            Console.ResetColor();
            Console.WriteLine("\n\n  [System] Operation canceled. Exiting gracefully...");
        }
        catch (Exception ex)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n\n  [Fatal Error] An unhandled exception occurred: {ex.Message}");
            Console.ResetColor();
            if (Console.IsInputRedirected)
            {
                Console.WriteLine("  [Redirected] Press Enter to exit...");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("  Press any key to exit...");
                Console.ReadKey(intercept: true);
            }
        }
    }
}

/// <summary>
/// Entry point container for the CLI application.
/// </summary>
public static class Program
{
    /// <summary>
    /// Application Main entry point.
    /// </summary>
    public static async Task Main()
    {
        var app = new CLIApp();
        await app.RunAsync();
    }
}
