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
