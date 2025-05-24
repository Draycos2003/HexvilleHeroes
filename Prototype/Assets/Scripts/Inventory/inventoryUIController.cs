using System;
using System.ComponentModel.Design;
using UnityEngine;

public class inventoryUIController : MonoBehaviour
{
    [SerializeField] inventoryUI invUI;
    [SerializeField] inventoryItemDescription des;

    public inventorySO inventoryData;

    public int inventorySize;

    public void Start()
    {
        PrepareUI();
        // inventoryData.Initialize();
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

    }

    private void HandleDragging(int itemIndex)
    {

    }

    private void HandleSwapItems(int itemIndex_1, int itemIndex_2)
    {

    }

    private void HandleDescriptionRequest(int itemIndex)
    {
        InventoryItem invItem = inventoryData.GetItemAt(itemIndex);
        if (invItem.isEmpty)
        {
            invUI.ResetSelection();
            return;
        }
        pickupItemSO item = invItem.item;
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
