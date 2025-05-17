using System.Collections;
using UnityEngine;

public class pickupItem : MonoBehaviour
{
    [SerializeField] Transform respawnPos;
    [SerializeField] GameObject item;
    [SerializeField] float respawnRate;

    enum boostType { health, shield, damage, speed }

    [SerializeField] boostType type;
    [SerializeField] int healthAmount;
    [SerializeField] int shieldAmount;
    [SerializeField] int damageAmount;
    [SerializeField] int speedAmount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawn();
}

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        IPickup item = other.GetComponent<IPickup>();
        if (item != null)
        {
            if (type == boostType.health)
            {
                Debug.Log("GAIN HP");
                item.gainHealth(healthAmount);
            }
            if (type == boostType.shield)
            {
                Debug.Log("GAIN DEF");
                item.gainShield(shieldAmount);
            }
            if (type == boostType.damage)
            {
                Debug.Log("DAMAGE UP");
                item.gainDamage(damageAmount);
            }
            if (type == boostType.speed)
            {
                Debug.Log("SPEED UP");
                item.gainSpeed(speedAmount);
            }

        }
        StartCoroutine(respawn());
    }

    void spawn()
    {
        Instantiate(item, respawnPos.position, transform.rotation);
    }

    IEnumerator respawn()
    {
        yield return new WaitForSeconds(respawnRate);
    }
}
