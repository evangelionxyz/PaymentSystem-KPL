using Minimarket.Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Minimarket.API.Services;

public class PaymentService(
    IOptions<Settings> settings,
    IOptions<PaymentFeeSettings> feeSettings,
    IMongoClient client,
    CartService cartService,
    ReceiptService receiptService)
{
    private readonly IMongoCollection<Payment> _payments =
        client.GetDatabase(settings.Value.DatabaseName)
              .GetCollection<Payment>(settings.Value.PaymentCollectionName);

    // ── Basic CRUD ─────────────────────────────────────────────────────────────

    public async Task<List<Payment>> GetAsync() =>
        await _payments.Find(_ => true).ToListAsync();

    public async Task<Payment?> GetAsync(string id) =>
        await _payments.Find(x => x.ID == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Payment payment) =>
        await _payments.InsertOneAsync(payment);

    // ── Business Logic ─────────────────────────────────────────────────────────

    /// <summary>
    /// Processes payment for a checked-out cart:
    ///  1. Loads the cart (must be checked out).
    ///  2. Applies the payment method fee from PaymentFeeSettings.
    ///  3. Creates a Payment record and a Receipt in MongoDB.
    ///  4. Returns the completed Receipt.
    /// </summary>
    public async Task<Receipt> ProcessAsync(string cartId, PaymentMethod method, string? customerId)
    {
        var cart = await cartService.CheckoutAsync(cartId);

        // Apply payment fee from runtime config.
        var feeKey = method.ToString();
        var feeRate = feeSettings.Value.Fees.TryGetValue(feeKey, out var rate) ? rate : 0m;
        var feeAmount = Math.Round(cart.Total * feeRate, 2);
        var finalTotal = cart.Total + feeAmount;

        // Persist payment record.
        var payment = new Payment
        {
            Date          = DateTime.UtcNow,
            PaymentMethod = method,
            Customer      = customerId is not null ? new Customer { ID = customerId } : null,
        };
        await _payments.InsertOneAsync(payment);

        // Create receipt.
        var receipt = new Receipt
        {
            CustomerId     = customerId,
            Items          = cart.Items,
            Subtotal       = cart.Subtotal,
            DiscountAmount = cart.DiscountAmount,
            TaxAmount      = cart.TaxAmount,
            FeeAmount      = feeAmount,
            Total          = finalTotal,
            PaymentMethod  = method,
            Date           = payment.Date,
        };
        await receiptService.CreateAsync(receipt);
        return receipt;
    }
}
