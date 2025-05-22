using UnityEngine;

public class inventorymanager : MonoBehaviour
{
    public static inventorymanager instance;

    public pickupItemStats[] startItems;

    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;
    public GameObject inventoryMenu;
    [Range(1, 5)] public int hotBarSize;

    int selectedSlot = -1;
    [HideInInspector] public int inventorySize;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ChangeSelectedSlot(0);  
        inventorySize = inventorySlots.Length;

        foreach(var item in startItems)
        {
            AddItem(item);
        }
    }

    private void Update()
    {
        if(Input.inputString != null)
        {
            bool isNumber = int.TryParse(Input.inputString, out int number);
            if(isNumber && (number > 0 && number <= hotBarSize))
            {
                ChangeSelectedSlot(number - 1);
            }
        }
        gamemanager.instance.openInventory(inventoryMenu);
    }

    public void ChangeSelectedSlot(int newValue)
    {
        if(selectedSlot >= 0) 
            inventorySlots[selectedSlot].Deselect();

        inventorySlots[newValue].Select();
        selectedSlot = newValue;
    }

    public bool AddItem(pickupItemStats item)
    {
        // check if any slot has the same item with count lower than maximum
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            inventoryItem itemInSlot = slot.GetComponentInChildren<inventoryItem>();
            if (itemInSlot != null && itemInSlot.item == item && itemInSlot.count < itemInSlot.item.maxStack 
                && itemInSlot.item.stackable)
            {
                itemInSlot.count++;
                itemInSlot.RefreshCount();
                return true;
            }
        }

        // find empty slot
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            inventoryItem itemInSlot = slot.GetComponentInChildren<inventoryItem>();
            if (itemInSlot == null)
            {
                SpawnNewItem(item, slot);
                return true;
            }
        }
        return false;
    }

    void SpawnNewItem(pickupItemStats item, InventorySlot slot)
    {
        GameObject newItem = Instantiate(inventoryItemPrefab, slot.transform);
        inventoryItem invItem = newItem.GetComponent<inventoryItem>();
        invItem.InitializeItem(item);
    }

    public pickupItemStats GetSelectedItem(bool use)
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        inventoryItem itemInSlot = slot.GetComponentInChildren<inventoryItem>();
        if (itemInSlot != null)
        {
            pickupItemStats item = itemInSlot.item;

            if (use)
            {
                itemInSlot.count--;
                if (itemInSlot.count <= 0)
                {
                    Destroy(itemInSlot.gameObject);
                }
                else
                {
                    itemInSlot.RefreshCount();
                }
            }
            return item;
        }
        return null;
    }
}
