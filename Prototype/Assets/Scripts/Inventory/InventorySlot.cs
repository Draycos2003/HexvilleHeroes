using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if(transform.childCount == 0)
        {
            inventoryItem item = eventData.pointerDrag.GetComponent<inventoryItem>();
            item.parentAfterDrag = transform;
        }
    }
}
