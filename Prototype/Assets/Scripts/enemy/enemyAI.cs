using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.Rendering;


public class enemyAI : MonoBehaviour, IDamage
{

    private Vector3 playerDir;
    private float angleToPlayer;
    private float shootTimer;
    private float stoppingDistOrig;

    [SerializeField] private Transform headPos;
    [SerializeField] private int FOV;
    [SerializeField] private NavMeshAgent agent;

    private Vector3 targetPos;
    public int attackRange;


    [Header("Enemy Fields")]
    public int HP;
    public int Shield;
    public Renderer model;
    public float faceTargetSpeed;
    public Transform target;

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
    
    EnemyReferences references;

    private void Start()
    {
        references = GetComponent<EnemyReferences>();
        colorOrig = model.material.color; // Starter color
        gamemanager.instance.updateGameGoal(1); // total enemy count
        stoppingDistOrig = agent.stoppingDistance;
    }

    // Update is called once per frame
    private void Update()
    {
        if (inRange)
        {
            CanSeePlayer();
                
            float dist = Vector3.Distance(transform.position,target.position);
               
            if (CanSeePlayer())
            {
                references.animate.SetBool("casting", inRange);
            }

            if (dist > attackRange)
            {
                UpdatePath();
                if (dist == attackRange)
                {
                    agent.isStopped = true;
                }
            }
            else 
            {
                if (shootTimer <= shootRate)
                    shoot();
            }
        }
        else if (!inRange)
        {
            UpdatePath();
        }
    }

    void setAnimPara()
    {

    }

    bool CanSeePlayer()
    {
        targetPos = (target.transform.position - headPos.position);
        angleToPlayer = Vector3.Angle(new Vector3(targetPos.x, 0, targetPos.z), transform.forward);
        Debug.DrawRay(headPos.position, new Vector3(targetPos.x, 0, targetPos.z));

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, targetPos, out hit, attackRange))
        {
            if (angleToPlayer <= FOV && hit.collider.CompareTag("Player"))
            {
                UpdatePath();
                Debug.Log(hit.collider);

                if (shootTimer >= shootRate)
                {
                    shoot();
                }

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();
                }

                agent.stoppingDistance = stoppingDistOrig;
                return true;
            }       
        }
        agent.stoppingDistance = 0;
        return false;
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
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, faceTargetSpeed * Time.deltaTime);
    }

    private void shoot()
    {
        if (projectile == null)
            Debug.LogWarning("No projectile set");

        Instantiate(projectile, shootPos.position, transform.rotation);
    }

    public void UpdatePath()
    {
        if (Time.time >= updatePathDeadline)
        {
            updatePathDeadline = Time.time + references.pathUpdateDely;
            references.navMesh.SetDestination(target.position);
        }
    }
    private void OnTriggerEnter(Collider other)
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

