using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.Rendering;



public class enemyAI : MonoBehaviour, IDamage
{

    private Vector3 playerDir;
    private float angleToPlayer;
    private float shootTimer;

    [SerializeField] Transform headPos;
    [SerializeField] int FOV;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] float animTransSpeed;

    private Vector3 targetPos;
    public int attackRange;

    [Header("Enemy Fields")]
    public int HP;
    public int Shield;
    public Renderer model;
    public float faceTargetSpeed;
    Transform target;

    public int CurrentHP => HP;
    public int currentShield => Shield;
    private float updatePathDeadline;

    [Header("Range Fields")]
    public Transform shootPos;
    public GameObject projectile;
    public float shootRate;

    [Header("Melee Fields")]
    public float attackSpeed;
    public GameObject weapon;
    public Collider hitPos;

    Color colorOrig;
    bool inRange;
    float pathUpdateDely;

    private void Start()
    {
        setAnimPara();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        colorOrig = model.material.color; // Starter color
        gamemanager.instance.updateGameGoal(1); // total enemy count
    }

    // Update is called once per frame
    private void Update()
    {
        shootTimer += Time.deltaTime;
        
        setAnimPara();

        if (inRange && CanSeePlayer())
        {
           
            float dist = Vector3.Distance(transform.position, target.position);

            if (dist > attackRange)
            {
                UpdatePath();
            }
            else if (dist <= attackRange)
            {
                if(shootTimer >= shootRate)
                {
                    shoot();
                }
            }

        }
        else
        { 
            UpdatePath();
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
        if (target == null)
        {
            return false;
        }
        else
        {
            targetPos = (target.transform.position - headPos.position);
        
           angleToPlayer = Vector3.Angle(new Vector3(targetPos.x, 0, targetPos.z), transform.forward);
        
            Debug.DrawRay(headPos.position, new Vector3(targetPos.x, 0, targetPos.z));

            RaycastHit hit;
        
            if (Physics.Raycast(headPos.position, targetPos, out hit, attackRange))
            {
                if (angleToPlayer <= FOV && hit.collider.CompareTag("Player"))
                {
                    Debug.Log(hit.collider);


                    if (agent.remainingDistance <= agent.stoppingDistance)
                    {
                        faceTarget();
                    }

                    return true;
                }       
            }
            return false;

        }
    }


    public void TakeDamage(int Amount)
    {
        if (currentShield <= 0)
        {
            HP -= Amount;
           
            if (HP <= 0)
            {
                Destroy(gameObject);
                gamemanager.instance.updateGameGoal(-1);

            }
            else
            {
                StartCoroutine(flashRed()); 
            }
        }
        else
        {
            Shield -= Amount;
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
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, faceTargetSpeed * Time.deltaTime);
    }

    private void shoot()
    {
        anim.SetTrigger("shoot");
        shootTimer = 0;
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
            agent.SetDestination(target.transform.position);
            Debug.Log("Updating Path");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == ("Player"))
        {
            target = other.transform;
            inRange = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == ("Player"))
        {
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

