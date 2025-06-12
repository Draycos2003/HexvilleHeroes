using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class LootBag : MonoBehaviour
{
    public GameObject droppedItemPrefab;
    public List<Loot> lootList = new List<Loot>();


    
   public Loot GetDroppedItem()
   {
        int randNum = Random.Range(1, 101); 
        List<Loot> possibleItems = new List<Loot>();
        foreach(Loot item in lootList)
        {
            if(randNum <= item.dropChance)
            {
                possibleItems.Add(item);
                
            }
            
        }
        if(possibleItems.Count > 0)
        {
            Loot droppedItem = possibleItems[Random.Range(0, possibleItems.Count)];
            return droppedItem;
        }
        return null;

   }

    public void InstantiateLoot(Vector3 spawnPos)
    {
        Loot droppedItem = GetDroppedItem();
        if(droppedItem != null)
        {
            GameObject lootobj = Instantiate(droppedItemPrefab, spawnPos, Quaternion.identity);
            float dropForce = 300f;
            Vector3 dropDir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            lootobj.GetComponent<Rigidbody>().AddForce(dropDir * dropForce);
        }
    }




}
