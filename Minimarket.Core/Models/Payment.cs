using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Minimarket.Core.Models;

public enum PaymentMethod : uint
{
    Cash = 0,
    EWallet,
    BankTransfer,
    QRIS,
    CreditCard
}

public class Payment : BaseModel
{
    [BsonElement("customer")]
    [JsonPropertyName("customer")]
    public Customer? Customer { get; set; }

    [BsonElement("date")]
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [BsonElement("paymentMethod")]
    [BsonRepresentation(BsonType.Int32)]
    [JsonPropertyName("paymentMethod")]
    public PaymentMethod PaymentMethod { get; set; }
}
