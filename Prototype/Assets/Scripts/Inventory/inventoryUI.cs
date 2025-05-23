using System.Collections.Generic;
using UnityEngine;

public class inventoryUI : MonoBehaviour
{
    [SerializeField] inventoryItem itemPrefab;

    [SerializeField] RectTransform contentPanel;

    List<inventoryItem> listOfUIItems = new List<inventoryItem>();

    public void InitializeInventoryUI(int inventorySize)
    {
        for (int i = 0; i < inventorySize; i++)
        {
            inventoryItem uiItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            uiItem.transform.SetParent(contentPanel);
            listOfUIItems.Add(uiItem);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
