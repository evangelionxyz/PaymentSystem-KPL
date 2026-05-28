using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Minimarket.Core.Models;

public class Receipt : BaseModel
{
    [JsonPropertyName("customer")]
    public Customer Customer { get; set; }

    [JsonPropertyName("payment")]
    public Payment Payment { get; set; }
}
