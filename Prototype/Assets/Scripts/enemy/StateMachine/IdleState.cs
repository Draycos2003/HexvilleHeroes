using UnityEngine;

public class IdleState : StateMachine
{
    public ChaseState chaseState;
    public bool inChasingRange;

    enemyAI enemy;

    public override StateMachine RunCurrentState()
    {
        if (inChasingRange)
        {
            return chaseState;

        }
        else
        {
            return this;
        }
    }

}
