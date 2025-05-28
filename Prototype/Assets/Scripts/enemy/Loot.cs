using UnityEngine;

[CreateAssetMenu (menuName ="Scriptable object/loot")]
public class Loot : ScriptableObject
{
    public int dropChance;
    public string lootName;
    public Sprite lootSprite;

    public Loot(int dropChance, string lootName)
    {
        this.dropChance = dropChance;
        this.lootName = lootName;
    }
}
