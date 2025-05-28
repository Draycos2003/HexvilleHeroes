using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[RequireComponent(typeof(Collider))]
public class ProximityPrompt : MonoBehaviour
{
    public ItemSO item;

    [Header("UI References")]
    [SerializeField] CanvasGroup promptCanvasGroup;
    [SerializeField] TextMeshProUGUI primaryPromptTextLabel;
    [SerializeField] TextMeshProUGUI primarykeyTextLabel;
    [SerializeField] TextMeshProUGUI secondarykeyTextLabel;
    [SerializeField] TextMeshProUGUI secondaryPromptTextLabel;

    [Header("Prompt Settings")]
    [SerializeField] KeyCode primaryKey = KeyCode.E;
    [SerializeField] string primaryPromptMessage;
    [SerializeField] KeyCode secondaryKey = KeyCode.None;
    [SerializeField] string secondaryPromptMessage;

    [Header("Item Info Settings")]
    [SerializeField] TextMeshProUGUI promptItemNameLabel;
    [SerializeField] TextMeshProUGUI promptItemDescLabel;
    [SerializeField] TextMeshProUGUI promptItemAmountLabel;
    [SerializeField] string promptItemName;
    [SerializeField] string promptDesc;
    [SerializeField] string promptAmount;

    [Header("Fade Settings")]
    [SerializeField] float fadeDuration;

    [Header("Sight Settings")]
    [SerializeField, Tooltip("The Renderer on this object (or a child) used for sight checks")]
    Renderer sightRenderer;

    [Header("Events")]
    public UnityEvent onPrimaryPressed;
    public UnityEvent onSecondaryPressed;

    bool playerInRange;
    bool isVisible;
    Coroutine fadeRoutine;

    void Awake()
    {
        var buySell = GetComponent<ItemBuySell>();
        if (buySell != null)
        {
            if (buySell.PurchaseCost > 0)
            {
                primaryPromptMessage = $"Purchase (${buySell.PurchaseCost})";
                primaryPromptTextLabel.text = primaryPromptMessage;
                primaryKey = KeyCode.E;
            }

            if (buySell.SellPrice > 0)
            {
                secondaryPromptMessage = $"Sell (${buySell.SellPrice})";
                secondaryPromptTextLabel.text = secondaryPromptMessage;
                if (secondaryKey == KeyCode.None)
                    secondaryKey = KeyCode.F;
            }
        }
        else
        {
            if (primaryPromptTextLabel != null)
                primaryPromptTextLabel.text = primaryPromptMessage;
            if (secondaryPromptTextLabel != null)
                secondaryPromptTextLabel.text = secondaryPromptMessage;
        }

        if (promptCanvasGroup != null)
            promptCanvasGroup.alpha = 0;

        if (promptItemNameLabel != null) promptItemNameLabel.text = promptItemName;
        if (promptItemDescLabel != null) promptItemDescLabel.text = promptDesc;
        if (promptItemAmountLabel != null) promptItemAmountLabel.text = "x" + promptAmount;

        if (primarykeyTextLabel != null) primarykeyTextLabel.text = primaryKey.ToString();
        if (secondarykeyTextLabel != null) secondarykeyTextLabel.text = secondaryKey.ToString();

        if (sightRenderer == null)
            sightRenderer = GetComponent<Renderer>()
                         ?? GetComponentInChildren<Renderer>();

        if (sightRenderer == null)
            Debug.LogWarning($"[ProximityPrompt] No Renderer for frustum check on '{name}'");
    }

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
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
        bool nowVisible = IsInCameraFrustum();
        if (nowVisible != isVisible)
        {
            isVisible = nowVisible;
            TryShowOrHide();
        }

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
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeCoroutine(target));
    }

    private bool IsInCameraFrustum()
    {
        if (sightRenderer == null || Camera.main == null)
            return false;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        return GeometryUtility.TestPlanesAABB(planes, sightRenderer.bounds);
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
