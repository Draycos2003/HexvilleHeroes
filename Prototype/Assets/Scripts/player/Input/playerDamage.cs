using UnityEngine;

public class playerDamage : MonoBehaviour
{
    public playerController pc;
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        var dmgTarget = other.GetComponent<IDamage>();

        if(dmgTarget != null)
        {
            dmgTarget.TakeDamage(pc.damageAmount);
        }
    }
}
