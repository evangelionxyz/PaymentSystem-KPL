using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Minimarket.Core.Models;

public class PricingRule : BaseModel
{
    /// <summary>"DiscountPercentage" | "BuyXGetY" | "TaxRate" | "PaymentFee"</summary>
    [BsonElement("ruleType")]
    [JsonPropertyName("ruleType")]
    public string RuleType { get; set; } = string.Empty;

    /// <summary>Applies to a specific product when set.</summary>
    [BsonElement("productId")]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonPropertyName("productId")]
    public string? ProductId { get; set; }

    /// <summary>Applies to all products in this category when set.</summary>
    [BsonElement("categoryId")]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonPropertyName("categoryId")]
    public string? CategoryId { get; set; }

    /// <summary>Free-form condition string, e.g. "X=2,Y=1" for BuyXGetY.</summary>
    [BsonElement("condition")]
    [JsonPropertyName("condition")]
    public string? Condition { get; set; }

    /// <summary>Numeric value — discount %, tax rate, fee %, etc.</summary>
    [BsonElement("value")]
    [JsonPropertyName("value")]
    public decimal Value { get; set; }

    /// <summary>Lower number = applied first.</summary>
    [BsonElement("priority")]
    [JsonPropertyName("priority")]
    public int Priority { get; set; } = 0;
}
