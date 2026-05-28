using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Minimarket.Core.Models;

public class Receipt : BaseModel
{
    [BsonElement("customerId")]
    [JsonPropertyName("customerId")]
    public string? CustomerId { get; set; }

    [BsonElement("customerName")]
    [JsonPropertyName("customerName")]
    public string? CustomerName { get; set; }

    [BsonElement("items")]
    [JsonPropertyName("items")]
    public List<CartItem> Items { get; set; } = new();

    [BsonElement("subtotal")]
    [JsonPropertyName("subtotal")]
    public decimal Subtotal { get; set; }

    [BsonElement("discountAmount")]
    [JsonPropertyName("discountAmount")]
    public decimal DiscountAmount { get; set; }

    [BsonElement("taxAmount")]
    [JsonPropertyName("taxAmount")]
    public decimal TaxAmount { get; set; }

    [BsonElement("feeAmount")]
    [JsonPropertyName("feeAmount")]
    public decimal FeeAmount { get; set; }

    [BsonElement("total")]
    [JsonPropertyName("total")]
    public decimal Total { get; set; }

    [BsonElement("paymentMethod")]
    [BsonRepresentation(BsonType.Int32)]
    [JsonPropertyName("paymentMethod")]
    public PaymentMethod PaymentMethod { get; set; }

    [BsonElement("date")]
    [JsonPropertyName("date")]
    public DateTime Date { get; set; } = DateTime.UtcNow;
}
