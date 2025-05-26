using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;

public class agentWeapon : MonoBehaviour
{
    [SerializeField] private EquippableSO weapon;
    [SerializeField] private InventoryItem item;
    [SerializeField] private inventorySO inventoryData;

    [SerializeField] private List<ItemParameter> parametersToModify, itemCurrentState;

    public void SetWeapon(EquippableSO weaponItemSO, List<ItemParameter> itemState)
    {
        Debug.Log("EQUIP");
        weapon = weaponItemSO;
        weapon.isEquipped = true;
        itemCurrentState = new List<ItemParameter>(itemState);
    }

    public void ModifyAllParameters()
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

    public void ModifyParameter(string parameterName)
    {
        foreach (var parameter in parametersToModify)
        {
            if (itemCurrentState.Contains(parameter) && (parameter.itemParameter.ParameterName == parameterName))
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
