using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Minimarket.Core.Models;

public class Receipt : BaseModel
{
    [BsonElement("customer")]
    [JsonPropertyName("customer")]
    public Customer? Customer { get; set; }

    [BsonElement("payment")]
    [JsonPropertyName("payment")]
    public Payment? Payment { get; set; }
}
