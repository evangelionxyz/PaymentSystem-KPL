using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Minimarket.Core.Models;

public class Cart : BaseModel
{
    [BsonElement("customerId")]
    [JsonPropertyName("customerId")]
    public string? CustomerId { get; set; }

    [BsonElement("items")]
    [JsonPropertyName("items")]
    public List<CartItem> Items { get; set; } = new();

    [BsonIgnore]
    [JsonPropertyName("totalItems")]
    public int TotalItems => Items.Sum(i => i.Quantity);

    [BsonElement("subtotal")]
    [JsonPropertyName("subtotal")]
    public decimal Subtotal { get; set; } = 0;

    [BsonElement("discountAmount")]
    [JsonPropertyName("discountAmount")]
    public decimal DiscountAmount { get; set; } = 0;

    [BsonElement("taxAmount")]
    [JsonPropertyName("taxAmount")]
    public decimal TaxAmount { get; set; } = 0;

    [BsonElement("total")]
    [JsonPropertyName("total")]
    public decimal Total { get; set; } = 0;

    [BsonElement("isCheckedOut")]
    [JsonPropertyName("isCheckedOut")]
    public bool IsCheckedOut { get; set; } = false;

    [BsonElement("isPaid")]
    [JsonPropertyName("isPaid")]
    public bool IsPaid { get; set; } = false;
}
