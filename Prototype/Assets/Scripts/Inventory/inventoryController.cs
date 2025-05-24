using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class inventoryController : MonoBehaviour
{
    [SerializeField] inventoryUI invUI;
    [SerializeField] inventoryItemDescription des;

    public inventorySO inventoryData;

    public List<InventoryItem> initialItems = new();

    public void Start()
    {
        PrepareUI();
        PrepareInventoryData();
    }

    private void PrepareInventoryData()
    {
        inventoryData.Initialize();
        inventoryData.InventoryChanged += UpdateUI;
        foreach (InventoryItem item in initialItems)
        {
            if (item.isEmpty)
            {
                continue;
            }
            inventoryData.AddItem(item.item, item.quantity);
        }
    }

    private void UpdateUI(Dictionary<int, InventoryItem> state)
    {
        invUI.ResetAllItems();
        foreach (var item in state)
        {
            invUI.UpdateData(item.Key, item.Value.item.itemIcon, item.Value.quantity);
        }
    }

    private void PrepareUI()
    {
        invUI.InitializeInventoryUI(inventoryData.Size);
        invUI.OnDescriptionRequested += HandleDescriptionRequest;
        invUI.OnSwapItems += HandleSwapItems;
        invUI.OnStartDragging += HandleDragging;
        invUI.OnItemActionRequested += HandleItemActionRequest;
    }

    private void HandleItemActionRequest(int itemIndex)
    {
        InventoryItem item = inventoryData.GetItemAt(itemIndex);
        if (item.isEmpty)
            return;
    }

    private void HandleDragging(int itemIndex)
    {
        InventoryItem item = inventoryData.GetItemAt(itemIndex);
        if (item.isEmpty)
        {
            return;
        }
        IItemAction itemAction = item.item as IItemAction;
        if (itemAction != null)
        {
            itemAction.PerformAction(gameObject);
        }
    }

    private void HandleSwapItems(int itemIndex_1, int itemIndex_2)
    {
        inventoryData.SwapItems(itemIndex_1, itemIndex_2);
    }

    private void HandleDescriptionRequest(int itemIndex)
    {
        InventoryItem invItem = inventoryData.GetItemAt(itemIndex);
        if (invItem.isEmpty)
        {
            invUI.ResetSelection();
            return;
        }
        ItemSO item = invItem.item;
        invUI.UpdateDescription(itemIndex, item.itemIcon, item.name, item.description);
    }

    public void Update()
    {
        // press I to open/close inventory
        if (Input.GetKeyDown(KeyCode.I))
        {
            gamemanager.instance.openInventory(invUI.gameObject);
            Show();
        }
    }

    public void Show()
    {
        invUI.ResetSelection();

        foreach (var item in inventoryData.GetCurrentInventoryState())
        {
            invUI.UpdateData(item.Key, item.Value.item.itemIcon, item.Value.quantity);
        }
        Debug.Log("OPEN");
    }
}
