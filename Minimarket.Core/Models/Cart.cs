using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Minimarket.Core.Models;

public class Cart : BaseModel
{
    [JsonPropertyName("items")]
    public List<Product> Items { get; set; } = new(); 

    [JsonPropertyName("totalItems")]
    public int TotalItems { get => Items.Count; }

    [JsonPropertyName("totalPrice")]
    public int TotalPrice { get; set; } = 0;
}
