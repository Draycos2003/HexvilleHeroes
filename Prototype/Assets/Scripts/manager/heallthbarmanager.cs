using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarController : MonoBehaviour
{
    [Header("HP Bar")]
    [SerializeField] private Transform healthBarFill;

    [Header("HP Text")]
    [SerializeField] private TMP_Text hpText;

    private enemyAI enemy;
    private playerController player;
    private int maxHP;
    private Vector3 origFillScale;
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
            origFillScale = healthBarFill.localScale;
        else
            Debug.LogError("HealthBarController: healthBarFill not assigned!", this);
    }

    void Start()
    {
        // Grab max HP for the found object
        if (enemy != null)
            maxHP = enemy.currentHP;
        else // player != null
            maxHP = player.HPOrig;

        // just in case someone set HPOrig to 0 in the Inspector
        if (maxHP <= 0)
        {
            Debug.LogWarning($"HealthBarController on {name}: maxHP ≤ 0, clamping to 1");
            maxHP = 1;
        }

        // initial
        if (hpText != null)
            hpText.text = $"{maxHP}/{maxHP}";
    }

    void LateUpdate()
    {
        if (healthBarFill == null) return;

        // read current HP
        int currentHP = (enemy != null)
            ? enemy.currentHP
            : player.HPOrig;

        // percent full
        float pct = Mathf.Clamp01((float)currentHP / maxHP);

        // compute and lerp the bar
        Vector3 targetScale = origFillScale;
        targetScale.x *= pct;
        healthBarFill.localScale = Vector3.Lerp(
            healthBarFill.localScale,
            targetScale,
            Time.deltaTime * lerpSpeed
        );

        // update HP text
        if (hpText != null)
            hpText.text = $"{currentHP}/{maxHP}";
    }
}
