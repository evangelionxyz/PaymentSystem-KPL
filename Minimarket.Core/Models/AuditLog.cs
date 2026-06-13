using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Text.Json.Serialization;

namespace Minimarket.Core.Models;

public class AuditLog : BaseModel
{
    [BsonElement("transactionId")]
    [JsonPropertyName("transactionId")]
    public string TransactionId { get; set; } = string.Empty;

    [BsonElement("timestamp")]
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [BsonElement("fromState")]
    [JsonPropertyName("fromState")]
    public string FromState { get; set; } = string.Empty;

    [BsonElement("toState")]
    [JsonPropertyName("toState")]
    public string ToState { get; set; } = string.Empty;

    [BsonElement("trigger")]
    [JsonPropertyName("trigger")]
    public string Trigger { get; set; } = string.Empty;
}
