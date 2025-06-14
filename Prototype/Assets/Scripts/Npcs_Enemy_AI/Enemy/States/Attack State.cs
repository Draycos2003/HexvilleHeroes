using UnityEngine;

public class AttackState : IState<enemyAI>
{
    public void Enter(enemyAI e)
    {
        if (e.enableStateLogs) Debug.Log("Attack: Enter");
        e.agent.isStopped = true;
    }

    public void Update(enemyAI e)
    {
        if (!e.InAttackRange())
        {
            e.ChangeTo(enemyAI.StateType.Chase);
            return;
        }

        e.FaceTarget();
        e.PerformAttack();
    }

    public void Exit(enemyAI e)
    {
        if (e.enableStateLogs) Debug.Log("Attack: Exit");
        e.agent.isStopped = false;
    }
}
