using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class inventoryItemDescription : MonoBehaviour
{
    [SerializeField] Image itemImage;
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text description;

    public void Start()
    {
        ResetDescription();
    }

    public void ResetDescription()
    {
        itemImage.sprite = null;
        title.text = "";
        description.text = "";
    }

    public void SetDescription(Sprite sprite, string itemName, string itemDescription)
    {
        itemImage.sprite = sprite;
        title.text = itemName;
        description.text = itemDescription;
    }
}
