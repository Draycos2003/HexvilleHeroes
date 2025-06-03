using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public float playerX;
    public float playerY;
    public float playerZ;
    public int playerHP;
    public int playerShield;
    public inventorySO currentInv;
    public string currentScene;
}