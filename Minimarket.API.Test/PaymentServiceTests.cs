using Minimarket.Core.Models;
using NUnit.Framework;
namespace Minimarket.API.Test;

public class PaymentServiceTests
{
    [Test]
    public void PaymentMethod_QRIS_ShouldHaveValue3()
    {
        Assert.That((uint)PaymentMethod.QRIS, Is.EqualTo(3));
    }

    [Test]
    public void Receipt_ShouldStorePaymentMethod()
    {
        var receipt = new Receipt
        {
            CustomerId = "cust001",
            PaymentMethod = PaymentMethod.CreditCard,
            Total = 100000
        };

        Assert.That(receipt.PaymentMethod, Is.EqualTo(PaymentMethod.CreditCard));
        Assert.That(receipt.Total, Is.EqualTo(100000));
    }

    [Test]
    public void Cart_DefaultState_ShouldBeUnpaid()
    {
        var cart = new Cart();

        Assert.That(cart.IsPaid, Is.False);
        Assert.That(cart.IsCheckedOut, Is.False);
    }

    [Test]
    public void CartItem_LineTotal_ShouldBeCalculatedCorrectly()
    {
        var item = new CartItem
        {
            ProductId = "P001",
            ProductName = "Indomie",
            UnitPrice = 3500,
            Quantity = 2,
            DiscountAmount = 500
        };

        Assert.That(item.LineTotal, Is.EqualTo(6500));
    }
}