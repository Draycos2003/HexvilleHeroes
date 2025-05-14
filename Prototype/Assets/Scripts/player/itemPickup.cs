using UnityEngine;

public class itemPickup : MonoBehaviour
{
    public int health;
    public int shield;

    private void OnTriggerEnter(Collider other)
    {
        IPickup item = other.GetComponent<IPickup>();
        if (item != null)
        {
            Debug.Log("HP");
            item.gainHealth(health);
        }
        else if (item != null)
        {
            Debug.Log("Shield");
            item.gainShield(shield);
        }
    }
    

}
