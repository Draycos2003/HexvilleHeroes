using UnityEngine;

public class demo : MonoBehaviour
{
   public inventorymanager iManager;
    public Item[] itemsToPickup;

    public void PickUpItem(int id)
    {
        bool result = iManager.AddItem(itemsToPickup[id]);
        if (result)
        {
            Debug.Log(itemsToPickup[id].name + " WAS ADDED");
        }
        else
        {
            Debug.Log(itemsToPickup[id].name + " WAS NOT ADDED");
        }
    }
}
