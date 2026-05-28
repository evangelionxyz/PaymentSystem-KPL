using System.Text.Json.Serialization;

namespace Core.Models
{
    public class BaseModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
    }
}
