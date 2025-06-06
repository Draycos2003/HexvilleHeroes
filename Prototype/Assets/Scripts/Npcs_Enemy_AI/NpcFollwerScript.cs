using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class NpcFollwerScript : MonoBehaviour
{
    // Everything works beisdes damaging enemys.

    // [Removed] enemyAI enemy;
    // [Added] Track the current enemy this NPC is targeting
    private enemyAI currentEnemy;

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

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        // [Removed] enemy = Object.FindAnyObjectOfType<enemyAI>();
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;

        // [Added] Find all alive, aggroed enemies each frame using non-deprecated API
        var aggroed = Object.FindObjectsByType<enemyAI>(FindObjectsSortMode.None)
                       .Where(e => !e.isDead && e.enemyAggro)
                       .ToArray();

        if (aggroed.Length > 0)
        {
            // [Added] If we don’t have a valid target, pick the first in the list
            if (currentEnemy == null || currentEnemy.isDead || !currentEnemy.enemyAggro)
            {
                currentEnemy = aggroed[0];
            }

            // [Added] If we have a valid currentEnemy, go defend
            if (currentEnemy != null)
            {
                DefendPlayer();
                return; // skip Follow() if defending
            }
        }
        else
        {
            // [Added] No aggroed enemies -> clear currentEnemy and fall back to following
            currentEnemy = null;
        }

        Follow();
    }

    void Follow()
    {
        transform.LookAt(target);
        setAnimPara();

        if (Physics.Raycast(Npc.transform.position, transform.TransformDirection(Vector3.forward), out hit))
        {
            PlayerDistance = hit.distance;

            if (PlayerDistance > allowedDistances)
            {
                agent.SetDestination(target.position);

                // [Changed] Check the currentEnemy instead of single 'enemy'
                if (currentEnemy != null && currentEnemy.CanSeePlayer())
                {
                    DefendPlayer();
                    transform.LookAt(currentEnemy.transform.position);

                    // [Added] In Follow(), if we are already in attack range, we can attempt an attack
                    float distToEnemyInFollow = Vector3.Distance(transform.position, currentEnemy.transform.position);
                    if (distToEnemyInFollow <= agent.stoppingDistance + 0.1f)
                    {
                        inRange = true;
                    }
                    else
                    {
                        inRange = false;
                    }

                    if (inRange && attackTimer >= attackRate)
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
        if (currentEnemy != null && currentEnemy.enemyAggro)
        {
            // [Changed] Chase the currentEnemy’s position directly
            agent.SetDestination(currentEnemy.transform.position);
            transform.LookAt(currentEnemy.transform.position);
            setAnimPara();

            // [Added] Check distance to see if we're in range to attack
            float distToEnemy = Vector3.Distance(transform.position, currentEnemy.transform.position);
            if (distToEnemy <= agent.stoppingDistance + 0.1f)
            {
                inRange = true;
            }
            else
            {
                inRange = false;
            }

            // [Added] If in range and cooldown is ready, attack
            if (inRange && attackTimer >= attackRate)
            {
                Attack();
            }

            // [Added] If the currentEnemy just died, clear it so Update() picks a new one next frame
            if (currentEnemy.isDead)
            {
                currentEnemy = null;
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
