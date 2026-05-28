using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Minimarket.Core.Models;

public class Cart : BaseModel
{
    [BsonElement("items")]
    [JsonPropertyName("items")]
    public List<Product> Items { get; set; } = new(); 

    [BsonElement("totalItems")]
    [JsonPropertyName("totalItems")]
    public int TotalItems { get => Items.Count; }

    [BsonElement("totalPrice")]
    [JsonPropertyName("totalPrice")]
    public int TotalPrice { get; set; } = 0;
}
