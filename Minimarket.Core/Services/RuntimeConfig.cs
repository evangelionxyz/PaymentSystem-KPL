using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Minimarket.Core.Services;

public class RuntimeConfig
{
    [JsonPropertyName("baseUrl")]
    public string BaseUrl { get; set; } = string.Empty;

    public static RuntimeConfig? Load(string filename)
    {
        try
        {
            var path = filename;
            if (!File.Exists(path))
            {
                path = Path.Combine(AppContext.BaseDirectory, filename);
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Could not find configuration file: {filename}");
            }

            var jsonStr = File.ReadAllText(path);
            var rtConfig = JsonSerializer.Deserialize<RuntimeConfig>(jsonStr);
            return rtConfig;
        }
        catch (Exception e)
        {
            Trace.TraceError(e.Message);
            throw; // Re-throw so callers are aware of setup failure
        }
    }
}
