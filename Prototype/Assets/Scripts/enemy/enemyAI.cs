using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    //  -- enemy qualities
    [SerializeField] int HP;
    [SerializeField] float faceTargetSpeed;
    private float updatePathDeadline;
    public Transform target;
    Color colorOrig;
    [SerializeField] Renderer model;


    //  -- shooting fields
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    private float shootDistance;
    float shootTimer;
    bool inRange;

    private EnemyReferences references;

    public int CurrentHP => HP;

    private void Awake()
    {
        references = GetComponent<EnemyReferences>();
    }

    void Start()
    {
        colorOrig = model.material.color; // Starter color
        gamemanager.instance.updateGameGoal(1); // total enemy count
        shootDistance = references.navMesh.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        

        if (target != null)
        {
//          -- in range is true if the outcome is less or equal to stopping distance
            inRange = Vector3.Distance(transform.position, target.position) <= shootDistance;

            if (inRange)
            {
                faceTarget();
                references.animate.SetBool("casting", inRange);
            }
            else
            {
                UpdatePath();
            }

//          -- Faces target if still in range
            if (references.navMesh.remainingDistance < references.navMesh.stoppingDistance)
            {
                faceTarget();
            }
        }
        if (!inRange)
            references.animate.SetFloat("speed", references.navMesh.desiredVelocity.sqrMagnitude);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = false;
        }
    }

    public void TakeDamage(int Amount)
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

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        model.material.color = colorOrig;
    }

    void faceTarget()
    {
//      --  Creates a smoother rotation by using Slerp.
//      -- I use Slerp instead of lerp because i don't know what type of rotation the character could make it could be big but if not, it could be juddery using lerp so be safe with Slerp.

        Vector3 lookPos = target.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, faceTargetSpeed * Time.deltaTime);

    }

    void shoot()
    {
        shootTimer = 0;

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
