using UnityEngine;

public class PatrolState : IState<enemyAI>
{
    public void Enter(enemyAI e)
    {
        if (e.enableStateLogs) Debug.Log("Patrol: Enter");
        e.agent.isStopped = false;
        e.UpdatePathToOrigin();
    }

    public void Update(enemyAI e)
    {
        if (e.CanSeePlayer())
        {
            e.ChangeTo(enemyAI.StateType.Chase);
            return;
        }

        float distHome = Vector3.Distance(e.transform.position, e.OriginPosition);
        if (distHome < 0.1f)
            e.agent.isStopped = true;
        else
            e.agent.SetDestination(e.OriginPosition);
    }

    public void Exit(enemyAI e)
    {
        if (e.enableStateLogs) Debug.Log("Patrol: Exit");
    }
}