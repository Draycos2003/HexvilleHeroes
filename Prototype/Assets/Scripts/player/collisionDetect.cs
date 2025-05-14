using UnityEngine;

public class collisionDetect : MonoBehaviour
{
    public playerAttack PA;
    public int damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && PA.isAttacking)
        {
            Debug.Log(other.name);
            other.GetComponent<Animator>().SetTrigger("hit");
            other.GetComponent<IDamage>().TakeDamage(damage);
        }

        IPickup item = other.GetComponent<IPickup>();
        if (item != null )
        {

        }
    }
}
