using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class AttackState : StateMachine
{
    enemyAI enemy;
    public ChaseState chaseState;

    private void Start()
    {
        chaseState = GetComponent<ChaseState>();
        enemy = GetComponent<enemyAI>();
    }

    public override StateMachine RunCurrentState()
    {
       if(chaseState.canAttack == true)
       {
          enemy.shoot();

       }
       else
       {
            return chaseState;
       }
       return this;
        
    }

}
