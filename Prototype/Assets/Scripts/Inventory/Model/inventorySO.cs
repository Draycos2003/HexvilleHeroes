using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu (menuName = "Scriptable object/inventory")]
public class inventorySO : ScriptableObject
{
    [SerializeField] private List<InventoryItem> inventoryItems;

    [SerializeField] public int Size { get; private set; } = 10;

    public event Action<Dictionary<int, InventoryItem>> InventoryChanged;

    public void Initialize()
    {
        inventoryItems = new List<InventoryItem>();
        for(int i = 0; i < Size; i++)
        {
            inventoryItems.Add(InventoryItem.GetEmptyItem());
        }
    }

    public int AddItem(ItemSO item, int quantity, List<ItemParameter> itemState = null)
    {
        if (InventoryFull())
        {
            Debug.Log("INVENTORY FULL");
            return 0;
        }
        if (!item.isStackable)
        {
            Debug.Log("NO STACK");
            quantity -= AddNonStackableItem(item, 1, itemState);
        }
        else if (item.isStackable)
        {
            Debug.Log("STACK");
            quantity = AddStackableItem(item, quantity); 
        }
        InformAbountChange();
        return quantity;
    }

    private int AddNonStackableItem(ItemSO item, int quantity, List<ItemParameter> itemState = null)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].isEmpty)
            {
                inventoryItems[i] = new InventoryItem
                {
                    item = item,
                    quantity = quantity,
                    itemState = new List<ItemParameter>(itemState == null ? item.defaultParameterList : itemState)
                };
                return quantity;
            }
        }
        return 0;
    }

    private bool InventoryFull() // Had to change the way this works since when hitting last slot, regardless of stack, it would return full inventory. 
    {
        foreach (var slot in inventoryItems)
        {
            if (slot.isEmpty)
                return false;

            if (slot.item.isStackable && slot.quantity < slot.item.maxStack)
                return false;
        }

        return true;
    }

    private int AddStackableItem(ItemSO item, int quantity)
    {
        // Fill existing stacks
        for (int i = 0; i < inventoryItems.Count && quantity > 0; i++)
        {
            var slot = inventoryItems[i];
            if (slot.isEmpty || slot.item != item)
                continue;

            int space = item.maxStack - slot.quantity;
            if (space <= 0)
                continue;

            int toAdd = Mathf.Min(space, quantity);
            inventoryItems[i] = slot.ChangeQuantity(slot.quantity + toAdd);
            quantity -= toAdd;
        }

        // New stack in empty slot
        for (int i = 0; i < inventoryItems.Count && quantity > 0; i++)
        {
            if (!inventoryItems[i].isEmpty)
                continue;

            int toAdd = Mathf.Min(item.maxStack, quantity);
            AddNonStackableItem(item, toAdd);
            quantity -= toAdd;
        }

        InformAbountChange();

        return quantity;
    }

    public Dictionary<int, InventoryItem> GetCurrentInventoryState()
    {
        Dictionary<int, InventoryItem> returnValue = new Dictionary<int, InventoryItem>();

        for (int i = 0; i < inventoryItems.Count ; i++)
        {
            if (inventoryItems[i].isEmpty)
                continue;
            returnValue[i] = inventoryItems[i];
        }
        return returnValue;
    }

    public InventoryItem GetItemAt(int itemIndex)
    {
        return inventoryItems[itemIndex];
    }

    public void SwapItems(int itemIndex_1, int itemIndex_2)
    {
        (inventoryItems[itemIndex_2], inventoryItems[itemIndex_1]) = (inventoryItems[itemIndex_1], inventoryItems[itemIndex_2]);
        InformAbountChange();
    }

    private void InformAbountChange()
    {
        InventoryChanged?.Invoke(GetCurrentInventoryState());
    }

    public void RemoveItem(int itemIndex, int amount)
    {
        if(inventoryItems.Count > itemIndex){
            if (inventoryItems[itemIndex].isEmpty)
                return;
            int remainder = inventoryItems[itemIndex].quantity - amount;
            if(remainder <= 0)
                inventoryItems[itemIndex] = InventoryItem.GetEmptyItem();
            else
                inventoryItems[itemIndex] = inventoryItems[itemIndex].ChangeQuantity(remainder);
            InformAbountChange();
        }
    }
}

[Serializable]
public struct InventoryItem
{
    public int quantity;
    public ItemSO item;
    public List<ItemParameter> itemState;
    public bool isEmpty => item == null;

    public InventoryItem ChangeQuantity(int newQuantity)
    {
            return new InventoryItem
            {
                item = this.item,
                quantity = newQuantity,
                itemState = new List<ItemParameter>(this.itemState)
            };
    }

    public static InventoryItem GetEmptyItem()
        => new InventoryItem
        {
            item = null, 
            quantity = 0,
            itemState = new List<ItemParameter>()
        };
}