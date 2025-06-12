using UnityEngine;

public class ItemBuySell : MonoBehaviour
{
    [SerializeField] private playerController player;
    [SerializeField] private int itemCost;
    [SerializeField] private int sellValue;

    public int PurchaseCost => itemCost;
    public int SellPrice => sellValue;

    public void AttemptPurchase()
    {
        if (player.RemoveGold(itemCost))
            Debug.Log($"Bought item for {itemCost} gold. You now have {player.Gold}.");
        else
            Debug.Log("Not enough gold!");
    }

    public void AttemptSell()
    {
        player.AddGold(sellValue);
        Debug.Log($"Sold item for {sellValue} gold. You now have {player.Gold}.");
    }
}
