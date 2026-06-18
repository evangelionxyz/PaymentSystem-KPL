using Minimarket.Core.Models;
using Minimarket.Core.Services;

namespace Minimarket.CLI.Screens;

/// <summary>
/// Handles the authentication screen: login and registration.
/// Returns only when the user is successfully authenticated or chooses to exit.
/// </summary>
public class AuthScreen
{
    private readonly CLISession _session;

    /// <summary>Initializes the auth screen with the given session.</summary>
    public AuthScreen(CLISession session) => _session = session;

    /// <summary>
    /// Runs the authentication loop. Returns <c>true</c> if the user authenticated,
    /// <c>false</c> if they chose to exit.
    /// </summary>
    public async Task<bool> RunAsync()
    {
        while (true)
        {
            ConsoleHelper.Clear();
            Console.WriteLine("╔══════════════════════════════════╗");
            Console.WriteLine("║     MINIMARKET — SIGN IN         ║");
            Console.WriteLine("╚══════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("  [1] Login");
            Console.WriteLine("  [2] Register");
            Console.WriteLine("  [3] Exit");
            Console.WriteLine();
            Console.Write("Select > ");
            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    if (await LoginAsync()) return true;
                    break;
                case "2":
                    await RegisterAsync();
                    break;
                case "3":
                    return false;
                default:
                    Console.WriteLine("Invalid choice. Press Enter/Key to try again.");
                    if (Console.IsInputRedirected) Console.ReadLine();
                    else Console.ReadKey(intercept: true);
                    break;
            }
        }
    }

    private async Task<bool> LoginAsync()
    {
        ConsoleHelper.Clear();
        Console.WriteLine("── LOGIN ──────────────────────────────");
        Console.Write("Username: ");
        var username = Console.ReadLine()?.Trim() ?? string.Empty;

        Console.Write("Password: ");
        var password = ReadPassword();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            WriteError("Username and password are required.");
            Pause();
            return false;
        }

        try
        {
            var user = await _session.Api.LoginAsync(username, password);
            if (user is not null)
            {
                _session.AuthenticatedUser = user;
                _session.IsVip = username.EndsWith("vip", StringComparison.OrdinalIgnoreCase);
                WriteSuccess($"Welcome, {username}!");
                Pause();
                return true;
            }
            else
            {
                WriteError("Invalid credentials.");
                Pause();
                return false;
            }
        }
        catch (Exception ex)
        {
            WriteError($"Login failed: {ex.Message}");
            Pause();
            return false;
        }
    }

    private async Task RegisterAsync()
    {
        ConsoleHelper.Clear();
        Console.WriteLine("── REGISTER ───────────────────────────");
        Console.Write("Username: ");
        var username = Console.ReadLine()?.Trim() ?? string.Empty;

        Console.Write("Password: ");
        var password = ReadPassword();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            WriteError("Username and password are required.");
            Pause();
            return;
        }

        try
        {
            var newUser = new User { Username = username, Password = password, Role = "Customer" };
            var result = await _session.Api.RegisterAsync(newUser);
            if (result is not null)
                WriteSuccess("Account created! You can now log in.");
            else
                WriteError("Registration failed. Username may already be taken.");
        }
        catch (Exception ex)
        {
            WriteError($"Registration error: {ex.Message}");
        }
        Pause();
    }

    private static string ReadPassword()
    {
        if (Console.IsInputRedirected)
        {
            return Console.ReadLine()?.Trim() ?? string.Empty;
        }

        var sb = new System.Text.StringBuilder();
        while (true)
        {
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Enter) { Console.WriteLine(); break; }
            if (key.Key == ConsoleKey.Backspace && sb.Length > 0) { sb.Remove(sb.Length - 1, 1); Console.Write("\b \b"); }
            else if (key.Key != ConsoleKey.Backspace) { sb.Append(key.KeyChar); Console.Write("*"); }
        }
        return sb.ToString();
    }

    private static void WriteError(string msg) => Console.WriteLine($"\n  ✗ {msg}");
    private static void WriteSuccess(string msg) => Console.WriteLine($"\n  ✓ {msg}");
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
