using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


[CreateAssetMenu(menuName = "Scriptable object/item")]
public class pickupItemSO : ScriptableObject
{
    public enum ItemType { melee, ranged, shield, consumable }

    public ItemType IType;
    public int ID => GetInstanceID();

    public TileBase tile;
    public GameObject model;

    [Header("UI")]
    [TextArea] public string description;
    public bool isStackable;
    [Range(10, 24)] public int maxStack;
    public Sprite itemIcon;

    [Header("Other")]
    public ParticleSystem hitFX;
    public AudioClip[] hitSound;
    [Range(0, 1)] public float volume;

    [Header("Weapon")]
    [Range(1, 60)] public int damgageAmount;
    [Range(5, 1000)] public int shootRange;
    [Range(0, 2)] public float shootRate;

    [Header("Consumable")]
    [Range(10, 50)] public int buffAmount;
}
