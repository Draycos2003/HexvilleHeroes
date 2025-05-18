using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class AttackState : StateMachine
{
    public override StateMachine RunCurrentState()
    {
        Debug.Log("Attacking");
        return this;
    }

}
