using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket.Core.States;

public enum PaymentState : uint
{
    Idle,
    Scanning,
    AwaitingPayment,
    ProcessingPayment,
    Completed,
    Cancelled,
}

public class PaymentStateMachine : StateMachine
{
}
