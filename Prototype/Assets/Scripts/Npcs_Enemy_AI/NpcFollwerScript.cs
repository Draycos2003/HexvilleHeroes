
using UnityEngine;
using UnityEngine.AI;

public class NpcFollwerScript : MonoBehaviour
{
    // Everything works beisdes damaging enemys.

    enemyAI enemy;

    public Transform target;  // who he/she is following
    public GameObject Npc; 
    public float allowedDistances; // how far the npc can be before he/she starts chasing after the player
    public float followerSpeed; // npc speed
    public NavMeshAgent agent;
    public Animator anim;
    public float animTransSpeed;
    public float attackRate;
    public GameObject weapon;
    
    RaycastHit hit;
    float attackTimer;
    float PlayerDistance; // how far the player is 
    bool inRange;


    private void Start()
    {
        setAnimPara();
        weapon.GetComponent<Collider>().enabled = false;
        enemy = Object.FindAnyObjectByType<enemyAI>();
        agent = GetComponent<NavMeshAgent>();  
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;
        
        Follow();
    }

    void Follow() {

        transform.LookAt(target);
        setAnimPara();

        if (Physics.Raycast(Npc.transform.position, transform.TransformDirection(Vector3.forward), out hit))
        {

            PlayerDistance = hit.distance;

            if (PlayerDistance > allowedDistances)
            {

                agent.SetDestination(target.position);
                 
                if(enemy.CanSeePlayer() == true)
                {
                    DefendPlayer();
                    transform.LookAt(enemy.transform.position);

                    if(inRange == true && attackTimer >= attackRate)
                    {
                        
                        Attack();
                        
                    }
                }
            }
        }
    }

    void DefendPlayer()
    {
        // if enemy is aggro on player

        if (enemy.enemyAggro)
        {
            for (int i = 0; i < enemy.AggroList.Count; i++)
            {
                agent.SetDestination(enemy.AggroList[i].transform.position);
            }

            if (agent.remainingDistance == 0)
            {
                inRange = true;
            }


            if (enemy.isDead)
            { 
                for (int i = 0; i < enemy.AggroList.Count; i++)
                {
                    if(enemy.HP != 0 && enemy.AggroList[i] != null)
                    {
                        agent.SetDestination(enemy.AggroList[i].transform.position);
                    }
                    else
                    {
                        DefendPlayer();
                    }
                }

            }
        }
        else
        {
            Follow();
        }
    }

    void Attack()
    {
        anim.SetTrigger("attack");
        attackTimer = 0;
    }

    void setAnimPara()
    {
        float agentSpeedCur = agent.velocity.normalized.magnitude;
        float animSpeedCur = anim.GetFloat("Speed");

        anim.SetFloat("Speed", Mathf.Lerp(animSpeedCur, agentSpeedCur, Time.deltaTime * animTransSpeed));
    }

    public void WeaponColOn()
    {
        weapon.GetComponent<Collider>().enabled = true;
    }
    public void WeaponColOff()
    {
        weapon.GetComponent<Collider>().enabled = false;
    }
  
}



