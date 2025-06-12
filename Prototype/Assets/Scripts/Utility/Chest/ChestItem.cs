using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Chest Loot Table", fileName = "NewChestLootTable")]
public class ChestItem : ScriptableObject
{
    [System.Serializable]
    public struct DropEntry
    {
        [Tooltip("Prefab to spawn")]
        public GameObject prefab;
        [Tooltip("Relative weight for this drop (0 = never, higher = more likely)")]
        [Range(0f, 1f)]
        public float dropChance;
    }

    [Header("Loot Drops")]
    [Tooltip("All non-currency items this chest can drop")]
    public List<DropEntry> lootEntries = new List<DropEntry>();
    [Header("Loot Bonus Drop Chance (0–100%)")]
    [Range(0, 100)]
    public int lootDoubleDropChance = 0;

    [Header("Currency Drops")]
    [Tooltip("All currency prefabs this chest can drop")]
    public List<DropEntry> currencyEntries = new List<DropEntry>();
    [Header("Currency Bonus Drop Chance (0–100%)")]
    [Range(0, 100)]
    public int currencyDoubleDropChance = 0;

    // Generic weighted pick
    private GameObject PickOneFrom(List<DropEntry> list)
    {
        float total = 0f;
        foreach (var item in list) total += item.dropChance;
        if (total <= 0f) return null;

        float roll = Random.Range(0f, total);
        float cum = 0f;
        foreach (var item in list)
        {
            cum += item.dropChance;
            if (roll <= cum && item.prefab != null)
                return item.prefab;
        }
        return list[0].prefab;
    }

    // Pick one loot-item based on lootEntries weights.
    public GameObject GetRandomLoot()
        => PickOneFrom(lootEntries);

    // Pick one currency-item based on currencyEntries weights.
    public GameObject GetRandomCurrency()
        => PickOneFrom(currencyEntries);

    // Should we spawn an extra loot drop
    public bool ShouldDoubleLoot()
        => Random.Range(0, 100) < lootDoubleDropChance;

    // Should we spawn an extra currency drop
    public bool ShouldDoubleCurrency()
        => Random.Range(0, 100) < currencyDoubleDropChance;
}
