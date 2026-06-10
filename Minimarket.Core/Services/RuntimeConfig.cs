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
            var jsonStr = File.ReadAllText(filename);
            var rtConfig = JsonSerializer.Deserialize<RuntimeConfig>(jsonStr);
            return rtConfig;
        }
        catch (JsonException e)
        {
            Trace.TraceError(e.Message);
            return null;
        }
    }
}
