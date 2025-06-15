using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class TeleportPad : MonoBehaviour
{
    public enum TeleportMode { Teleporter, SceneLoader }

    [Header("Mode")]
    public TeleportMode teleportMode;

    [Header("Teleport Settings")]
    [ShowIf("teleportMode", TeleportMode.Teleporter)]
    public Transform targetDestination;

    [ShowIf("teleportMode", TeleportMode.Teleporter)]
    public float forwardOffset = 2f;

    [ShowIf("teleportMode", TeleportMode.Teleporter)]
    public float upwardOffset = 0.1f;

    [ShowIf("teleportMode", TeleportMode.Teleporter)]
    [Header("Cooldown")]
    public float teleportCooldown = 0.25f;

    [ShowIf("teleportMode", TeleportMode.SceneLoader)]
    public int sceneIndexToLoad;

    private Collider col;
    private bool isOnCooldown;

    void Awake()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isOnCooldown)
            return;

        var gm = gamemanager.instance;
        if (gm != null && other.CompareTag("Player"))
        {
            if (gm.GameGoalCount > 0)
            {
                Debug.Log("[TeleportPad] You have not killed all enemies in this room!");
                return;
            }

            gm.SetMatchTime(0);
            StartCoroutine(TeleportRoutine(gm.Player.transform));
        }
    }

    private IEnumerator TeleportRoutine(Transform playerT)
    {
        isOnCooldown = true;

        if (teleportMode == TeleportMode.SceneLoader)
        {
            var pc = playerT.GetComponent<playerController>();
            if (pc != null)
                pc.SetSceneIndex(sceneIndexToLoad);

            SceneManager.LoadScene(sceneIndexToLoad);
            yield break;
        }

        col.enabled = false;
        var cc = playerT.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        Vector3 dest = targetDestination.position
                     + targetDestination.forward * forwardOffset
                     + Vector3.up * upwardOffset;

        playerT.SetPositionAndRotation(dest, targetDestination.rotation);

        if (cc != null) cc.enabled = true;

        yield return new WaitForSeconds(teleportCooldown);
        col.enabled = true;
        isOnCooldown = false;
    }
}
