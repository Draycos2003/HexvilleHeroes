using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Unity.VisualScripting;


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
        if (target != null)
        {

            //if (LOS() == true)
            //{
            //    references.animate.SetBool("casting", LOS());

            //}
            //else
            //{
            //    references.animate.SetBool("casting", LOS());
            //    UpdatePath();
            //}

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
        //Creates a smoother rotation by using Slerp.
        Vector3 lookPos = target.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, faceTargetSpeed * Time.deltaTime);

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
            Debug.Log("Updating Path");
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
