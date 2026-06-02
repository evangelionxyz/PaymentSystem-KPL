using System;
using System.Collections.Generic;

namespace Minimarket.Core.Models;

public class PaymentConfig
{
    public decimal RateFee { get; set; } = 0m;
    public decimal FlatFee { get; set; } = 0m;
}

public interface IPaymentMethod<TConfig> where TConfig : PaymentConfig
{
    decimal CalculateSurcharge(decimal subtotal, TConfig config);
}

public class CashPaymentConfig : PaymentConfig { }
public class EWalletPaymentConfig : PaymentConfig { }
public class BankTransferPaymentConfig : PaymentConfig { }
public class QrisPaymentConfig : PaymentConfig { }
public class CreditCardPaymentConfig : PaymentConfig
{
    public Dictionary<decimal, decimal> SurchargeRules { get; set; } = new();
}

public class CashPaymentMethod : IPaymentMethod<CashPaymentConfig>
{
    public decimal CalculateSurcharge(decimal subtotal, CashPaymentConfig config)
        => config.FlatFee + (subtotal * config.RateFee);
}

public class EWalletPaymentMethod : IPaymentMethod<EWalletPaymentConfig>
{
    public decimal CalculateSurcharge(decimal subtotal, EWalletPaymentConfig config)
        => config.FlatFee + (subtotal * config.RateFee);
}

public class BankTransferPaymentMethod : IPaymentMethod<BankTransferPaymentConfig>
{
    public decimal CalculateSurcharge(decimal subtotal, BankTransferPaymentConfig config)
        => config.FlatFee + (subtotal * config.RateFee);
}

public class QrisPaymentMethod : IPaymentMethod<QrisPaymentConfig>
{
    public decimal CalculateSurcharge(decimal subtotal, QrisPaymentConfig config)
        => config.FlatFee + (subtotal * config.RateFee);
}

public class CreditCardPaymentMethod : IPaymentMethod<CreditCardPaymentConfig>
{
    public decimal CalculateSurcharge(decimal subtotal, CreditCardPaymentConfig config)
    {
        decimal rate = config.RateFee;
        foreach (var rule in config.SurchargeRules)
        {
            if (subtotal >= rule.Key)
            {
                rate = rule.Value;
            }
        }
        return config.FlatFee + (subtotal * rate);
    }
}

public static class PaymentCalculator
{
    public static decimal Calculate(PaymentMethod method, decimal subtotal, decimal rateFromConfig)
    {
        // Wrapper for dynamic calculation based on payment type
        switch (method)
        {
            case PaymentMethod.Cash:
                return new CashPaymentMethod().CalculateSurcharge(subtotal, new CashPaymentConfig { RateFee = rateFromConfig });
            case PaymentMethod.EWallet:
                return new EWalletPaymentMethod().CalculateSurcharge(subtotal, new EWalletPaymentConfig { RateFee = rateFromConfig });
            case PaymentMethod.BankTransfer:
                return new BankTransferPaymentMethod().CalculateSurcharge(subtotal, new BankTransferPaymentConfig { RateFee = rateFromConfig });
            case PaymentMethod.QRIS:
                return new QrisPaymentMethod().CalculateSurcharge(subtotal, new QrisPaymentConfig { RateFee = rateFromConfig });
            case PaymentMethod.CreditCard:
                var ccConfig = new CreditCardPaymentConfig
                {
                    RateFee = rateFromConfig,
                    SurchargeRules = new Dictionary<decimal, decimal>
                    {
                        { 0m, rateFromConfig },
                        { 100000m, rateFromConfig * 0.9m }, // 10% discount on rate for purchases > 100k
                        { 500000m, rateFromConfig * 0.8m }  // 20% discount on rate for purchases > 500k
                    }
                };
                return new CreditCardPaymentMethod().CalculateSurcharge(subtotal, ccConfig);
            default:
                return 0m;
        }
    }
}
