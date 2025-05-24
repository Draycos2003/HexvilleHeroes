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

    public int AddItem(ItemSO item, int quantity)
    {
        if (InventoryFull())
        {
            Debug.Log("INVENTORY FULL");
            return 0;
        }
        if (!item.isStackable)
        {
            Debug.Log("NO STACK");
            quantity -= AddNonStackableItem(item, 1);
        }
        else if (item.isStackable)
        {
            Debug.Log("STACK");
            quantity = AddStackableItem(item, quantity); 
        }
        InformAbountChange();
        return quantity;
    }

    private int AddNonStackableItem(ItemSO item, int quantity)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].isEmpty)
            {
                inventoryItems[i] = new InventoryItem
                {
                    item = item,
                    quantity = quantity,
                };
                return quantity;
            }
        }
        return 0;
    }

    private bool InventoryFull() => inventoryItems.Any(item => item.isEmpty) == false;

    private int AddStackableItem(ItemSO item, int quantity)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].isEmpty)
                continue;
            if (inventoryItems[i].item == item)
            {
                int takableAmount = inventoryItems[i].item.maxStack -inventoryItems[i].quantity;
                if(quantity > takableAmount)
                {
                    inventoryItems[i] = inventoryItems[i].ChangeQuantity(inventoryItems[i].item.maxStack);
                    quantity -= takableAmount;
                }
                else
                {
                    inventoryItems[i] = inventoryItems[i].ChangeQuantity(inventoryItems[i].quantity + quantity);
                    InformAbountChange();
                    return 0;
                }

                return quantity;
            }
        }
        while (quantity > 0 && InventoryFull() == false)
        {
            int newQuantity = Mathf.Clamp(quantity, 0, item.maxStack);
            quantity -= newQuantity;
            AddNonStackableItem(item, newQuantity);
        }
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
}

[Serializable]
public struct InventoryItem
{
    public int quantity;
    public ItemSO item;
    public bool isEmpty => item == null;

    public InventoryItem ChangeQuantity(int newQuantity)
    {
            return new InventoryItem
            {
                item = this.item,
                quantity = newQuantity,
            };
    }

    public static InventoryItem GetEmptyItem()
        => new InventoryItem
        {
            item = null, 
            quantity = 0,
        };
}