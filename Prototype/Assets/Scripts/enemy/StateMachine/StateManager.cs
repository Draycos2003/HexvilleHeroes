using UnityEngine;

public class StateManager : MonoBehaviour
{

    public StateMachine currentState;

    // Update is called once per frame
    void Update()
    {
        RunStateMachine();
    }
    private void RunStateMachine()
    {
        // The question mark just checks if the next state is null. If not it will run the current state
        StateMachine nextState = currentState?.RunCurrentState();

        if (nextState != null)
        {
            SwitchToNextState(nextState);
        }
    }
    private void SwitchToNextState(StateMachine nextState)
    {
        // Switches the current state to the next state
        currentState = nextState;
    }
}
