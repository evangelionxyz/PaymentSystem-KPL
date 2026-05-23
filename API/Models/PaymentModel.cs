using System.Text.Json.Serialization;

namespace API.Models
{
    public class PaymentModel : BaseModel
    {
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;
    }
}
