using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public Image image;
    public Color selectedColor, deselectColor;

    private void Start()
    {
        Deselect();
    }

    public void Select()
    {
        image.color = selectedColor;
        inventoryItem itemInSlot = gameObject.GetComponentInChildren<inventoryItem>();
        if(itemInSlot != null )
        {

        }
    }

    public void Deselect()
    {
        image.color = deselectColor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(transform.childCount == 0)
        {
            inventoryItem item = eventData.pointerDrag.GetComponent<inventoryItem>();
            item.parentAfterDrag = transform;
        }
    }
}
