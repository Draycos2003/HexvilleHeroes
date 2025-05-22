using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[RequireComponent(typeof(Collider))]
public class ProximityPrompt : MonoBehaviour
{
    public pickupItemStats item;

    [Header("UI References")]
    [SerializeField] CanvasGroup promptCanvasGroup;
    [SerializeField] TextMeshProUGUI primaryPromptTextLabel;
    [SerializeField] TextMeshProUGUI primarykeyTextLabel;

    [SerializeField] TextMeshProUGUI secondarykeyTextLabel;
    [SerializeField] TextMeshProUGUI secondaryPromptTextLabel;

    [Header("Prompt Settings")]
    [SerializeField] KeyCode primaryKey = KeyCode.E;
    [SerializeField] string primaryPromptMessage = "Interact";
    [SerializeField] KeyCode secondaryKey = KeyCode.None;
    [SerializeField] string secondaryPromptMessage = "Equip";

    [Header("Item Info Settings")]
    [SerializeField] TextMeshProUGUI promptItemNameLabel;
    [SerializeField] TextMeshProUGUI promptItemDescLabel;
    [SerializeField] TextMeshProUGUI promptItemAmountLabel;
    [SerializeField] string promptItemName = "Apple";
    [SerializeField] string promptDesc = "Gives +10 HP";
    [SerializeField] string promptAmount = "x";

    [Header("Fade Settings")]
    [SerializeField] float fadeDuration = 0.2f;

    [Header("Sight Settings")]
    [Tooltip("The Renderer on this object (or a child) used for sight checks")]
    [SerializeField] Renderer sightRenderer;

    [Header("Events")]
    public UnityEvent onPrimaryPressed;
    public UnityEvent onSecondaryPressed;

    bool playerInRange;
    bool isVisible;
    Coroutine fadeRoutine;

    void Reset()
    {
        // grab the trigger collider (Object must have a collider (preferably a shepere)
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void Awake()
    {

        // hide immediately
        if (promptCanvasGroup != null)
            promptCanvasGroup.alpha = 0;

        // message shown on prompt for keybind(s)
        if (primaryPromptTextLabel != null)
            primaryPromptTextLabel.text = primaryPromptMessage;
        if (secondaryPromptTextLabel != null)
            secondaryPromptTextLabel.text = secondaryPromptMessage;

        // item info blocks
        if (promptItemNameLabel != null)
            promptItemNameLabel.text = promptItemName;
        if (promptItemDescLabel != null)
            promptItemDescLabel.text = promptDesc;
        if (promptItemAmountLabel != null)
            promptItemAmountLabel.text = promptAmount;

        if (primarykeyTextLabel != null)
            primarykeyTextLabel.text = primaryKey.ToString();
        if (secondarykeyTextLabel != null)
            secondarykeyTextLabel.text = secondaryKey.ToString();


        // no Rendered assigned, will find the objects automatically
        if (sightRenderer == null)
            sightRenderer = GetComponent<Renderer>()
                         ?? GetComponentInChildren<Renderer>();

        if (sightRenderer == null)
            Debug.LogWarning($"[ProximityPrompt] No Renderer found for frustum check on '{name}'");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            TryShowOrHide();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            TryShowOrHide();
        }
    }

    void Update()
    {
        // re-check visibility each frame
        bool nowVisible = IsInCameraFrustum();
        if (nowVisible != isVisible)
        {
            isVisible = nowVisible;
            TryShowOrHide();
        }

        // handle input only when both conditions true
        if (playerInRange && isVisible)
        {
            if (Input.GetKeyDown(primaryKey))
            {
                Debug.Log("Pressed primary key");
                onPrimaryPressed?.Invoke();
            }

            if (secondaryKey != KeyCode.None && Input.GetKeyDown(secondaryKey))
            {
                Debug.Log("Pressed secondary key");
                onSecondaryPressed?.Invoke();
            }
        }
    }

    private void TryShowOrHide()
    {
        float target = (playerInRange && isVisible) ? 1f : 0f;
        FadeTo(target);
    }

    private bool IsInCameraFrustum()
    {
        if (sightRenderer == null || Camera.main == null)
            return false;

        // build frustum planes for the main camera
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        // test the renderer's bounding box
        return GeometryUtility.TestPlanesAABB(planes, sightRenderer.bounds);
    }

    private void FadeTo(float target)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeCoroutine(target));
    }

    private IEnumerator FadeCoroutine(float target)
    {
        float start = promptCanvasGroup.alpha;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            promptCanvasGroup.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }
        promptCanvasGroup.alpha = target;
        fadeRoutine = null;
    }
}
