using System;

namespace Minimarket.API;

public class Settings
{
    public string? ConnectionString { get; set; } = null;
    public string? DatabaseName { get; set; } = null;
    public string? CustomerCollectionName { get; set; } = null;
    public string? ProductCollectionName { get; set; } = null;
    public string? PaymentCollectionName { get; set; } = null;
    public string? CartCollectionName { get; set; } = null;
    public string? ReceiptCollectionName { get; set; } = null;
    public string? CategoryCollectionName { get; set; } = null;
    public string? PricingRuleCollectionName { get; set; } = null;
    public string? MachineStateCollectionName { get; set; } = null;
}

