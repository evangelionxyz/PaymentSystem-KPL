using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Minimarket.Core.Models;

public class Product : BaseModel
{
    [BsonElement("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("description")]
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("price")]
    [JsonPropertyName("price")]
    public int Price { get; set; } = 0;
}
