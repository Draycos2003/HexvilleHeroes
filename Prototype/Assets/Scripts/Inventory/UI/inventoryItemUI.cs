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

    public event Action<inventoryItemUI> OnItemClicked, OnItemDropped,
        OnItemBeginDrag, OnItemEndDrag, OnRightMouseBtnClick;

    private bool empty = true;

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
        borderImage.enabled = false; ;
    }

    public void Select()
    {
        borderImage.enabled = true;
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
