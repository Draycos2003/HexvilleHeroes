using UnityEngine;

public class IdleState : StateMachine
{
    public ChaseState chaseState;
    public bool inChasingRange;

    public override StateMachine RunCurrentState()
    {
        if (inChasingRange)
        {
            inChasingRange = true;
            return chaseState;
        }
        else
        {
            Debug.Log("Still idle");
            return this;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == ("Player"))
        {
            inChasingRange = true;
        }

    }
}
