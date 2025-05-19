using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class IdleState : StateMachine
{
    public ChaseState chaseState;
    public bool inChasingRange;
    enemyAI enemy;

    public override StateMachine RunCurrentState()
    {
        if (enemy.inRange == true)
        {
            inChasingRange = true;
            return chaseState;
        }
        else
        {
            return this;
        }
    }
}
