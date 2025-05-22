using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class TeleportPad : MonoBehaviour
{
    [Header("Where to send the player")]
    [Tooltip("Destination (Object)")]
    [SerializeField] private Transform targetDestination;
    [Tooltip("Destination (Scene)")]
    [SerializeField] private int sceneIndexToLoad;

    [Header("How far out from the door")]
    [Tooltip("Forward offset from the destination")]
    [SerializeField] private float forwardOffset = 2f;
    [Tooltip("Vertical offset from the destination")]
    [SerializeField] private float upwardOffset = 0.1f;

    [Header("Cooldown")]
    [Tooltip("Seconds before you can teleport again")]
    [SerializeField] private float teleportCooldown = 0.25f;

    private Collider col;
    private bool isOnCooldown;

    void Awake()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;

        if (targetDestination == null)
            Debug.LogError("[TeleportPad] No target assigned!", this);
    }

    void OnTriggerEnter(Collider other)
    {
        var gm = gamemanager.instance;

        // Only allow if the collider is a Player AND GameManager is valid
        if (gm != null && other.CompareTag("Player"))
        {
            if (gm.GameGoalCount > 0)
            {
                Debug.Log("[TeleportPad] You have not killed all enemies in this room!");
                return;
            }

            gm.SetMatchTime(0); // Reset the match timer before teleport
            StartCoroutine(TeleportRoutine(gm.Player.transform));
        }
    }

    private IEnumerator TeleportRoutine(Transform playerT)
    {
        isOnCooldown = true;
        
        // If target is scene tag
        if (targetDestination != null && targetDestination.CompareTag("SceneLoader"))
        {
            Debug.Log("[Loading Scene Index " + sceneIndexToLoad);
            SceneManager.LoadScene(sceneIndexToLoad);
            yield break; // Breaks function
        } 
        else
        {
            col.enabled = false;

            // Otherwise do normal teleport logic
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
}
