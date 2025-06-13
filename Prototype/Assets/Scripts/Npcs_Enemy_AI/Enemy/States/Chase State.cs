using UnityEngine;

public class ChaseState : IState<enemyAI>
{
    public void Enter(enemyAI e)
    {
        if (e.enableStateLogs) Debug.Log("Chase: Enter");
        e.agent.isStopped = false;
    }

    public void Update(enemyAI e)
    {
        // attack if close
        if (e.InAttackRange())
        {
            e.ChangeTo(enemyAI.StateType.Attack);
            return;
        }

        // seen again? reset lostTimer & keep chasing
        if (e.CanSeePlayer())
        {
            // we rely on ChangeTo resetting lostTimer on entry
            e.UpdatePath();
            return;
        }

        // out of sight: still within grace period?
        if (e.LostTimer < e.ChaseDuration)
        {
            e.UpdatePath();
        }
        else
        {
            e.ChangeTo(enemyAI.StateType.Patrol);
        }
    }

    public void Exit(enemyAI e)
    {
        if (e.enableStateLogs) Debug.Log("Chase: Exit");
        e.agent.isStopped = true;
    }
}
