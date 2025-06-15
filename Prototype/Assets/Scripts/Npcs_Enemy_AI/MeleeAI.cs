using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.Rendering;
using Unity.VisualScripting;



public class MeleeAI : MonoBehaviour, IDamage
{

    public int CurrentHP => HP;
    public int currentShield => Shield;

    float attackTimer;
    float angleToPlayer;
    Vector3 targetPos;
    Transform target;
    float updatePathDeadline;
    Color colorOrig;
    bool inRange;
    float pathUpdateDely;
    private float dist;


    [Header("Enemy Fields")]
    [SerializeField] int HP;
    [SerializeField] int Shield;
    [SerializeField] Renderer model;
    [SerializeField] Transform headPos;
    [SerializeField] float faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] float attackRange;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] float animTransSpeed;


    [Header("Melee Fields")]
    [SerializeField] float attackRate;
    [SerializeField] Collider hitPos;



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

        dist = Vector3.Distance(target.position, headPos.position);

        if (Physics.Raycast(headPos.position, targetPos, dist))
        {
            if (angleToPlayer <= FOV)
            {
                if (dist > attackRange) // dist check. If not in attacking range. Get in attacking range.
                {
                    UpdatePath();
                }
                else
                {
                    faceTarget();
                    if (attackTimer >= attackRate)
                    {
                        meleeAttack();
                    }
                       
                }
                return true;
            }
        }
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

    void meleeAttack()
    {
        anim.SetTrigger("attack");
        attackTimer = 0;
    }

    public void weaponColOn()
    {
        hitPos.enabled = true;
    }

    public void weaponColOff()
    {
        hitPos.enabled = false;
        
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

