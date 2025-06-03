using UnityEngine;

public class NpcFollwerScript : MonoBehaviour
{
    public Transform player; // who he/she is following
    public GameObject Npc; 
    public float PlayerDistance; // how far the player is 
    public float allowedDistances; // how far the npc can be before he/she starts chasing after the player
    public float followerSpeed; // npc speed
    public RaycastHit hit;

    private void Update()
    {

        transform.LookAt(player.transform.position);
        
        // Us navmesh instead of distance checking

        if (Physics.Raycast(Npc.transform.position, transform.TransformDirection(Vector3.forward), out hit)) {

            PlayerDistance = hit.distance;

            if(PlayerDistance > allowedDistances)
            {
                followerSpeed = 0.02f;
                //Speed will be the animatior's speed
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, followerSpeed);
                // Play running animation 
            }

            else
            {
                followerSpeed = 0;
                // Play idle animation 
            }
        }
    }

    void DefendPlayer()
    {
        // if player is being attacked
        // Get enemy attacking player
        // attack that enemy till death and move to the next one
    }

    void Attack()
    {
        // if hurt
        // Get enemy transform
        // attack!!
    }
}


