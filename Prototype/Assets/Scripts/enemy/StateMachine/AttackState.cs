using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class AttackState : StateMachine
{
    enemyAI enemy;
    public ChaseState chaseState;

    public override StateMachine RunCurrentState()
    {
        while(enemy.LOS() == true)
        {
            enemy.shoot();        

            if(enemy.LOS() == false)
            {
                break;
            }
        }
        return chaseState;
    }

}
