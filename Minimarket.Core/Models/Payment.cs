using System.Text.Json.Serialization;

namespace Minimarket.Core.Models;

public enum PaymentMethod : uint
{
    Cash = 0,
    EWallet,
    BankTransfer,
    QRIS,
    CreditCard
}

public class Payment : BaseModel
{
    [JsonPropertyName("customer")]
    public Customer Customer { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("paymentMethod")]
    public PaymentMethod PaymentMethod { get; set; }
}
