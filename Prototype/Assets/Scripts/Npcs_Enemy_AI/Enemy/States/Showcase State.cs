using UnityEngine;

public class ShowcaseState : IState<enemyAI>
{
    public void Enter(enemyAI e)
    {
        if (e.enableStateLogs) Debug.Log("Showcase: Enter");
        e.agent.isStopped = true;
    }

    public void Update(enemyAI e)
    {
        // idle
    }

    public void Exit(enemyAI e)
    {
        if (e.enableStateLogs) Debug.Log("Showcase: Exit");
        e.agent.isStopped = false;
    }
}
