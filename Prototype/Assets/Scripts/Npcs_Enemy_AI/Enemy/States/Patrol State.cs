using UnityEngine;
using UnityEngine.AI;

public class PatrolState : IState<enemyAI>
{
    private float patrolTimer;
    private Vector3 nextPatrolPoint;

    public void Enter(enemyAI e)
    {
        if (e.enableStateLogs) Debug.Log("Patrol: Enter");

        // Override for patrol only
        e.agent.isStopped = false;
        e.agent.stoppingDistance = 0f;

        patrolTimer = 0f;
        nextPatrolPoint = GetRandomPoint(e);
        e.agent.SetDestination(nextPatrolPoint);
    }

    public void Update(enemyAI e)
    {
        if (e.CanSeePlayer())
        {
            e.ChangeTo(enemyAI.StateType.Chase);
            return;
        }

        patrolTimer += Time.deltaTime;

        bool closeEnough = !e.agent.pathPending && e.agent.remainingDistance <= 0.05f;
        bool waited = patrolTimer >= e.patrolWaitTime;

        if (closeEnough && waited)
        {
            patrolTimer = 0f;
            nextPatrolPoint = GetRandomPoint(e);
            e.agent.SetDestination(nextPatrolPoint);
        }
    }

    public void Exit(enemyAI e)
    {
        if (e.enableStateLogs) Debug.Log("Patrol: Exit");

        // Restore original stopping distance
        e.agent.stoppingDistance = e.DefaultStoppingDistance;
    }

    private Vector3 GetRandomPoint(enemyAI e)
    {
        Vector2 offset = Random.insideUnitCircle * e.patrolRadius;
        Vector3 rawPoint = e.OriginPosition + new Vector3(offset.x, 0f, offset.y);

        if (NavMesh.SamplePosition(rawPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            return hit.position;

        return e.OriginPosition;
    }
}
