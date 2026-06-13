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
            throw new InvalidOperationException($"TransactionFSM: trigger '{trigger}' is not allowed from state '{CurrentState}'.");

        CurrentState = match.To;
    }

    /// <summary>Returns all transitions available from the current state.</summary>
    public IEnumerable<MachineStateTransition> AvailableTransitions() =>
        _transitions.Where(t => t.From == CurrentState);
}

public class StateMachine<TState, TInput>
{
    private readonly Dictionary<(TState, TInput), TState> _rules;

    public StateMachine(Dictionary<(TState, TInput), TState> rules)
    {
        _rules = rules ?? new Dictionary<(TState, TInput), TState>();
    }

    public TState MoveNext(TState current, TInput input)
    {
        if (_rules.TryGetValue((current, input), out var nextState))
        {
            return nextState;
        }
        throw new InvalidOperationException($"Invalid transition from state '{current}' with input '{input}'.");
    }

    public bool CanTransition(TState current, TInput input)
    {
        return _rules.ContainsKey((current, input));
    }
}
