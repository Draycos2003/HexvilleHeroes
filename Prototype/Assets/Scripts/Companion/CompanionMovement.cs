using UnityEngine;
using UnityEngine.AI;

public class CompanionMovement_1 : MonoBehaviour
{
    [SerializeField]
    private Transform player;
    
    [SerializeField]
    private GameObject companion;
    
    [SerializeField]
    private NavMeshAgent agent;
    
    [SerializeField]
    [Range(5,10f)]
    private float companionSpeed;
    
    [SerializeField]
    [Range(0,10f)]
    private float companionRotateSpeed;
    
    [SerializeField]
    private float allowedDistance;

    private float playerDist;
    private RaycastHit hit;
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame

    // Try to make this into States.

    void Update()
    {

        transform.LookAt(player.transform.position);

        if (Physics.Raycast(companion.transform.position, transform.TransformDirection(Vector3.forward), out hit))
        {
            float playerDist = hit.distance;

            if (playerDist > allowedDistance)
            {
               
                agent.speed = companionSpeed;
                agent.SetDestination(player.transform.position);
            }
            else
            {
                agent.speed = 0;
            }

        }
    }
}
            