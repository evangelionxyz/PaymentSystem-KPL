using MongoDB.Bson;
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
    public decimal Price { get; set; } = 0;

    [BsonElement("categoryId")]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonPropertyName("categoryId")]
    public string? CategoryId { get; set; }

    [BsonElement("categoryName")]
    [JsonPropertyName("categoryName")]
    public string? CategoryName { get; set; }
}
