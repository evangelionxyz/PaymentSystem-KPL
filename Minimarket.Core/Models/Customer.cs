using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Minimarket.Core.Models;

public class Customer : BaseModel
{
    [BsonElement("firstName")]
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [BsonElement("lastName")]
    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    [BsonElement("phone")]
    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;

    [BsonElement("isVip")]
    [JsonPropertyName("isVip")]
    public bool IsVip { get; set; } = false;
}
