using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class inventoryItemUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDropHandler, IEndDragHandler, IDragHandler
{
    #region Fields

    [SerializeField] Image itemImage;
    [SerializeField] TMP_Text quantityTxt;

    [SerializeField] Image borderImage;
    [SerializeField] GameObject actionPanel;
    [SerializeField] TMP_Text actionText;

    [SerializeField] inventorySO playerInv;

    public event Action<inventoryItemUI> OnItemClicked, OnItemSwap,
        OnItemBeginDrag, OnItemEndDrag, OnRightMouseBtnClick, OnItemAction, OnItemDrop;

    private bool drop = false;
    private bool empty = true;
    public static string ActionTxt;

    #endregion

    #region Unity Callbacks
    public void Awake()
    {
        ResetData();
        Deselect();
    }

    #endregion

    #region UI/Data Magament

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
    }

    public void Select()
    {
        borderImage.enabled = true;
    }

    public void ResetActionPanel()
    {
        actionPanel.SetActive(false);
    }

    public void SetActionPanel()
    {
        actionPanel.SetActive(true);
    }

    #endregion

    #region Pointer Events

    // pass the item being dragged through the mouse input system functions 
    public void OnBeginDrag(PointerEventData data)
    {
       Debug.Log("DRAG");
       if (empty) return;
       OnItemBeginDrag?.Invoke(this);
    }

    public void OnDrop(PointerEventData data)
    {
        OnItemSwap?.Invoke(this);
    }

    public void OnEndDrag(PointerEventData data)
    {
        OnItemEndDrag?.Invoke(this);
    }

    public void OnPointerClick(PointerEventData data)
    {
        if (data.button == PointerEventData.InputButton.Right)
        {
            actionPanel.SetActive(true);
            OnRightMouseBtnClick?.Invoke(this);
        }
        else
        {
            OnItemClicked?.Invoke(this);
        }

        UpadteActionTxt();
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    #endregion

    #region Helper Functions

    private void UpadteActionTxt()
    {
        actionText.text = ActionTxt;
    }

    public void OnActionPresssed()
    {
        OnItemAction?.Invoke(this);
    }

    public void OnDropItem()
    {
        OnItemDrop?.Invoke(this);
    }

    #endregion
}
