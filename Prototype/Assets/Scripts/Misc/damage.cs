using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Damage : MonoBehaviour
{
    #region Enums
    public enum DamageType
    {
        Ranged,
        Melee,
        Homing,
        DOT,
        AOE,
        RDOT
    }
    public enum Subtype
    {
        None,
        Slow,
        Chill,
        Freeze,
    }
    #endregion

    #region Debug
    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos;
    #endregion

    #region General Settings
    [Header("General Settings")]
    [SerializeField] private DamageType type;
    #endregion

    #region Subtype Settings
    [Header("Subtype Settings (Debuffs/Effects)")]
    [ShowIf("type", DamageType.Ranged, DamageType.Melee, DamageType.Homing, DamageType.DOT, DamageType.AOE, DamageType.RDOT)]
    [Tooltip("If using a debuff/effect, choose its subtype here")]
    [SerializeField] private Subtype subtype;
    #endregion

    #region References
    [Header("References")]
    [ShowIf("type", DamageType.Ranged, DamageType.Homing, DamageType.RDOT)]
    [SerializeField] private Rigidbody body;
    [ShowIf("type", DamageType.RDOT)]
    [SerializeField] private GameObject obj;

    private playerController playerController;

    #endregion

    #region Damage Properties
    [Header("Damage Properties")]
    public int damageAmount;

    [ShowIf("type", DamageType.Ranged, DamageType.Homing, DamageType.RDOT)]
    [SerializeField] private float speed;
    public float Speed => speed;

    [ShowIf("type", DamageType.Ranged, DamageType.Homing, DamageType.RDOT)]
    [SerializeField] private float destroyTime;

    [ShowIf("type", DamageType.AOE)]
    [SerializeField] private int damageAOE;
    [ShowIf("type", DamageType.AOE)]
    [SerializeField] private float radiusAOE;
    #endregion

    #region DOT / RDOT Settings
    [Header("DOT / RDOT Settings")]
    [ShowIf("type", DamageType.DOT, DamageType.RDOT)]
    [Tooltip("Total duration (sec) of the damage-over-time effect")]
    [SerializeField] private float dotDuration;

    [ShowIf("type", DamageType.DOT, DamageType.RDOT)]
    [Tooltip("How many times to tick damage evenly over the duration")]
    [SerializeField] private int dotMaxTicks;

    [ShowIf("type", DamageType.DOT, DamageType.RDOT)]
    [Tooltip("Damage amount per tick (over time)")]
    [SerializeField] private int dotDamageAmount;
    #endregion

    #region VFX / SFX
    [Header("VFX / SFX")]
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private AudioClip hitSFX;
    [SerializeField] private float sfxVolume;
    #endregion

    private bool isDamaging;

    #region Unity Callbacks
    private void Awake()
    {
        if (type == DamageType.Ranged || type == DamageType.Melee)
            playerController = Object.FindFirstObjectByType<playerController>();
    }

    private void Start()
    {
        if (IsProjectile() && body != null)
        {
            body.linearVelocity = transform.forward * speed;
        }
    }

    private void Update()
    {
        if (type == DamageType.Homing && body != null && gamemanager.instance.Player != null)
        {
            Vector3 dir = (gamemanager.instance.Player.transform.position - transform.position).normalized;
            body.linearVelocity = dir * speed;
        }
    }
    #endregion

    #region Collision Handlers
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        var dmgTarget = other.GetComponent<IDamage>();

        switch (type)
        {
            case DamageType.Ranged:
                if (dmgTarget != null)
                {
                    dmgTarget.TakeDamage(damageAmount);
                    HandleImpactFeedback();
                    StartCoroutine(DebuffOverTime());
                  
                }
                break;

            case DamageType.Melee:
                if (dmgTarget != null)
                {
                    dmgTarget.TakeDamage(damageAmount);
                    HandleImpactFeedback();
                    StartCoroutine(DebuffOverTime());
                }
                break;

            case DamageType.Homing:
                if (dmgTarget != null)
                {
                    dmgTarget.TakeDamage(damageAmount);
                    HandleImpactFeedback();
                    StartCoroutine(DebuffOverTime());
                }
                break;

            case DamageType.DOT:
                if (dmgTarget != null)
                {
                    dmgTarget.TakeDamage(damageAmount);
                    StartCoroutine(DamageOverTime(dmgTarget));
                    HandleImpactFeedback();
                    StartCoroutine(DebuffOverTime());
                }
                break;

            case DamageType.AOE:
                if (gamemanager.instance.Player != null)
                {
                    float dist = Vector3.Distance(
                        transform.position,
                        gamemanager.instance.Player.transform.position
                    );
                    if (dist <= radiusAOE && dmgTarget != null)
                    {
                        dmgTarget.TakeDamage(damageAmount);
                        HandleImpactFeedback();
                        StartCoroutine(DebuffOverTime());
                    }
                }
                break;

            case DamageType.RDOT:
                if (other.CompareTag("Player") && gamemanager.instance.Player != null)
                {
                    var rend = GetComponent<Renderer>();
                    obj.transform.SetParent(gamemanager.instance.Player.transform);

                    if (body != null)
                        body.linearVelocity = Vector3.zero;
                    if (rend != null)
                        rend.enabled = false;

                    dmgTarget?.TakeDamage(damageAmount);
                    StartCoroutine(DamageOverTime(dmgTarget));
                    StartCoroutine(DebuffOverTime());
                    HandleImpactFeedback();

                    return;
                }
                break;

        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger) return;
        var dmg = other.GetComponent<IDamage>();

        if ((type == DamageType.DOT || type == DamageType.RDOT) && dmg != null && !isDamaging)
            StartCoroutine(DamageOverTime(dmg));
    }
    #endregion

    #region Coroutines
    private IEnumerator DamageOverTime(IDamage dmg)
    {
        isDamaging = true;
        string startScene = SceneManager.GetActiveScene().name;

        float interval = dotDuration / Mathf.Max(dotMaxTicks, 1);
        int ticks = 0;
        float elapsed = 0f;

        while (isDamaging)
        {
            if (SceneManager.GetActiveScene().name != startScene ||
                dmg == null ||
                gamemanager.instance == null ||
                gamemanager.instance.Player == null)
                break;

            if (ticks >= dotMaxTicks || elapsed >= dotDuration)
                break;

            dmg.TakeDamage(dotDamageAmount);
            yield return new WaitForSeconds(interval);
            ticks++;
            elapsed += interval;
        }

        isDamaging = false;
    }

    private IEnumerator DebuffOverTime()
    {
        float elapsed = 0f;
        float interval = dotDuration / Mathf.Max(dotMaxTicks, 1);

        while (elapsed < dotDuration)
        {
            switch (subtype)
            {

                case Subtype.Slow:
                    
                    break;
                case Subtype.Chill:
                    
                    break;
                case Subtype.Freeze:
                    
                    break;
                case Subtype.None:
                default:
                    break;
            }

            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }
    }
    #endregion

    #region Utilities
    private bool IsProjectile()
        => type == DamageType.Ranged
        || type == DamageType.Homing
        || type == DamageType.DOT
        || type == DamageType.RDOT;
    #endregion

    #region Impact Feedback
    private void HandleImpactFeedback()
    {
        // Particle effect on hit
        /*
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }
        */

        // Play a hit sound
        /*
        if (hitSFX != null && soundFXmanager.instance != null)
        {
            soundFXmanager.instance.PlaySoundFX3DClip(
                hitSFX,
                transform,
                sfxVolume,
                2f,     // minDistance
                15f     // maxDistance
            );
        }
        */
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        if (!showDebugGizmos) return;
        Gizmos.color = Color.red;

        if (type == DamageType.AOE || type == DamageType.RDOT)
            Gizmos.DrawWireSphere(transform.position, radiusAOE);
    }
    #endregion
}
