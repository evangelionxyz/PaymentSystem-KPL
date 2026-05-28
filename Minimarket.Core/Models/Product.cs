using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Minimarket.Core.Models;

public class Product : BaseModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public int Price { get; set; } = 0;
}
