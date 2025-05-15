using UnityEngine;

public class boosts : MonoBehaviour
{
    enum boostType { health, shield, damage, speed }

    [SerializeField] boostType type;
    [SerializeField] int health;
    [SerializeField] int shield;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
            if(type == boostType.health)
            {
                item.gainHealth(health);
            }
            if(type == boostType.shield)
            {
                item.gainShield(shield);
            }
        }
    }

}
