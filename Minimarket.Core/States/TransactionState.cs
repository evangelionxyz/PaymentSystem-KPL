namespace Minimarket.Core.States;

public enum TransactionState : uint
{
    Idle = 0,
    Scanning,
    AwaitingPayment,
    ProcessingPayment,
    Completed,
    Cancelled,
}
