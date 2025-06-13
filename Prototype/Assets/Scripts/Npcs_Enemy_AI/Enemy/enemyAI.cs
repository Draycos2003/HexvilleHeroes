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
    [Header("State Machine")]
    [SerializeField] private StateType initialState = StateType.Patrol;
    [Tooltip("Tick to force a state on next frame")]
    [SerializeField] private bool forceState = false;
    [SerializeField] private StateType forcedState = StateType.Patrol;
    [SerializeField] public bool enableStateLogs = true;

    [Header("Detection")]
    [SerializeField] public Transform target;
    [SerializeField] public float detectionRange = 10f;
    [SerializeField] public float attackRange = 2f;

    [Header("Movement and Animation")]
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] public Animator anim;
    [SerializeField] public float animTransSpeed = 10f;

    [Header("Combat")]
    [SerializeField] public EnemyType type;
    [Header("Range Attack")]
    [SerializeField] public Transform shootPos;
    [SerializeField] public GameObject projectile;
    [SerializeField] public float shootRate = 1f;
    [Header("Melee Attack")]
    [SerializeField] public GameObject weapon;
    [SerializeField] public float attackRate = 1f;

    [Header("Health and Loot")]
    [SerializeField] public int HP;
    [SerializeField] public int Shield;
    [SerializeField] public Renderer model;
    [SerializeField] public Transform lootSpawnPos;
    #endregion

    #region Public Properties
    public int CurrentHP => HP;
    public int CurrentShield => Shield;
    #endregion

    #region Internal State
    private StateMachine<enemyAI> stateMachine;
    private StateType currentState;
    private float shootTimer;
    private float attackTimer;
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
        loot = GetComponent<LootBag>();
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
        float speed = agent.velocity.magnitude;
        anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), speed, Time.deltaTime * animTransSpeed));

        stateMachine.Update();
    }
    #endregion

    #region State Management
    public void ChangeTo(StateType next)
    {
        if (enableStateLogs)
            Debug.Log(name + " " + currentState + " -> " + next);

        currentState = next;

        switch (next)
        {
            case StateType.Patrol:
                stateMachine.ChangeState(new PatrolState());
                break;
            case StateType.Chase:
                stateMachine.ChangeState(new ChaseState());
                break;
            case StateType.Attack:
                stateMachine.ChangeState(new AttackState());
                break;
            case StateType.Showcase:
                stateMachine.ChangeState(new ShowcaseState());
                break;
        }
    }
    #endregion

    #region Checks and Actions
    public bool CanSeePlayer() => target != null && Vector3.Distance(transform.position, target.position) <= detectionRange;
    public bool InAttackRange() => target != null && Vector3.Distance(transform.position, target.position) <= attackRange;
    public void UpdatePath() { if (agent != null && target != null) agent.SetDestination(target.position); }
    public void PerformAttack()
    {
        if (type == EnemyType.Range)
        {
            if (shootTimer >= shootRate)
            {
                anim.SetTrigger("shoot");
                shootTimer = 0f;
            }
        }
        else
        {
            if (attackTimer >= attackRate)
            {
                anim.SetTrigger("attack");
                attackTimer = 0f;
            }
        }
    }
    #endregion

    #region Damage and Death
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
}