using UnityEngine;

public class ChaseState : IState<enemyAI>
{
    public void Enter(enemyAI e)
    {
        if (e.enableStateLogs) Debug.Log("Chase: Enter");
    }

    public void Update(enemyAI e)
    {
        if (!e.CanSeePlayer())
        {
            e.ChangeTo(enemyAI.StateType.Patrol);
            return;
        }
        e.agent.SetDestination(e.target.position);
        if (e.InAttackRange()) e.ChangeTo(enemyAI.StateType.Attack);
    }

    public void Exit(enemyAI e)
    {
        if (e.enableStateLogs) Debug.Log("Chase: Exit");
    }
}