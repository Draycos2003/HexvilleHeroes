using Unity.VisualScripting;
using UnityEngine;

public abstract class AbstractStateBehaviour<StateType> : MonoBehaviour where StateType : System.Enum
{
    [field :SerializeField]

    public StateType State
    {
        get; protected set;
    }

    public delegate void StateChangeEvent(StateType oldState, StateType newState);
    public StateChangeEvent OnStateChange;

    public virtual void ChangeState(StateType newState)
    {
        OnStateChange?.Invoke(State, newState);
        State = newState;
    }
}
