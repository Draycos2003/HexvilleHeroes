using UnityEngine;

public class PatrolState : IState<enemyAI>
{
    public void Enter(enemyAI e)
    {
        if (e.enableStateLogs) 
            Debug.Log("Patrol: Enter");
    }

    public void Update(enemyAI e)
    {
        if (e.CanSeePlayer()) 
            e.ChangeTo(enemyAI.StateType.Chase);
        else 
            e.UpdatePath();
    }

    public void Exit(enemyAI e)
    {
        if (e.enableStateLogs) 
            Debug.Log("Patrol: Exit");
    }
}