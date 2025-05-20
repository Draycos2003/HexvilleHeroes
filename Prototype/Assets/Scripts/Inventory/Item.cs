using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum ItemType { melee, ranged, shield, consumable }

public enum ActionType { swing, shoot, block, buff, debuff }

[CreateAssetMenu(menuName = "Scriptable object/item")]
public class Item : ScriptableObject
{
    [Header("Only gameplay")]
    public TileBase tile;
    public ItemType type;
    public ActionType actionType;
    public Vector2Int range = new Vector2Int(5, 4);
    public GameObject physicalObject;

    [Header("Only UI")]
    public string description;
    public bool stackable = true;
    public int maxStack;


    [Header("Both")]
    public Sprite sprite;
}
