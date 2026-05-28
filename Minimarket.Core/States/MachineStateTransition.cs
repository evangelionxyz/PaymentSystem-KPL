using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Minimarket.Core.States;

/// <summary>
/// Represents one allowed transition in the transaction FSM table.
/// Loaded from the `machineStates` MongoDB collection.
/// </summary>
public class MachineStateTransition
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonPropertyName("id")]
    public string? ID { get; set; } = null;

    [BsonElement("from")]
    [JsonPropertyName("from")]
    public TransactionState From { get; set; }

    [BsonElement("to")]
    [JsonPropertyName("to")]
    public TransactionState To { get; set; }

    [BsonElement("trigger")]
    [JsonPropertyName("trigger")]
    public string Trigger { get; set; } = string.Empty;
}
