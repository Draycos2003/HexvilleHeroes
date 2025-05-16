using UnityEngine;
using System.Collections;
using UnityEngine.AI;


public class enemyAI : MonoBehaviour, IDamage
{
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
    bool inRange;

    [Header("Melee Fields")]
    public float attackSpeed;
    public GameObject weapon;
    public Collider hitPos;

    Color colorOrig;

    private EnemyReferences references;
    private void Awake()
    {
        references = GetComponent<EnemyReferences>();
    }

    void Start()
    {
       colorOrig = model.material.color; // Starter color
        gamemanager.instance.updateGameGoal(1); // total enemy count
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {

            if (inRange == true)
            {
                references.animate.SetBool("casting", inRange);

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
        if (other.tag ==("Player"))
        {
            inRange = false;
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
