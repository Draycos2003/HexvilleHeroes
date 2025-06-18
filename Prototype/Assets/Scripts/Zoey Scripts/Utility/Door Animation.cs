using UnityEngine;
using System.Collections;

public class doorOpen : MonoBehaviour
{
    #region Inspector Fields

    public enum DoorRequirement
    {
        None,
        KeyObject
    }

    [Header("Key Settings")]
    [Tooltip("Choose 'None' if no key is required, or 'KeyObject' to require a key")]
    [SerializeField] private DoorRequirement doorRequirement = DoorRequirement.None;

    [ShowIf("doorRequirement", DoorRequirement.KeyObject)]
    [Tooltip("Index in PlayerProgression.keys list required to open this door")]
    [SerializeField] private int requiredKeyIndex = 0;

    [Header("Door Settings")]
    [Tooltip("Y rotation when the door is fully open")]
    [SerializeField] private float targetYRotation;

    [Header("Audio Clips (3D)")]
    [SerializeField] private AudioClip openClip;
    [SerializeField] private AudioClip closeClip;
    [Range(0f, 1f)]
    [SerializeField] private float soundVolume;

    [Header("Animation Speed")]
    [Range(0.1f, 3f)]
    [SerializeField] private float animationSpeed;

    [Header("3D Audio Settings")]
    [Range(1f, 5f)]
    [SerializeField] private float audioMinDistance;
    [Range(5f, 30f)]
    [SerializeField] private float audioMaxDistance;
    #endregion

    #region Private Fields
    private float initialYRotation;
    private bool isOpen;
    private bool isAnimating;
    private Coroutine currentCoroutine;
    #endregion

    #region Unity Callbacks
    private void Start()
    {
        initialYRotation = transform.localEulerAngles.y;
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        if (GetComponent<Rigidbody>() == null)
        {
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isAnimating || isOpen || !other.CompareTag("Player")) return;

        // no key needed
        if (doorRequirement == DoorRequirement.None)
        {
            OpenDoor();
            return;
        }

        // look on the root Player object for your PlayerProgression component
        var playerRoot = other.transform.root;
        var prog = playerRoot.GetComponentInChildren<PlayerProgression>();
        if (prog != null && prog.HasKey(requiredKeyIndex))
            OpenDoor();
    }


    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || !isOpen || isAnimating || closeClip == null || soundFXmanager.instance == null)
            return;

        isOpen = false;
        PlayAndAnimate3D(closeClip, targetYRotation, initialYRotation);
    }
    #endregion

    #region Door Animation Methods
    private void OpenDoor()
    {
        isOpen = true;
        PlayAndAnimate3D(openClip, initialYRotation, targetYRotation);
    }

    private void PlayAndAnimate3D(AudioClip clip, float fromY, float toY)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        soundFXmanager.instance.PlaySoundFX3DClip(
            clip,
            transform,
            soundVolume,
            audioMinDistance,
            audioMaxDistance
        );

        float duration = clip.length / animationSpeed;
        currentCoroutine = StartCoroutine(AnimateDoor(fromY, toY, duration));
    }
    #endregion

    #region Coroutines
    private IEnumerator AnimateDoor(float fromY, float toY, float duration)
    {
        isAnimating = true;
        float elapsed = 0f;
        Vector3 startEuler = transform.localEulerAngles;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float y = Mathf.LerpAngle(fromY, toY, t);
            transform.localEulerAngles = new Vector3(startEuler.x, y, startEuler.z);
            yield return null;
        }

        transform.localEulerAngles = new Vector3(startEuler.x, toY, startEuler.z);
        isAnimating = false;
        currentCoroutine = null;
    }
    #endregion
}
