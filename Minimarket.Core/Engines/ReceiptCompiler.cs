using System;
using System.Text;
using Minimarket.Core.Models;

namespace Minimarket.Core.Engines;

public static class ReceiptCompiler
{
    public const string DefaultTemplate = @"
==================================
        {{store_name}}
==================================
Date: {{date}}
Items:
{{items}}
----------------------------------
Subtotal:       {{subtotal}}
Discount:      -{{discount}}
Tax & Svc:      {{tax}}
Payment Fee:    {{fee}}
----------------------------------
TOTAL:          {{total}}
Paid via:       {{payment_method}}
==================================
   Thank you for shopping with us!
==================================
";

    public static string Compile(string template, Receipt receipt, string storeName = "MINIMARKET CO.")
    {
        var itemsBuilder = new StringBuilder();
        foreach (var item in receipt.Items)
        {
            var lineTotal = item.UnitPrice * item.Quantity - item.DiscountAmount;
            itemsBuilder.AppendLine($"{item.ProductName,-18} {item.Quantity}x {item.UnitPrice,-6:N0} = {lineTotal:N0}");
        }

        var result = template
            .Replace("{{store_name}}", storeName)
            .Replace("{{date}}", receipt.Date.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"))
            .Replace("{{items}}", itemsBuilder.ToString().TrimEnd())
            .Replace("{{subtotal}}", receipt.Subtotal.ToString("N2"))
            .Replace("{{discount}}", receipt.DiscountAmount.ToString("N2"))
            .Replace("{{tax}}", receipt.TaxAmount.ToString("N2"))
            .Replace("{{fee}}", receipt.FeeAmount.ToString("N2"))
            .Replace("{{total}}", receipt.Total.ToString("N2"))
            .Replace("{{payment_method}}", receipt.PaymentMethod.ToString());

        return result;
    }
}
