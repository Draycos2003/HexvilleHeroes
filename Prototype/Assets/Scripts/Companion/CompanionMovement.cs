
using UnityEngine;
using UnityEngine.AI;
using UnityEngineInternal;

public class CompanionMovement_1 : MonoBehaviour
{
    [SerializeField]
    private Transform player;
    
    [SerializeField]
    private NavMeshAgent agent;
    
    [SerializeField]
    [Range(5,10f)]
    private float companionSpeed;

    CompanionAttack ca;
   
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        ca = GetComponentInChildren<CompanionAttack>();
        agent.updatePosition = false; 
        agent.updateRotation = false;
    }


    void Update()
    {
        MovementHandler();
    }

    void MovementHandler()
    {
        if(ca.attack && ca.FindClosestEnemy() != null)
        {
            agent.speed = companionSpeed;
            Vector3 enemyPos = ca.FindClosestEnemy().transform.position + (ca.FindClosestEnemy().transform.right * 1.5f);

            agent.SetDestination(enemyPos);
            agent.transform.LookAt(enemyPos);
         
            var next = agent.nextPosition + Vector3.up * 1.5f;
             
            agent.transform.position = Vector3.Lerp(agent.transform.position, next, companionSpeed);
        }
        else
        {
            agent.speed = companionSpeed;
            agent.SetDestination(player.transform.position + (player.transform.right * 1.5f));
            agent.transform.LookAt(player);
            var next1 = agent.nextPosition + Vector3.up * 1.5f;
            agent.transform.position = Vector3.Lerp(agent.transform.position, next1, companionSpeed);
        

        }

      
        
    }
    
}
            