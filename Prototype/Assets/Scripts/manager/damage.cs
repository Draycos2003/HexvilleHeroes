using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Damage : MonoBehaviour
{

    enum DamageType
    {
        ranged, melee, frost, homing, DOT, AOE, RDOT
    }

    [SerializeField] DamageType type;
    [SerializeField] Rigidbody body;
    [SerializeField] GameObject obj;
    [SerializeField] playerController playerController;

    public int damageAmount;
    [SerializeField] int damageRate;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    [SerializeField] int damageAOE;
    [SerializeField] float radiusAOE;


    Vector2 centerAOE;
    Vector2 rangeAOE;
    
    bool isDamaging;
    bool frozen;
    float frozenTimer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (type == DamageType.ranged || type == DamageType.homing || type == DamageType.RDOT || type == DamageType.frost)
        {
            Destroy(gameObject, destroyTime);

            if(type == DamageType.ranged || type == DamageType.frost || type == DamageType.RDOT)
            {
                body.linearVelocity = transform.forward * speed; 
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        if(type == DamageType.homing)
        {
            body.linearVelocity = (gamemanager.instance.Player.transform.position - transform.position).normalized * speed * Time.deltaTime;
        }

        //rangeAOE = gamemanager.instance.Player.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.isTrigger)
        {
            return;
        }

        IDamage damage = other.GetComponent<IDamage>();
        
        if (damage != null && (type == DamageType.ranged || type == DamageType.melee))
        {
            if (playerController!=null)
            {
                damageAmount = playerController.damageAmount;
            }
            damage.TakeDamage(damageAmount);
        }

        if (damage != null && type == DamageType.frost)
        {
            damage.TakeDamage(damageAmount);
            
            //if (!frozen)
            //{
            //    frozenTimer += Time.deltaTime;
            //    StartCoroutine(frozenInTime());
            //    damage.TakeDamage(damageAmount);
            //    if(frozenTimer >= damageRate)
            //    {
            //        gamemanager.instance.PlayerScript.speed = gamemanager.instance.PlayerScript.speedOG;
            //        Debug.Log("unFrozen");
            //    }
            //}

        }

        if (type == DamageType.ranged || type == DamageType.homing || type == DamageType.frost)
        {
            Destroy(gameObject);
        }

        if (type == DamageType.AOE)
        {
            centerAOE = gamemanager.instance.Player.transform.position;

            if (Vector2.Distance(centerAOE, rangeAOE) <= radiusAOE)
            {
                damage.TakeDamage(damageAmount);
            }
        }

        if (type == DamageType.RDOT)
        {
            if (other.CompareTag("Player") && gamemanager.instance.Player != null)
            {
                obj.transform.SetParent(gamemanager.instance.Player.transform);
                body.linearVelocity = Vector3.zero;
                GetComponent<Renderer>().enabled = false;
           
            }
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if(other.isTrigger)
        {
            return;
        }

        IDamage damage = other.GetComponent<IDamage>();

        if(damage != null && type == DamageType.DOT || type == DamageType.RDOT)
        {
            if(!isDamaging)
            {
                StartCoroutine(damageOverTime(damage));
            }
        }
    }

    //IEnumerator frozenInTime()
    //{
    //    frozen = true;

    //    gamemanager.instance.PlayerScript.speed = 0;
    //    Debug.Log("HAHAHAHAH FROZEN");

    //    yield return new WaitForSeconds(damageRate);
    //    Debug.Log("This isnt working");
    //    frozen = false;
    //}

    IEnumerator damageOverTime(IDamage damage)
    {
        isDamaging = true;
        damage.TakeDamage(damageAmount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }
}
