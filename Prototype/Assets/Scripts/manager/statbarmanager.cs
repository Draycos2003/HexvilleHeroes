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

    private enemyAI enemy;
    private playerController player;
    private int maxHP;
    private int maxShield;
    private Vector3 hporigFillScale;
    private Vector3 shieldorigFillScale;
    private float lerpSpeed = 5f;
    private int currentHP = 0;
    private int currentShield = 0;

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
            enemy = GetComponent<enemyAI>();
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
            maxHP = enemy.CurrentHP;
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
            Debug.LogWarning($"HealthBarController on {name}: maxShield ≤ 0");
            maxShield = 0;
            shieldBarFill.gameObject.SetActive(false);
        }

        // initial
        if (hpText != null)
        hpText.text = $"{maxHP + maxShield}/{maxHP + maxShield}";
    }
    void LateUpdate()
    {

        if (healthBarFill == null || shieldBarFill == null) return;


        if (gameObject.CompareTag("Player"))
        {
            currentHP = player.HP;
            currentShield = player.Shield;
        }
        else if (gameObject.CompareTag("Enemy"))
        {
            currentHP = enemy.CurrentHP;
            currentShield = enemy.currentShield;
        }

        if (shieldBarFill != null && currentShield > 0 && maxShield > 0)
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
        else
        {
            // Force shield bar to flat since shield is 0 or invalid
            Vector3 flat = shieldBarFill.localScale;
            flat.x = 0f;
            shieldBarFill.localScale = flat;

            // HP BAR (only once shield is gone)
            if (maxHP > 0)
            {
                float hpPct = Mathf.Clamp01((float)currentHP / maxHP);
                Vector3 hpTargetScale = hporigFillScale;
                hpTargetScale.x *= hpPct;

                healthBarFill.localScale = Vector3.Lerp(
                    healthBarFill.localScale,
                    hpTargetScale,
                    Time.deltaTime * lerpSpeed
                );
            }
            else
            {
                // Fallback if maxHP is invalid
                Vector3 flatHP = healthBarFill.localScale;
                flatHP.x = 0f;
                healthBarFill.localScale = flatHP;
            }
        }

        // Always update Health/Shield text so you see your real HP
        if (hpText != null)
            hpText.text = $"{currentHP + currentShield}/{maxHP + maxShield}";
    }
}
