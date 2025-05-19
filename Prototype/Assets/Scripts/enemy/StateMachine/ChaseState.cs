using Unity.VisualScripting;
using UnityEngine;

public class ChaseState : StateMachine
{
    public AttackState attackState;
    
    IdleState idie;
    enemyAI enemy;

    public override StateMachine RunCurrentState()
    {
        if (idie.inChasingRange == true)
        {
            enemy.UpdatePath();
        }

        if (enemy.LOS() == true) // if player enters attacking range begin attacking
        {
            return attackState;
        }
        else
        {
            return this;
        }
    }

    
}
