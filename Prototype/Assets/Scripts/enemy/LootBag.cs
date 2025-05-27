using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class LootBag : MonoBehaviour
{
    public GameObject DroppedItem;
    public List<Loot> lootList = new List<Loot>();


    
   public List<Loot> GetDroppedItem()
   {
        int randNum = Random.Range(1, 101); 
        List<Loot> possibleItems = new List<Loot>();
        foreach(Loot item in lootList)
        {
            if(randNum <= item.dropChance)
            {
                possibleItems.Add(item);
                Instantiate(DroppedItem, GetComponent<Transform>());
                return possibleItems;
            }
            
        }
        return possibleItems;

   }




}
