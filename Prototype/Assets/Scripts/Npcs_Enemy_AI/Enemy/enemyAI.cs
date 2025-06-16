using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class enemyAI : MonoBehaviour, IDamage
{
    #region Enums
    public enum StateType { Patrol, Chase, Attack, Showcase }
    public enum EnemyType { Melee, Range }
    #endregion

    #region Inspector Fields
    [Header("Debug")]
    [SerializeField] public bool enableStateLogs;
    [SerializeField] public bool showPatrolGizmos = false;

    [Header("State Machine")]
    [SerializeField] private StateType initialState;
    [Tooltip("Tick to force a state on next frame")]
    [SerializeField] private bool forceState;
    [SerializeField] private StateType forcedState;


    [Header("Detection")]
    [SerializeField] public Transform target;
    [SerializeField] public float detectionRange;
    [SerializeField] public float attackRange;

    [Header("Chase Settings")]
    [Tooltip("Seconds to keep chasing after losing sight")]
    [SerializeField] public float chaseDuration;

    [Header("Patrol Settings")]
    [SerializeField] public float patrolRadius;
    [SerializeField] public float patrolWaitTime;

    [Header("Movement & Animation")]
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] public Animator anim;
    [SerializeField] public float animTransSpeed;

    #region Rotation
    [Header("Rotation Settings")]
    [SerializeField] private float faceTargetSpeed;
    #endregion

    [Header("Combat")]
    [SerializeField] public EnemyType type;

    [ShowIf("type", EnemyType.Range)]
    [Header("Range Attack")]
    [SerializeField] public Transform shootPos;
    [ShowIf("type", EnemyType.Range)]
    [SerializeField] public GameObject projectile;
    [ShowIf("type", EnemyType.Range)]
    [SerializeField] public float shootRate;

    [ShowIf("type", EnemyType.Melee)]
    [Header("Melee Attack")]
    [SerializeField] public GameObject weapon;
    [ShowIf("type", EnemyType.Melee)]
    [SerializeField] public float attackRate;

    [Header("Sound")]
    [ShowIf("type", EnemyType.Melee)]
    [SerializeField] private AudioClip[] meleeAttackSFX;
    [ShowIf("type", EnemyType.Range)]
    [SerializeField] private AudioClip[] rangedAttackSFX;
    [SerializeField] private float sfxVolume;
    [SerializeField] private float sfxMinDistance;
    [SerializeField] private float sfxMaxDistance;

    [Header("Health & Loot")]
    [SerializeField] public int HP;
    [SerializeField] public int Shield;
    [SerializeField] public Renderer model;
    [SerializeField] public Transform lootSpawnPos;
    #endregion

    #region Public Properties
    public int CurrentHP => HP;
    public int CurrentShield => Shield;

    [HideInInspector] public float DefaultStoppingDistance = 1f;

    #endregion

    #region Internal States
    private StateMachine<enemyAI> stateMachine;
    private StateType currentState;
    private Vector3 originPosition;
    private float shootTimer;
    private float attackTimer;
    private float lostTimer;
    private Color colorOrig;
    private Coroutine flashRoutine;
    public event Action<GameObject> OnDeath;
    private LootBag loot;
    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
        stateMachine = new StateMachine<enemyAI>(this);
    }

    private void Start()
    {
        DefaultStoppingDistance = agent.stoppingDistance;
        loot = GetComponent<LootBag>();
        originPosition = transform.position;
        colorOrig = model != null ? model.material.color : Color.white;
        agent = agent != null ? agent : GetComponent<NavMeshAgent>();
        anim = anim != null ? anim : GetComponent<Animator>();

        currentState = initialState;
        ChangeTo(currentState);
    }

    private void Update()
    {
        if (forceState)
        {
            ChangeTo(forcedState);
            forceState = false;
        }

        shootTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;
        lostTimer += Time.deltaTime;

        float speed = agent.velocity.magnitude;
        anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), speed, Time.deltaTime * animTransSpeed));

        stateMachine.Update();
    }
    #endregion

    #region State Management
    public void ChangeTo(StateType next)
    {
        if (enableStateLogs)
            Debug.Log($"{name}: {currentState} -> {next}");

        currentState = next;
        if (next == StateType.Chase) lostTimer = 0f;

        switch (next)
        {
            case StateType.Patrol: stateMachine.ChangeState(new PatrolState()); break;
            case StateType.Chase: stateMachine.ChangeState(new ChaseState()); break;
            case StateType.Attack: stateMachine.ChangeState(new AttackState()); break;
            case StateType.Showcase: stateMachine.ChangeState(new ShowcaseState()); break;
        }
    }
    #endregion

    #region Checks & Actions
    public bool CanSeePlayer() => target != null && Vector3.Distance(transform.position, target.position) <= detectionRange;
    public bool InAttackRange() => target != null && Vector3.Distance(transform.position, target.position) <= attackRange;

    public void UpdatePath()
    {
        if (agent != null && target != null)
            agent.SetDestination(target.position);
    }

    public void UpdatePathToOrigin()
    {
        if (agent != null)
        {
            agent.stoppingDistance = 0f;
            agent.SetDestination(originPosition);
        }
    }

    public void PerformAttack()
    {
        if (type == EnemyType.Range)
        {
            if (shootTimer >= shootRate)
            {
                anim.SetTrigger("shoot");
                shootTimer = 0f;

                if (rangedAttackSFX != null && rangedAttackSFX.Length > 0)
                {
                    soundFXmanager.instance.PlayRandomSoundFX3DClip(
                        rangedAttackSFX, transform, sfxVolume, sfxMinDistance, sfxMaxDistance
                    );
                }
            }
        }
        else
        {
            if (attackTimer >= attackRate)
            {
                anim.SetTrigger("attack");
                attackTimer = 0f;

                if (meleeAttackSFX != null && meleeAttackSFX.Length > 0)
                {
                    soundFXmanager.instance.PlayRandomSoundFX3DClip(
                        meleeAttackSFX, transform, sfxVolume, sfxMinDistance, sfxMaxDistance
                    );
                }
            }
        }
    }



    public void FaceTarget()
    {
        if (target == null) return;

        Vector3 dir = target.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        float slowFactor = 4f;
        float t = (faceTargetSpeed * Time.deltaTime) / slowFactor;

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, t);
    }
    #endregion

    #region Properties for States
    public float LostTimer => lostTimer;
    public float ChaseDuration => chaseDuration;
    public Vector3 OriginPosition => originPosition;
    #endregion

    #region Weapon, Damage & Death
    public void weaponColOn()
    {
        weapon.GetComponent<Collider>().enabled = true;
    }

    public void weaponColOff()
    {
        weapon.GetComponent<Collider>().enabled = false;
    }

    public void createProjectile()
    {
        if (projectile != null && shootPos != null && target != null)
        {
            GameObject proj = Instantiate(projectile, shootPos.position, Quaternion.identity);

            Vector3 direction = (target.position - shootPos.position).normalized;
            proj.transform.forward = direction;

            Rigidbody rb = proj.GetComponent<Rigidbody>();
            Damage dmg = proj.GetComponent<Damage>();

            if (rb != null && dmg != null)
            {
                rb.linearVelocity = direction * dmg.Speed;
            }
            else
            {
                Debug.LogWarning($"{name}: Missing Rigidbody or Damage component on projectile.");
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (Shield > 0) Shield -= amount;
        else
        {
            HP -= amount;
            if (flashRoutine != null) StopCoroutine(flashRoutine);
            flashRoutine = StartCoroutine(FlashColor(Color.red));
            if (HP <= 0) Die();
        }
    }

    private IEnumerator FlashColor(Color flashColor)
    {
        var rends = GetComponentsInChildren<Renderer>();
        var origs = new Color[rends.Length][];
        for (int i = 0; i < rends.Length; i++)
        {
            var mats = rends[i].materials;
            origs[i] = new Color[mats.Length];
            for (int j = 0; j < mats.Length; j++)
            {
                origs[i][j] = mats[j].color;
                mats[j].color = flashColor;
            }
        }

        yield return new WaitForSeconds(0.05f);

        for (int i = 0; i < rends.Length; i++)
        {
            var mats = rends[i].materials;
            for (int j = 0; j < mats.Length; j++)
                mats[j].color = origs[i][j];
        }
    }

    private void Die()
    {
        OnDeath?.Invoke(gameObject);
        loot?.InstantiateLoot(lootSpawnPos.position);
        Destroy(gameObject);
    }
    #endregion

    #region Collision
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            target = other.transform;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            target = null;
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        if (!showPatrolGizmos) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(originPosition != Vector3.zero ? originPosition : transform.position, patrolRadius);
    }
}
