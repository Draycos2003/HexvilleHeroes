using UnityEngine;

public class ShowcaseState : IState<enemyAI>
{
    public void Enter(enemyAI e)
    {
        if (e.enableStateLogs) 
            Debug.Log("Showcase: Enter");
    }

    public void Update(enemyAI e)
    {
        // idle—no movement or attacks
    }

    public void Exit(enemyAI e)
    {
        if (e.enableStateLogs)
            Debug.Log("Showcase: Exit");
    }
}