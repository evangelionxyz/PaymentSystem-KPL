using Minimarket.Core.Services;
using Minimarket.Core.Models;
using NUnit.Framework;

namespace Minimarket.API.Test;

[TestFixture]
public class PaymentApiTests
{
    private ApiClient _api = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _api = new ApiClient();
    }

    [Test]
    public async Task ProcessPayemnt_GetByID_ReturnsReceipt()
    {
        var cart = await _api.GetCartById("6a238315421eab35feb89289");
        Assert.That(cart, Is.Not.Null, "Invalid cart ID");

        var receipt = await _api.ProcessPaymentAsync(cart!.ID!, PaymentMethod.Cash, cart.CustomerId);

        Assert.That(receipt, Is.Not.Null);
        Assert.That(receipt!.PaymentMethod, Is.EqualTo(PaymentMethod.Cash));
        Assert.That(receipt.Total, Is.GreaterThan(0m));
    }

    [Test]
    public async Task ProcessPayment_ForPendingCart_ReturnsReceipt()
    {
        var pendingCarts = await _api.GetPendingCartsAsync();

        Assert.That(pendingCarts, Is.Not.Empty, "No pending carts were found in the API.");

        var cart = pendingCarts.First();
        var receipt = await _api.ProcessPaymentAsync(cart.ID!, PaymentMethod.Cash, cart.CustomerId);

        Assert.That(receipt, Is.Not.Null);
        Assert.That(receipt!.PaymentMethod, Is.EqualTo(PaymentMethod.Cash));
        Assert.That(receipt.Total, Is.GreaterThan(0m));
    }
}