using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Minimarket.Customer.Services;

public class RuntimeConfig
{
    [JsonPropertyName("baseUrl")]
    public string? BaseUrl { get; private set; }

    public static RuntimeConfig? Load(string filename)
    {
        try
        {
            var rtConfig = JsonSerializer.Deserialize<RuntimeConfig>(filename);
            return rtConfig;
        }
        catch (JsonException e)
        {
            Trace.TraceError(e.Message);
            return null;
        }
    }
}
