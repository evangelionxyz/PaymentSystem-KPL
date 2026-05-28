using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Minimarket.Core.Models;

public class Customer : BaseModel
{
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;
}
