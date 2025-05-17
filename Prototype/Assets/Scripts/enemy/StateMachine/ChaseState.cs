using UnityEngine;

public class ChaseState : StateMachine
{
    public AttackState attackState;
    public bool inAttackRange;

    public override StateMachine RunCurrentState()
    {
        if (inAttackRange)
        {
            return attackState;
        }
        else
        {
            return this;
        }
    }

    
}
