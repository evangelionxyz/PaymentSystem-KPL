using System.Text.Json.Serialization;

namespace API.Models
{
    public class BaseModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
    }
}
