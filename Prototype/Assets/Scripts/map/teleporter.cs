using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class TeleportPad : MonoBehaviour
{
    [Header("Where to send the player")]
    [Tooltip("Destination Door")]
    [SerializeField] private Transform targetDestination;

    [Header("How far out from the door")]
    [Tooltip("Forward offset from the destination")]
    [SerializeField] private float forwardOffset = 2f;
    [Tooltip("Vertical offset from the destination")]
    [SerializeField] private float upwardOffset = 0.1f;

    [Header("Cooldown")]
    [Tooltip("Seconds before you can teleport again")]
    [SerializeField] private float teleportCooldown = 0.25f;

    private Collider _col;
    private bool _isOnCooldown;

    void Awake()
    {
        _col = GetComponent<Collider>();
        _col.isTrigger = true;

        if (targetDestination == null)
            Debug.LogError("[TeleportPad] No target assigned!", this);
    }

    void OnTriggerEnter(Collider other)
    {
        var gm = gamemanager.instance;
        if (_isOnCooldown || gm == null || other.gameObject != gm.Player) return;
        StartCoroutine(TeleportRoutine(gm.Player.transform));
    }

    private IEnumerator TeleportRoutine(Transform playerT)
    {
        _isOnCooldown = true;
        _col.enabled = false;

        var cc = playerT.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        Vector3 dest = targetDestination.position
                     + targetDestination.forward * forwardOffset
                     + Vector3.up * upwardOffset;
        playerT.SetPositionAndRotation(dest, targetDestination.rotation);

        if (cc != null) cc.enabled = true;
        yield return new WaitForSeconds(teleportCooldown);

        _col.enabled = true;
        _isOnCooldown = false;
    }
}
