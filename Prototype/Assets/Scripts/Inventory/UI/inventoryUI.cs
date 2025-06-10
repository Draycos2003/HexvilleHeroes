using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class inventoryUI : MonoBehaviour
{
    [SerializeField] inventoryItemUI itemPrefab;

    [SerializeField] inventoryItemUI equipSlot;

    [SerializeField] RectTransform contentPanel;

    [SerializeField] RectTransform equipPanel;

    [SerializeField] inventoryItemDescription description;

    [HideInInspector] public List<inventoryItemUI> listOfUIItems = new List<inventoryItemUI>();

    public event Action<int> OnDescriptionRequested, OnItemActionRequested, OnStartDragging;

    public event Action<int, int> OnSwapItems;

    private int currentlyDraggedItmeIndex = -1;

    private void Start()
    {
        description.ResetDescription();
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
            uiItem.OnItemDropped += HandleSwap;
            uiItem.OnItemEndDrag += HandleEndDrag;
            uiItem.OnRightMouseBtnClick += HandleShowItemActions;
        }

        // add the equip slot
        inventoryItemUI equipItem = Instantiate(equipSlot, Vector3.zero, Quaternion.identity);
        equipItem.transform.SetParent(equipPanel);
        listOfUIItems.Add(equipItem);

        equipItem.OnItemClicked += HandleItemSelection;
        equipItem.OnItemBeginDrag += HandleBeginDrag;
        equipItem.OnItemDropped += HandleSwap;
        equipItem.OnItemEndDrag += HandleEndDrag;
        equipItem.OnRightMouseBtnClick += HandleShowItemActions;
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
        description.ResetDescription();
        DeselectAllItems();
    }

    private void DeselectAllItems()
    {
        foreach(inventoryItemUI item in listOfUIItems)
        {
            item.Deselect();
        }
    }

    public void UpdateDescription(int _itemIndex, Sprite _itemIcon, string _name, string _description)
    {
        description.SetDescription(_itemIcon, _name, _description);
        DeselectAllItems();
        listOfUIItems[_itemIndex].Select();
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
