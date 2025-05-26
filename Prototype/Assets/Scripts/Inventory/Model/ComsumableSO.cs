using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu]
public class ComsumableSO : ItemSO, IDestroyableItem, IItemAction
{
    [SerializeField] private List<ModifierData> modifiers = new();
    public string ActionName => "Consume";

    public bool PerformAction(GameObject character, List<ItemParameter> itemState = null)
    {
        foreach (ModifierData data in modifiers)
        {
            data.statMod.AffectCharacter(character, data.value);
        }
        return true;
    }
}

