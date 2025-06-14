public class StateMachine<T>
{
    private IState<T> currentState;
    private T owner;

    public StateMachine(T owner) => this.owner = owner;

    public void ChangeState(IState<T> newState)
    {
        if (currentState != null)
            currentState.Exit(owner);
        currentState = newState;
        if (currentState != null)
            currentState.Enter(owner);
    }

    public void Update()
    {
        if (currentState != null)
            currentState.Update(owner);
    }
}
