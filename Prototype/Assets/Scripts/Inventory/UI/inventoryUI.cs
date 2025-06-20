using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class inventoryUI : MonoBehaviour
{
    [SerializeField] inventoryItemUI itemPrefab;

    [SerializeField] RectTransform contentPanel;

    [SerializeField] RectTransform quickBuffPanel, shieldPanel, weaponPanel;

    [SerializeField] inventoryItemDescription description;

    [HideInInspector] public List<inventoryItemUI> listOfUIItems = new List<inventoryItemUI>();

    public event Action<int> OnDescriptionRequested, OnItemActionRequested, OnStartDragging, OnItemAction, OnItemDropped;

    public event Action<int, int> OnSwapItems;

    private int currentlyDraggedItmeIndex = -1;

    private void Start()
    {
        //description.ResetDescription();
        ResetAllActionPanels();
    }

    public void InitializeInventoryUI(int inventorySize)
    {
        // instaniate item slots based on given inventory size value
        for (int i = 0; i < inventorySize; i++)
        {
            inventoryItemUI uiItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            uiItem.transform.SetParent(contentPanel);
            listOfUIItems.Add(uiItem);

            uiItem.OnItemClicked += HandleItemSelection;
            uiItem.OnItemBeginDrag += HandleBeginDrag;
            uiItem.OnItemSwap += HandleSwap;
            uiItem.OnItemEndDrag += HandleEndDrag;
            uiItem.OnRightMouseBtnClick += HandleShowItemActions;
            uiItem.OnItemAction += HandleItemAction;
            uiItem.OnItemDrop += HandleItemDrop;
        }

        // add the buff equip slot
        InitializeQuickBuffSlot();

        // add the shield equip slot
        InitializeShieldSlot();

        // add the weapon equip slot
        InitializeWeaponSlot();
    }

    private void InitializeQuickBuffSlot()
    {
        inventoryItemUI equipItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
        equipItem.transform.SetParent(quickBuffPanel);
        listOfUIItems.Add(equipItem);

        equipItem.OnItemClicked += HandleItemSelection;
        equipItem.OnItemBeginDrag += HandleBeginDrag;
        equipItem.OnItemSwap += HandleSwap;
        equipItem.OnItemEndDrag += HandleEndDrag;
        equipItem.OnRightMouseBtnClick += HandleShowItemActions;
        equipItem.OnItemAction += HandleItemAction;
        equipItem.OnItemDrop += HandleItemDrop;
    }

    private void InitializeShieldSlot()
    {
        inventoryItemUI equipItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
        equipItem.transform.SetParent(shieldPanel);
        listOfUIItems.Add(equipItem);

        equipItem.OnItemClicked += HandleItemSelection;
        equipItem.OnItemBeginDrag += HandleBeginDrag;
        equipItem.OnItemSwap += HandleSwap;
        equipItem.OnItemEndDrag += HandleEndDrag;
        equipItem.OnRightMouseBtnClick += HandleShowItemActions;
        equipItem.OnItemAction += HandleItemAction;
        equipItem.OnItemDrop += HandleItemDrop;
    }

    private void InitializeWeaponSlot()
    {
        inventoryItemUI equipItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
        equipItem.transform.SetParent(weaponPanel);
        listOfUIItems.Add(equipItem);

        equipItem.OnItemClicked += HandleItemSelection;
        equipItem.OnItemBeginDrag += HandleBeginDrag;
        equipItem.OnItemSwap += HandleSwap;
        equipItem.OnItemEndDrag += HandleEndDrag;
        equipItem.OnRightMouseBtnClick += HandleShowItemActions;
        equipItem.OnItemAction += HandleItemAction;
        equipItem.OnItemDrop += HandleItemDrop;
    }

    private void HandleItemDrop(inventoryItemUI item)
    {
        int index = listOfUIItems.IndexOf(item);
        if (index == -1)
        {
            return;
        }
        OnItemDropped?.Invoke(index);
        ResetAllActionPanels();
    }

    private void HandleItemAction(inventoryItemUI item)
    {
        int index = listOfUIItems.IndexOf(item);
        if (index == -1)
        {
            return;
        }
        OnItemAction?.Invoke(index);
        ResetAllActionPanels();
    }

    private void HandleItemSelection(inventoryItemUI item)
    {
        int index = listOfUIItems.IndexOf(item);
        if (index == -1)
            return;
        OnDescriptionRequested?.Invoke(index);
    }

    private void HandleBeginDrag(inventoryItemUI item)
    {
        int index = listOfUIItems.IndexOf(item);
        if (index == -1) return;
        currentlyDraggedItmeIndex = index;
        HandleItemSelection(item);
        OnStartDragging(index);
    }

    private void HandleSwap(inventoryItemUI item)
    {
        int index = listOfUIItems.IndexOf(item);
        if (index == -1)
        {
            return;
        }
        OnSwapItems?.Invoke(currentlyDraggedItmeIndex, index);
        HandleItemSelection(item);
    }

    public void ResetDraggedItem()
    {
        currentlyDraggedItmeIndex = -1;
    }

    private void HandleEndDrag(inventoryItemUI item)
    {
        ResetDraggedItem();
    }

    public void UpdateData(int itemIndex, Sprite itemImage, int itemQuantity)
    {
        if(listOfUIItems.Count > itemIndex)
        {
            listOfUIItems[itemIndex].SetData(itemImage, itemQuantity);
        }
    }

    private void HandleShowItemActions(inventoryItemUI item)
    {
        int index = listOfUIItems.IndexOf(item);
        if (index == -1)
        {
            return;
        }
        OnItemActionRequested?.Invoke(index);
    }

    public void ResetSelection()
    {
        //description.ResetDescription(); !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        DeselectAllItems();
    }

    private void DeselectAllItems()
    {
        foreach(inventoryItemUI item in listOfUIItems)
        {
            item.Deselect();
        }
    }

    private void ResetAllActionPanels()
    {
        foreach (inventoryItemUI item in listOfUIItems)
        {
            item.ResetActionPanel();
        }
    }

    public void UpdateDescription(int _itemIndex, Sprite _itemIcon, string _name, string _description)
    {
        //description.SetDescription(_itemIcon, _name, _description); !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        ResetAllActionPanels();
        DeselectAllItems();
        listOfUIItems[_itemIndex].Select();
    }

    public void UpdateActionPanel(int _itemIndex)
    {
        ResetAllActionPanels();
        listOfUIItems[_itemIndex].SetActionPanel();
    }

    internal void ResetAllItems()
    {
       foreach(var item in listOfUIItems)
        {
            item.ResetData();
            item.Deselect();
        }
    }
}
