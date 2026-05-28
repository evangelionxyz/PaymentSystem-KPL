using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Minimarket.Core.Models;

public class CartItem
{
    [BsonElement("productId")]
    [JsonPropertyName("productId")]
    public string ProductId { get; set; } = string.Empty;

    [BsonElement("productName")]
    [JsonPropertyName("productName")]
    public string ProductName { get; set; } = string.Empty;

    [BsonElement("categoryId")]
    [JsonPropertyName("categoryId")]
    public string CategoryId { get; set; } = string.Empty;

    [BsonElement("unitPrice")]
    [JsonPropertyName("unitPrice")]
    public decimal UnitPrice { get; set; }

    [BsonElement("quantity")]
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; } = 1;

    [BsonElement("discountAmount")]
    [JsonPropertyName("discountAmount")]
    public decimal DiscountAmount { get; set; } = 0;

    [BsonIgnore]
    [JsonPropertyName("lineTotal")]
    public decimal LineTotal => (UnitPrice * Quantity) - DiscountAmount;
}
