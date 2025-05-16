using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    [Header("HP Bar")]
    [SerializeField] private Transform healthBarFill;

    private enemyAI enemy;
    private playerController player;
    private int maxHP;
    private Vector3 origFillScale;
    private float lerpSpeed = 5f;

    void Awake()
    {
        // Cache references
        if (gameObject.CompareTag("Player"))
        {
            player = GetComponent<playerController>();
        }
        else if (gameObject.CompareTag("Enemy"))
        {
            enemy = GetComponent<enemyAI>();
        }
        else
        {
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

        if (enemy != null)
            maxHP = enemy.currentHP;
        else if (player != null)
        {
            maxHP = player.HPOrig;
        }
        else
        {
            Debug.LogError("HealthBarController: no enemyAI or playerController found", this);
            enabled = false;
            return;
        }

        // guard against zero (just in case)
        if (maxHP <= 0)
        {
            Debug.LogWarning($"HealthBarController on {name}: maxHP ≤ 0, clamping to 1");
            maxHP = 1;
        }
    }

    void LateUpdate()
    {
        if (healthBarFill == null) return;

        // pick current HP
        int currentHP = (enemy != null)
            ? enemy.currentHP
            : player.HPOrig;

        // compute fill percentage
        float pct = Mathf.Clamp01((float)currentHP / maxHP);

        // scale X only
        Vector3 targetScale = origFillScale;
        targetScale.x *= pct;

        // smooth‐lerp
        healthBarFill.localScale = Vector3.Lerp(
            healthBarFill.localScale,
            targetScale,
            Time.deltaTime * lerpSpeed
        );
    }
}
