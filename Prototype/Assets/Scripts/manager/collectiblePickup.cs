using System.Collections;
using UnityEngine;

public class collectiblePickup : MonoBehaviour
{
    [Header("For Buffs")]
    [SerializeField] Transform respawnPos;
    [SerializeField] GameObject item;

    [Header("For Weapons")]
    [SerializeField] pickupItemSO weapon;
    [SerializeField] float respawnRate;

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
        IPickup pickup = other.GetComponent<IPickup>();
        if (pickup != null)
        {
            pickup.getItemStats(weapon);

            Destroy(gameObject);
        }
    }

    void spawn()
    {
        Instantiate(item, respawnPos.position, transform.rotation);
    }

    IEnumerator respawn()
    {
        Destroy(gameObject);
        yield return new WaitForSeconds(respawnRate);
        spawn();
    }
}
