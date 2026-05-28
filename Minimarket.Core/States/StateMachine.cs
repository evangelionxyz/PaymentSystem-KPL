using Minimarket.Core.States;

namespace Minimarket.Core.States;

/// <summary>
/// Table-driven finite-state machine for POS transaction flow.
/// Transitions are loaded from the `machineStates` MongoDB collection and
/// passed in via the constructor — satisfying the automata requirement.
/// </summary>
public class TransactionFSM
{
    private readonly IReadOnlyList<MachineStateTransition> _transitions;

    public TransactionState CurrentState { get; private set; } = TransactionState.Idle;

    public TransactionFSM(IEnumerable<MachineStateTransition> transitions)
    {
        _transitions = transitions.ToList().AsReadOnly();
    }

    /// <summary>
    /// Applies <paramref name="trigger"/> to the current state.
    /// Throws <see cref="InvalidOperationException"/> if the transition is not in the table.
    /// </summary>
    public void Trigger(string trigger)
    {
        var match = _transitions.FirstOrDefault(
            t => t.From == CurrentState &&
                 string.Equals(t.Trigger, trigger, StringComparison.OrdinalIgnoreCase));

        if (match is null)
            throw new InvalidOperationException(
                $"TransactionFSM: trigger '{trigger}' is not allowed from state '{CurrentState}'.");

        CurrentState = match.To;
    }

    /// <summary>Returns all transitions available from the current state.</summary>
    public IEnumerable<MachineStateTransition> AvailableTransitions() =>
        _transitions.Where(t => t.From == CurrentState);
}
