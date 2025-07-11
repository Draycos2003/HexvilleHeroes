using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class inventoryController : MonoBehaviour
{
    [SerializeField] inventoryUI invUI;
    [SerializeField] inventoryItemDescription des;
    [SerializeField] inventoryItemUI inventoryItemUI;

    public inventorySO inventoryData;

    public List<InventoryItem> initialItems = new();

    [SerializeField] AudioClip dropClip;
    [SerializeField] AudioSource audioSource;

    private gamemanager gm = gamemanager.instance;

    private int ConsumableSlotIndex;
    private int ShieldSlotIndex;
    private int WeaponSlotIndex;

    public void Start()
    {
        PrepareUI();
        PrepareInventoryData();

        ConsumableSlotIndex = inventoryData.inventoryItems.Count - 1;
        ShieldSlotIndex = inventoryData.inventoryItems.Count - 2;
        WeaponSlotIndex = inventoryData.inventoryItems.Count - 3;
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
        invUI.OnItemAction += HandleItemAction;
        invUI.OnItemDropped += HandleItemDropped;
    }

    private void HandleItemDropped(int itemIndex)
    {
        InventoryItem item = inventoryData.GetItemAt(itemIndex);
        Vector3 dropPos = new Vector3(transform.position.x, transform.position.y, transform.position.z + 3);
        Instantiate(item.item.model, dropPos, transform.rotation);
        DropItem(itemIndex, 1);
    }

    private void HandleItemActionRequest(int itemIndex)
    {
        InventoryItem invItem = inventoryData.GetItemAt(itemIndex);
        invUI.UpdateActionPanel(itemIndex);

        if (invItem.isEmpty)
        {
            inventoryItemUI.ActionTxt = null;
            return;
        }

        if (invItem.item.IType == ItemSO.ItemType.consumable)
        {
            inventoryItemUI.ActionTxt = "Use";
        }
        else
        {
            inventoryItemUI.ActionTxt = "Equip";
        }
    }

    public void HandleItemAction(int itemIndex)
    {

        InventoryItem item = inventoryData.GetItemAt(itemIndex);

        if (item.isEmpty)
            return;
        IDestroyableItem _item = item.item as IDestroyableItem;
        if (_item != null)
        {
            IEquippable eq = item.item as IEquippable;
            if (eq != null)
            {
                int equipSlotIndex = inventoryData.inventoryItems.Count - 1;

                inventoryData.SwapItems(itemIndex, equipSlotIndex);
            }
            else
            {
                inventoryData.RemoveItem(itemIndex, 1);
            }
        }
        IItemAction itemAction = item.item as IItemAction;
        if (itemAction != null)
        {
            itemAction.PerformAction(gameObject, item.itemState);
        }
    }

    private void DropItem(int itemIndex, int quantity)
    {
        inventoryData.RemoveItem(itemIndex, quantity);
        invUI.ResetSelection();
        audioSource.PlayOneShot(dropClip);
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
            itemAction.PerformAction(gameObject, null);
        }
    }

    private void HandleSwapItems(int itemIndex_1, int itemIndex_2)
    {
        inventoryData.SwapItems(itemIndex_1, itemIndex_2);
    }

    private void HandleDescriptionRequest(int itemIndex)
    {
        InventoryItem invItem = inventoryData.GetItemAt(itemIndex);
        string description = PrepareDescription(invItem);
        if (invItem.isEmpty)
        {
            invUI.ResetSelection();
            return;
        }
        ItemSO item = invItem.item;
        invUI.UpdateDescription(itemIndex, item.itemIcon, item.name, description);
    }

    public string PrepareDescription(InventoryItem invItem)
    {
        if (!invItem.isEmpty)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(invItem.item.description);
            sb.AppendLine();
            for (int i = 0; i < invItem.itemState.Count; i++)
            {
                sb.Append($"{invItem.itemState[i].itemParameter.ParameterName}" +
                    $": {invItem.itemState[i].value}");
                sb.AppendLine();
            }
            return sb.ToString();
        }
        return null;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) return;

        if (Input.GetKeyDown(KeyCode.I))
        {
            bool wasOpen = invUI.gameObject.activeSelf;

            if (wasOpen)
            {
                invUI.gameObject.SetActive(false);
                gamemanager.instance.stateUnpause();
            }
            else
            {
                invUI.gameObject.SetActive(true);
                Time.timeScale = 0f;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                gamemanager.instance.setActiveMenu(invUI.gameObject);
                Show();
            }
        }
    }

    public void Show()
    {
        invUI.ResetSelection();

        foreach (var item in inventoryData.GetCurrentInventoryState())
        {
            invUI.UpdateData(item.Key, item.Value.item.itemIcon, item.Value.quantity);
        }
    }
}
