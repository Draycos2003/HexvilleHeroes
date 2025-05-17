using UnityEngine;

public interface IPickup
{
    void gainHealth(int amount);

    void gainShield(int amount);

    void gainDamage(int amount);

    void gainSpeed(int amount);
}
