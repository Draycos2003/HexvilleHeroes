using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

enum Type
{
    health,
    shield,
    damage,
    speed,
    money
}

public class collectiblePickup : MonoBehaviour
{
    [Header("Collectible")]
    [SerializeField] Type type;
    [SerializeField] int amountToBuff;
    [SerializeField] float respawnRate;
    [SerializeField] AudioClip pickupSound;


    private void OnTriggerEnter(Collider other)
    {
        soundFXmanager.instance.PlaySoundFXClip(pickupSound, transform, 0.1f);
        IPickup pickup = other.GetComponent<IPickup>();
        if (pickup != null)
        {
            playerController pc = other.GetComponent<playerController>();
            if (pc != null)
            {
                switch (type)
                {
                    case Type.health:
                        pc.gainHealth(amountToBuff);
                        break;
                    case Type.shield:
                        pc.gainShield(amountToBuff);
                        break;
                    case Type.damage:
                        pc.gainDamage(amountToBuff);
                        break;
                    case Type.speed:
                        pc.gainSpeed(amountToBuff);
                        break;
                    case Type.money:
                        pc.AddGold(amountToBuff);
                        break;

                }
            }
            Destroy(gameObject);
        }

    }
}
