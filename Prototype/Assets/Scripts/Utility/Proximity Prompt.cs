using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[RequireComponent(typeof(Collider))]
public class ProximityPrompt : MonoBehaviour
{
    #region Inspector Fields

    public enum PromptType
    {
        None,
        KeyPickup
        // Add more types here as needed
    }

    [Header("Pickup Type")]
    [Tooltip("Select 'KeyPickup' if this prompt should collect a key, or 'None' otherwise")]
    [SerializeField] private PromptType promptType = PromptType.None;

    [ShowIf("promptType", PromptType.KeyPickup)]
    [Header("Key Pickup Settings")]
    [Tooltip("Index in PlayerProgression.keys list to set true on pickup")]
    [SerializeField] private int keyIndex = -1;

    [Header("UI References")]
    [SerializeField] private CanvasGroup promptCanvasGroup;
    [SerializeField] private TextMeshProUGUI primaryPromptTextLabel;
    [SerializeField] private TextMeshProUGUI primarykeyTextLabel;
    [SerializeField] private TextMeshProUGUI secondarykeyTextLabel;
    [SerializeField] private TextMeshProUGUI secondaryPromptTextLabel;

    [Header("Prompt Settings")]
    [SerializeField] private KeyCode primaryKey = KeyCode.E;
    [SerializeField] private string primaryPromptMessage;
    [SerializeField] private KeyCode secondaryKey = KeyCode.None;
    [SerializeField] private string secondaryPromptMessage;

    [Header("Item Info Settings")]
    [SerializeField] private TextMeshProUGUI promptItemNameLabel;
    [SerializeField] private TextMeshProUGUI promptItemDescLabel;
    [SerializeField] private TextMeshProUGUI promptItemAmountLabel;
    [SerializeField] private string promptItemName;
    [SerializeField] private string promptDesc;
    [SerializeField] private string promptAmount;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration;

    [Header("Sight Settings")]
    [SerializeField, Tooltip("The Renderer on this object (or a child) used for sight checks")]
    private Renderer sightRenderer;

    [Header("Events")]
    public UnityEvent onPrimaryPressed;
    public UnityEvent onSecondaryPressed;

    #endregion

    #region Private Fields

    private bool playerInRange;
    private bool isVisible;
    private Coroutine fadeRoutine;

    #endregion

    #region Unity Callbacks

    private void Awake()
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
            sightRenderer = GetComponent<Renderer>() ?? GetComponentInChildren<Renderer>();

        if (sightRenderer == null)
            Debug.LogWarning($"[ProximityPrompt] No Renderer for frustum check on '{name}'");
    }

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            TryShowOrHide();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            TryShowOrHide();
        }
    }

    private void Update()
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
                Destroy(gameObject);
                TryPickup();
            }

            if (secondaryKey != KeyCode.None && Input.GetKeyDown(secondaryKey))
            {
                Debug.Log("Pressed secondary key");
                onSecondaryPressed?.Invoke();
            }
        }
    }

    #endregion

    #region Visibility Methods

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

    #endregion

    #region Coroutines

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

    #endregion

    #region Pickup Methods

    private void TryPickup()
    {
        switch (promptType)
        {
            case PromptType.None:
                // No pickup behavior
                break;
            case PromptType.KeyPickup:
                TryPickupKey();
                break;
        }
    }

    private void TryPickupKey()
    {
        // If keyIndex is negative, do nothing
        if (keyIndex < 0)
            return;

        PlayerProgression prog = Object.FindFirstObjectByType<PlayerProgression>();
        if (prog != null)
        {
            prog.CollectKey(keyIndex);
            Debug.Log($"[Progression] Key {keyIndex} set to true");
            gameObject.SetActive(false); // disable or destroy this object
        }
    }

    #endregion
}
