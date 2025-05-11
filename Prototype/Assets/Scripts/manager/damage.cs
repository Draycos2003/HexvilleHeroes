using System.Collections;
using UnityEngine;

public class Damage : MonoBehaviour
{

    enum DamageType
    {
        ranged, melee, casting, homing, DOT 
    }

    [SerializeField] DamageType type;
    [SerializeField] Rigidbody body;

    [SerializeField] int damageAmount;
    [SerializeField] int damageRate;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    bool isDamaging;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (type == DamageType.ranged || type == DamageType.homing || type == DamageType.casting)
        {
            Destroy(gameObject, destroyTime);

            if(type == DamageType.ranged || type == DamageType.casting)
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.isTrigger)
        {
            return;
        }

        IDamage damage = other.GetComponent<IDamage>();
        if ((damage != null && type == DamageType.ranged || type == DamageType.melee || type == DamageType.casting || type == DamageType.casting))
        {
            damage.TakeDamage(damageAmount);
        }

        if (type == DamageType.ranged || type == DamageType.homing || type == DamageType.casting)
        {
            //Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.isTrigger)
        {
            return;
        }

        IDamage damage = other.GetComponent<IDamage>();

        if(damage != null && type == DamageType.DOT)
        {
            if(!isDamaging)
            {
                StartCoroutine(damageOther(damage));
            }
        }
    }

    IEnumerator damageOther(IDamage damage)
    {
        isDamaging = true;
        damage.TakeDamage(damageAmount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }
}
