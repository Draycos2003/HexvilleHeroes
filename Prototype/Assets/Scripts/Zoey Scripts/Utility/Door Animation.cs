using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class doorOpen : MonoBehaviour
{
    [Header("Door Settings")]
    [Tooltip("Y rotation when the door is fully open")]
    [SerializeField] private float targetYRotation;

    [Header("Audio Clips (3D)")]
    [SerializeField] private AudioClip openClip;
    [SerializeField] private AudioClip closeClip;
    [Range(0f, 1f)][SerializeField] private float soundVolume;

    [Header("Animation Speed")]
    [Range(0.1f, 3f)][SerializeField] private float animationSpeed;

    [Header("3D Audio Settings")]
    [Range(1f, 5f)][SerializeField] private float audioMinDistance;
    [Range(5f, 30f)][SerializeField] private float audioMaxDistance;

    private float initialYRotation;
    private bool isOpen;
    private bool isAnimating;
    private Coroutine currentCoroutine;

    private void Start()
    {
        initialYRotation = transform.localEulerAngles.y;
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOpen && !isAnimating && openClip != null && soundFXmanager.instance != null)
        {
            isOpen = true;
            PlayAndAnimate3D(openClip, initialYRotation, targetYRotation);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isOpen && !isAnimating && closeClip != null && soundFXmanager.instance != null)
        {
            isOpen = false;
            PlayAndAnimate3D(closeClip, targetYRotation, initialYRotation);
        }
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
}
