using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public abstract class ItemSO : ScriptableObject
{
    public enum ItemType { melee, ranged, shield, consumable }

    public ItemType IType;
    [SerializeField] public int ID => GetInstanceID();

    public GameObject model;

    [Header("UI")]
    [TextArea] public string description;
    public int quantity;
    public bool isStackable;
    [Range(1, 99)] public int maxStack;
    public Sprite itemIcon;

    [Header("Other")]
    public ParticleSystem hitFX;
    public AudioClip[] hitSound;
    [Range(0, 1)] public float volume;

    [field : SerializeField] public List<ItemParameter> defaultParameterList {  get; set; }
}

[Serializable]
public struct ItemParameter : IEquatable<ItemParameter>
{
    public ItemParameterSO itemParameter;
    public float value;

    public bool Equals(ItemParameter other)
    {
        return other.itemParameter == itemParameter;
    }
}

