using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class inventoryItemUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDropHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] Image itemImage;
    [SerializeField] TMP_Text quantityTxt;

    [SerializeField] Image borderImage;
    [SerializeField] GameObject actionPanel;
    [SerializeField] TMP_Text actionTxt;

    [SerializeField] inventorySO playerInv;

    public event Action<inventoryItemUI> OnItemClicked, OnItemDropped,
        OnItemBeginDrag, OnItemEndDrag, OnRightMouseBtnClick;

    private bool empty = true;
    static int itemIndex;

    public void Awake()
    {
        ResetData();
        Deselect();
    }

    public void ResetData()
    {
        itemImage.gameObject.SetActive(false);
        empty = true;
    }

    public void SetData(Sprite sprite, int quantity)
    {
        itemImage.gameObject.SetActive(true);
        itemImage.sprite = sprite;
        quantityTxt.text = quantity + "";
        empty = false;
    }
    public void Deselect()
    {
        borderImage.enabled = false;
        actionPanel.SetActive(false);
    }

    public void Select()
    {
        Debug.Log($"{itemIndex}");
        

        borderImage.enabled = true;
    }

    public static void SetItemIndex(int index)
    {
        itemIndex = index;
    }

    // pass the item being dragged through the mouse input system functions 
    public void OnBeginDrag(PointerEventData data)
    {
       Debug.Log("DRAG");
       if (empty) return;
       OnItemBeginDrag?.Invoke(this);
    }

    public void OnDrop(PointerEventData data)
    {
        OnItemDropped?.Invoke(this);
    }

    public void OnEndDrag(PointerEventData data)
    {
        OnItemEndDrag?.Invoke(this);
    }

    public void OnPointerClick(PointerEventData data)
    {
        if (data.button == PointerEventData.InputButton.Right)
        {
            if (playerInv.inventoryItems[itemIndex].item.IType == ItemSO.ItemType.consumable)
            {
                actionTxt.text = "Use";
            }
            else
            {
                actionTxt.text = "Equip";
            }

            actionPanel.SetActive(true);
            OnRightMouseBtnClick?.Invoke(this);
        }
        else
        {       
            OnItemClicked?.Invoke(this);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
    }
}
