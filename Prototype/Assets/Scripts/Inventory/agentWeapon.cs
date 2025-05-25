using System;
using System.Collections.Generic;
using UnityEngine;

public class agentWeapon : MonoBehaviour
{
    [SerializeField] private EquippableSO weapon;
    [SerializeField] private inventorySO inventoryData;

    [SerializeField] private List<ItemParameter> parametersToModify, itemCurrentState;

    public void SetWeapon(EquippableSO weaponItemSO, List<ItemParameter> itemState)
    {
        if(weapon != null)
        {
            inventoryData.AddItem(weapon, 1, itemCurrentState);
        }

        weapon = weaponItemSO;
        itemCurrentState = new List<ItemParameter>(itemState);
        ModifyParameters();
    }

    private void ModifyParameters()
    {
        foreach(var parameter in parametersToModify)
        {
            if(itemCurrentState.Contains(parameter))
            {
                int index = itemCurrentState.IndexOf(parameter);
                float newValue = itemCurrentState[index].value + parameter.value;
                itemCurrentState[index] = new ItemParameter
                {
                    itemParameter = parameter.itemParameter,
                    value = newValue
                };
            }
        }
    }
}
