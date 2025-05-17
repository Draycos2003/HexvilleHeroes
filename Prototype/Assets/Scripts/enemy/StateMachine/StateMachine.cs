using UnityEngine;

// All state machine class derive from this class

public abstract class StateMachine : MonoBehaviour
{
    public abstract StateMachine RunCurrentState();
}
