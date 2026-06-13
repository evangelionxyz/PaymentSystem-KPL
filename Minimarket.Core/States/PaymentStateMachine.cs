using Minimarket.Core.States;

namespace Minimarket.Core.States;

/// <summary>
/// Concrete FSM pre-configured with the default payment flow transitions.
/// Extends TransactionFSM which replaced the old StateMachine stub.
/// </summary>
public class PaymentStateMachine : TransactionFSM
{
    public PaymentStateMachine(IEnumerable<MachineStateTransition> transitions)
        : base(transitions) { }
}
