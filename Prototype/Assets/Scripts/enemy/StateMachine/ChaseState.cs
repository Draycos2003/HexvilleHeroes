using Unity.VisualScripting;
using UnityEngine;

public class ChaseState : StateMachine
{
    public AttackState attackState;
    public bool canAttack;
    
    IdleState idie;
    enemyAI enemy;

    private void Start()
    {
        idie = GetComponent<IdleState>();
        enemy = GetComponent<enemyAI>(); 
    }

    public override StateMachine RunCurrentState()
    {
        if (idie.inChasingRange == true)
        {
            enemy.UpdatePath();
            Debug.Log("Updating path to find player");
        }

        if (enemy.LOS() == true) // if player enters attacking range begin attacking
        {
            canAttack = true;
            return attackState;
        }
        else
        {
            return this;
        }
    }

    
}
