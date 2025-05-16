using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarController : MonoBehaviour
{
    [Header("HP Bar")]
    [SerializeField] private Transform healthBarFill;

    [Header("Shield Bar")]
    [SerializeField] private Transform shieldBarFill;

    [Header("HP Text")]
    [SerializeField] private TMP_Text hpText;

    private enemyAINew enemy;
    private playerController player;
    private int maxHP;
    private int maxShield;
    private Vector3 hporigFillScale;
    private Vector3 shieldorigFillScale;
    private float lerpSpeed = 5f;

    void Awake()
    {
        // figure out if we're on the Player or an Enemy via tag
        if (gameObject.CompareTag("Player"))
        {
            player = GetComponent<playerController>();
            if (player == null)
                Debug.LogError($"HealthBarController on {name}: tagged Player but no playerController!", this);
        }
        else if (gameObject.CompareTag("Enemy"))
        {
            enemy = GetComponent<enemyAINew>();
            if (enemy == null)
                Debug.LogError($"HealthBarController on {name}: tagged Enemy but no enemyAI!", this);
        }
        else
        {
            Debug.LogError($"HealthBarController on {name}: wrong tag (must be Player or Enemy)", this);
            enabled = false;
            return;
        }

        if (healthBarFill != null)
            hporigFillScale = healthBarFill.localScale;
        else
            Debug.LogError("HealthBarController: healthBarFill not assigned!", this);

        if (shieldBarFill != null)
            shieldorigFillScale = shieldBarFill.localScale;
        else
            Debug.LogError("HealthBarController: shieldBarFill not assigned!", this);
    }

    void Start()
    {
        // Grab max HP for the found object
        if (enemy != null)
        {
            maxHP = enemy.currentHP;
            maxShield = enemy.currentShield;
        }
        else if (player != null)
        {
            maxHP = player.HPOrig;
            maxShield = player.ShieldOrig;
        }
        else
        {
            Debug.LogError("HealthBarController: no enemyAI or playerController found", this);
            enabled = false;
            return;
        }

        // just in case someone set HPOrig to 0 in the Inspector
        if (maxHP <= 0)
        {
            Debug.LogWarning($"HealthBarController on {name}: maxHP ≤ 0, clamping to 1");
            maxHP = 1;
        }
        else if (maxShield <= 0)
        {
            Debug.LogWarning($"HealthBarController on {name}: maxShield ≤ 0, clamping to 1");
            maxShield = 1;
        }

        // initial
        if (hpText != null)
        hpText.text = $"{maxHP + maxShield}/{maxHP + maxShield}";
    }
    void LateUpdate()
    {

        if (healthBarFill == null || shieldBarFill == null) return;

        int currentHP = 0;
        int currentShield = 0;

        if (gameObject.CompareTag("Player"))
        {
            currentHP = player.HPOrig;
            currentShield = player.ShieldOrig;
        }
        else if (gameObject.CompareTag("Enemy"))
        {
            currentHP = enemy.currentHP;
            currentShield = enemy.currentShield;
        }

        // Update shield bar only if shield > 0
        if (shieldBarFill != null && currentShield > 0)
        {
            float shieldPct = Mathf.Clamp01((float)currentShield / maxShield);
            Vector3 shieldTargetScale = shieldorigFillScale;
            shieldTargetScale.x *= shieldPct;
            shieldBarFill.localScale = Vector3.Lerp(
                shieldBarFill.localScale,
                shieldTargetScale,
                Time.deltaTime * lerpSpeed
            );
        }

        // Only start updating HP bar visually when shield is depleted
        if (currentShield <= 0)
        {
            if (shieldBarFill != null)
            {
                Vector3 flat = shieldBarFill.localScale;
                flat.x = 0f;
                shieldBarFill.localScale = flat;
            }
            float hpPct = Mathf.Clamp01((float)currentHP / maxHP);
            Vector3 hpTargetScale = hporigFillScale;
            hpTargetScale.x *= hpPct;
            healthBarFill.localScale = Vector3.Lerp(
                healthBarFill.localScale,
                hpTargetScale,
                Time.deltaTime * lerpSpeed
            );
        }

        // Always update the text so player sees real HP count
        if (hpText != null)
            hpText.text = $"{currentHP + currentShield}/{maxHP + maxShield}";
    }
}
