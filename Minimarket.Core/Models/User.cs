using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Minimarket.Core.Models;

public class User : BaseModel
{
    [BsonElement("username")]
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [BsonElement("password")]
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}
