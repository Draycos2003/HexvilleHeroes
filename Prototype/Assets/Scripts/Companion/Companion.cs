using UnityEngine;

public class Companion : AbstractStateBehaviour<CompanionState>
{
   private void Start()
   {
        ChangeState(CompanionState.idle); 
   }

   
}
