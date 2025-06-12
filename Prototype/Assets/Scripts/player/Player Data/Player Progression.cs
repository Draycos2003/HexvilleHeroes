using System.Collections.Generic;
using UnityEngine;

public class PlayerProgression : MonoBehaviour
{
    [Header("Key Progression")]
    [SerializeField] private List<bool> keys = new List<bool>();

    [Header("Checkpoint Progression")]
    [Tooltip("List size = number of story checkpoints (e.g. bosses)")]
    [SerializeField] private List<bool> checkpoints = new List<bool>();

    #region Key Collection
    public void CollectKey(int keyIndex)
    {
        if (keyIndex >= 0 && keyIndex < keys.Count)
            keys[keyIndex] = true;
    }

    public bool HasKey(int keyIndex) =>
        keyIndex >= 0 && keyIndex < keys.Count && keys[keyIndex];

    #endregion

    #region Checkpoints
    public void CollectCheckpoint(int checkpointIndex)
    {
        if (checkpointIndex >= 0 && checkpointIndex < checkpoints.Count)
        {
            checkpoints[checkpointIndex] = true;
            Debug.Log($"[Progression] Checkpoint {checkpointIndex} reached");
        }
    }

    public bool HasCheckpoint(int checkpointIndex) =>
        checkpointIndex >= 0 && checkpointIndex < checkpoints.Count
        && checkpoints[checkpointIndex];
    #endregion
}
