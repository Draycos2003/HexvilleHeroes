using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.Rendering;


public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] private Transform headPos;
    [SerializeField] private int FOV;
    [SerializeField] private NavMeshAgent agent;

    private Vector3 playerDir;
    private float angleToPlayer;
    private float shootTimer;
    private float stoppingDistOrig;

    public enum EnemyTypes
    {
        Range,
        Melee,
    }

    public EnemyTypes enemyType;


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
    public GameObject bullet;
    public float shootRate;
    private bool inRange;

    [Header("Melee Fields")]
    public float attackSpeed;
    public GameObject weapon;
    public Collider hitPos;

    private Color colorOrig;

    private EnemyReferences references;

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
        if (target != null)
        {

            if (inRange == true)
            {
                if (CanSeePlayer())
                {
                    references.animate.SetBool("casting", inRange);
                }

            }
            else
            {
                references.animate.SetBool("casting", inRange);
                UpdatePath();
            }

            //          -- Faces target if still in range
            if (references.navMesh.remainingDistance < references.navMesh.stoppingDistance)
            {
                faceTarget();
            }
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

    bool CanSeePlayer()
    {
        playerDir = (gamemanager.instance.Player.transform.position - headPos.position);
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);
        Debug.DrawRay(headPos.position, new Vector3(playerDir.x, 0, playerDir.z));

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (angleToPlayer <= FOV && hit.collider.CompareTag("Player"))
            {
                agent.SetDestination(gamemanager.instance.Player.transform.position);

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
        //      --  Creates a smoother rotation by using Slerp.
        //      -- I use Slerp instead of lerp because i don't know what type of rotation the character could make it could be big but if not, it could be juddery using lerp so be safe with Slerp.

        //Vector3 lookPos = target.position - transform.position;
        //lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * faceTargetSpeed);

    }

    private void shoot()
    {
        Instantiate(bullet, shootPos.position, transform.rotation);
    }

    private void UpdatePath()
    {
        //      -- Updates the Path every 0.2 seconds instead of every frame like navMesh.SetDestination(target.postion)
        if (Time.time >= updatePathDeadline)
        {
            Debug.Log("Updating Path");
            updatePathDeadline = Time.time + references.pathUpdateDely;
            references.navMesh.SetDestination(target.position);
        }
    }
}
