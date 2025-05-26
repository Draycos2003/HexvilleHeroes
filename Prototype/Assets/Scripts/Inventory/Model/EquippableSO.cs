using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EquippableSO : ItemSO, IDestroyableItem, IItemAction, IEquippable
{
    public string ActionName => "Equip";
    [HideInInspector] public bool isEquipped = false;

    public bool PerformAction(GameObject character, List<ItemParameter> itemState)
    {
        agentWeapon weaponSystem = character.GetComponent<agentWeapon>();
        if (weaponSystem != null)
        {
            weaponSystem.SetWeapon(this, itemState == null ? defaultParameterList : itemState);
            return true;
        }
        return false;
    }
}
