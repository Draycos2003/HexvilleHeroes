using UnityEngine;
using System.Collections.Generic;

public class PlayerProgression : MonoBehaviour
{
    [Header("Key Progression")]
    [Tooltip("List size = number of keys; index corresponds to each key/door pair")]
    [SerializeField] private List<bool> keys = new List<bool>();

    public void CollectKey(int keyIndex)
    {
        if (keyIndex >= 0 && keyIndex < keys.Count)
        {
            keys[keyIndex] = true;
            Debug.Log($"[Progression] Key {keyIndex} collected");
        }
    }

    public bool HasKey(int keyIndex)
    {
        return keyIndex >= 0 && keyIndex < keys.Count && keys[keyIndex];
    }
}
