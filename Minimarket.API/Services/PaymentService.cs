using Minimarket.Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Minimarket.API.Services;

public class PaymentService(
    IOptions<Settings> settings,
    IOptions<PaymentFeeSettings> feeSettings,
    IMongoClient client,
    CartService cartService,
    ReceiptService receiptService,
    CustomerService customerService,
    UserService userService)
{
    private readonly IMongoCollection<Payment> _payments = client.GetDatabase(settings.Value.DatabaseName).GetCollection<Payment>(settings.Value.PaymentCollectionName);
    public async Task<List<Payment>> GetAsync() => await _payments.Find(_ => true).ToListAsync();

    public async Task<Payment?> GetAsync(string id) => await _payments.Find(x => x.ID == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Payment payment) => await _payments.InsertOneAsync(payment);

    public async Task<Receipt> ProcessAsync(string cartId, PaymentMethod method, string? customerId)
    {
        var cart = await cartService.CheckoutAsync(cartId);

        var resolvedCustomerId = customerId ?? cart.CustomerId;
        var customer = await ResolveCustomerAsync(resolvedCustomerId);

        // Apply payment fee from runtime config and payment plugins.
        var feeKey = method.ToString();
        var feeRate = feeSettings.Value.Fees.TryGetValue(feeKey, out var rate) ? rate : 0m;
        var feeAmount = Math.Round(PaymentCalculator.Calculate(method, cart.Total, feeRate), 2);
        var finalTotal = cart.Total + feeAmount;

        // Persist payment record.
        var payment = new Payment
        {
            Date          = DateTime.UtcNow,
            PaymentMethod = method,
            Customer      = customer,
        };
        await _payments.InsertOneAsync(payment);

        cart.IsPaid = true;
        if (!string.IsNullOrWhiteSpace(resolvedCustomerId) && cart.CustomerId != resolvedCustomerId)
        {
            cart.CustomerId = resolvedCustomerId;
        }
        await cartService.UpdateAsync(cartId, cart);

        // Create receipt.
        var receipt = new Receipt
        {
            CustomerId     = resolvedCustomerId,
            CustomerName   = BuildCustomerName(customer),
            Items          = cart.Items,
            Subtotal       = cart.Subtotal,
            DiscountAmount = cart.DiscountAmount,
            TaxAmount      = cart.TaxAmount,
            FeeAmount      = feeAmount,
            Total          = finalTotal,
            PaymentMethod  = method,
            Date           = payment.Date,
        };
        await receiptService.CreateAsync(receipt, customer);
        return receipt;
    }

    private async Task<Customer?> ResolveCustomerAsync(string? customerId)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            return null;

        var customer = await customerService.GetAsync(customerId);
        if (customer is not null)
            return customer;

        var user = await userService.GetAsync(customerId);
        if (user is null)
            return null;

        return new Customer
        {
            ID = user.ID,
            FirstName = user.Username,
            LastName = string.Empty,
            Phone = string.Empty,
            IsVip = user.Username.EndsWith("vip", StringComparison.OrdinalIgnoreCase),
        };
    }

    private static string? BuildCustomerName(Customer? customer)
    {
        if (customer is null)
            return null;

        var fullName = $"{customer.FirstName} {customer.LastName}".Trim();
        return string.IsNullOrWhiteSpace(fullName) ? null : fullName;
    }
}
