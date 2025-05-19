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

    public void GetSelectedItem()
    {
        Item recivedItem = iManager.GetSelectedItem(false);
        if (recivedItem != null)
            Debug.Log("Recieved Item");
        else
            Debug.Log("No Item Recieved");
    }

    public void UseSelectedItem()
    {
        Item recivedItem = iManager.GetSelectedItem(true);
        if (recivedItem != null)
            Debug.Log("Recieved Item");
        else
            Debug.Log("No Item Used");
    }
}
