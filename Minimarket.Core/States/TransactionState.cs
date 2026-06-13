namespace Minimarket.Core.States;

public enum TransactionState : uint
{
    Idle = 0,
    AwaitingPayment,
    ProcessingPayment,
    Completed,
    Cancelled,
}
