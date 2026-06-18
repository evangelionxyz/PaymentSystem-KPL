namespace Minimarket.CLI;

/// <summary>
/// Provides safe wrapper methods for Console I/O operations, particularly Console.Clear,
/// to avoid throwing exceptions when output is redirected.
/// </summary>
public static class ConsoleHelper
{
    /// <summary>
    /// Safely clears the console if output is not redirected.
    /// </summary>
    public static void Clear()
    {
        if (!Console.IsOutputRedirected)
        {
            try
            {
                Console.Clear();
            }
            catch
            {
                // Fallback: ignore exceptions on clear if handles are invalid
            }
        }
    }
}
