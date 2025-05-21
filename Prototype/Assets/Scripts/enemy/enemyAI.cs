using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Unity.VisualScripting;


public class enemyAI : MonoBehaviour, IDamage
{
<<<<<<< Updated upstream
    public enum EnemyTypes
    {
        Range, 
        Melee,
    }

    public EnemyTypes enemyType;
=======
    [SerializeField] private Transform headPos;
    [SerializeField] private int FOV;
    [SerializeField] private NavMeshAgent agent;

    private Vector3 targetPos;
    private float angleToPlayer;
    private float shootTimer;
    private float stoppingDistOrig;
    public int attackRange;
>>>>>>> Stashed changes


    [Header("Enemy Fields")]
    public int HP;
    public int Shield;
    public Renderer model;
    public float faceTargetSpeed;
    public Transform target;
    public float attackRange;

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


    public bool inRange;


    private EnemyReferences references;

    void Start()
    {
        references = GetComponent<EnemyReferences>();
        colorOrig = model.material.color; // Starter color
        gamemanager.instance.updateGameGoal(1); // total enemy count
    }

    // Update is called once per frame
    void Update()
    {
        //if (target != null)
        //{
        //    if (inRange)
        //    {
        //        UpdatePath();

<<<<<<< Updated upstream
        //        if (LOS() == true)
        //            shoot();
        //    }
            

        //    if (references.navMesh.remainingDistance < references.navMesh.stoppingDistance)
        //    {
        //        faceTarget();
        //    }
        //}
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == ("Player"))
    //    {
    //        inRange = true;
    //    }
=======
            if (inRange)
            {
                CanSeePlayer();
                float dist = Vector3.Distance(transform.position,target.position);

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
    }
 
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == ("Player"))
        {
            inRange = true;
        }
>>>>>>> Stashed changes

    //}
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == ("Player"))
        {
            inRange = false;
        }
    }

<<<<<<< Updated upstream
=======
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

>>>>>>> Stashed changes
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

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        model.material.color = colorOrig;
    }

    void faceTarget()
    {
<<<<<<< Updated upstream
        //Creates a smoother rotation by using Slerp.
        Vector3 lookPos = target.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, faceTargetSpeed * Time.deltaTime);
=======
        //      --  Creates a smoother rotation by using Slerp.
        //      -- I use Slerp instead of lerp because i don't know what type of rotation the character could make it could be big but if not, it could be juddery using lerp so be safe with Slerp.

        //Vector3 lookPos = target.position - transform.position;
        //lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(new Vector3(targetPos.x, transform.position.y, targetPos.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * faceTargetSpeed);
>>>>>>> Stashed changes

    }

    public void shoot()
    {
        if (projectile == null)
            Debug.LogWarning("No projectile set");

        Instantiate(projectile, shootPos.position, transform.rotation);
    }

    public void UpdatePath()
    {
        //Updates the Path every 0.2 seconds instead of every frame
        if (Time.time >= updatePathDeadline)
        {
            updatePathDeadline = Time.time + references.pathUpdateDely;
            references.navMesh.SetDestination(target.position);
        }
    }

    public bool LOS()
    {
        LayerMask mask = LayerMask.GetMask("Player");
        RaycastHit hitInfo;


        if (Physics.Raycast(transform.forward, target.position, out hitInfo, attackRange, mask))
        {
            if (attackRange == 0f)
                Debug.LogWarning("No attack range set");
            
            if (hitInfo.transform.CompareTag("Player"))
            {
                return true;
            }
            Debug.DrawRay(transform.position, target.position * hitInfo.distance, Color.red);
            Debug.Log("Hit");
        }
        return false;


    }


}
