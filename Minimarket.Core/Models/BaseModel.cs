using System.Text.Json.Serialization;
using System;

namespace Minimarket.Core.Models;

public class BaseModel
{
    [JsonPropertyName("id")]
    public string ID { get; set; } = Guid.NewGuid().ToString();
}
