using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.Rendering;
using Unity.VisualScripting;



public class enemyAI : MonoBehaviour, IDamage
{

    enum EnemyType
    {
        melee, range,
    }

    [SerializeField] EnemyType type;

    private float angleToPlayer;
    private float shootTimer;
    private float attackTimer;


    private Vector3 targetPos;

    [Header("Enemy Fields")]
    public int HP;
    public int Shield;
    public Renderer model;
    public float faceTargetSpeed;
    [SerializeField] float attackRange;
    [SerializeField] Transform headPos;
    [SerializeField] int FOV;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] float animTransSpeed;

    public int CurrentHP => HP;
    public int currentShield => Shield;
    private float updatePathDeadline;

    [Header("Range Fields")]
    public Transform shootPos;
    public GameObject projectile;
    public float shootRate;

    [Header("Melee Fields")]
    public float attackRate;
    [SerializeField] GameObject weapon;

    Transform target;
    Color colorOrig;
    bool inRange;
    float pathUpdateDely;
    private float dist;
    

    private void Start()
    {
        if(weapon != null)
            weapon.GetComponent<Collider>().enabled = false;

        setAnimPara();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        colorOrig = model.material.color; // Starter color
        //gamemanager.instance.updateGameGoal(1); // total enemy count
    }

    // Update is called once per frame
    private void Update()
    {
        shootTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;
        
        setAnimPara();
        
        if (inRange)//player is in the collider
        {
            CanSeePlayer(); // can we see the player?
        }
        else
        {
            UpdatePath(); // get back in range of the player
        }
    }

    void setAnimPara()
    {
        float agentSpeedCur = agent.velocity.normalized.magnitude;
        float animSpeedCur = anim.GetFloat("Speed");

        anim.SetFloat("Speed", Mathf.Lerp(animSpeedCur, agentSpeedCur, Time.deltaTime * animTransSpeed));
    }

    bool CanSeePlayer()
    {
        targetPos = (target.transform.position - headPos.position);
        
        angleToPlayer = Vector3.Angle(new Vector3(targetPos.x, 0, targetPos.z), transform.forward);
        
        Debug.DrawRay(headPos.position, new Vector3(targetPos.x, 0, targetPos.z));

        dist = Vector3.Distance(target.position,headPos.position);

        if (Physics.Raycast(headPos.position, targetPos, dist))
        {
            if (angleToPlayer <= FOV)
            {
                if(dist > attackRange) // dist check. If not in attacking range. Get in attacking range.
                {
                    UpdatePath();
                }
                else
                {
                    faceTarget();
                    
                    if(type == EnemyType.range)
                    {
                        if (shootTimer >= shootRate )
                            shoot();

                    }
                    if(type == EnemyType.melee)
                    {
                        if(attackTimer >= attackRate)
                        {
                            Debug.Log("calling attack"); // works
                            attack();
                        }
                    }
                }
                return true;
            }       
        }
        return false;   
    }


    public void TakeDamage(int amount)
    {
        if (Shield > 0)
        {
            Shield -= amount;
            StartCoroutine(flashRed());
        }
        else
        {
            HP -= amount;
            StartCoroutine(flashRed());
        }

        // check for death
        if (HP <= 0)
        {
            gamemanager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }

    private IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        model.material.color = colorOrig;
    }

    private void faceTarget()
    {
        Vector3 lookPos = target.transform.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, faceTargetSpeed * Time.deltaTime);
    }

    private void shoot()
    {
        anim.SetTrigger("shoot");
        shootTimer = 0;
    }

    private void attack()
    {
        Debug.Log("Trigger was set"); // works
        anim.SetTrigger("attack");
        attackTimer = 0;
    }

    public void weaponColOn()
    {
        weapon.GetComponent<Collider>().enabled = true;
    }
    public void weaponColOff()
    {
        weapon.GetComponent<Collider>().enabled = false;
    }

    public void createProjectile()
    {
        if (projectile == null)
        {
            Debug.LogWarning("No projectile set");
            return;
        }

        Instantiate(projectile, shootPos.position, transform.rotation);
    }

    public void UpdatePath()
    {
        if (Time.time >= updatePathDeadline)
        {
            pathUpdateDely = 0.2f;
            updatePathDeadline = Time.time + pathUpdateDely;

            if (target != null)
                agent.SetDestination(target.transform.position);
            
            Debug.Log("Updating Path");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == ("Player"))
        {
            target = other.transform; // grabs the targets transform once inside the collider
            inRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == ("Player"))
        {
            inRange = false;
        }
    }
}

