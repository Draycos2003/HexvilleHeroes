using UnityEngine;

public class rogueAI : enemyAI
{
    [Header("Rogue Settings")]
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] Transform shootPOS;
    [SerializeField] int meleeDmg;
    [SerializeField] float meleeRange;
    [SerializeField] float rangeAttackRange;

    [SerializeField] float meleCooldownTimer;
    [SerializeField] float rangeCooldownTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();


    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (target != null)
        {
            float playerDist = Vector3.Distance(transform.position, target.position);
            if (playerDist <= rangeAttackRange)
            {
                if (playerDist <= meleeRange)
                {
                    meleeAttack();

                }
                else
                {
                    rangedAttack();
                }

            }

        }

    }

    void meleeAttack()
    {
        if (meleCooldownTimer <= 0)
        {
            Debug.Log("melee attack");

            float playerDist = Vector3.Distance(transform.position, target.position);

            if (playerDist <= meleeRange)
            {
                // I want to move close to do a melee attack.
                playerController player = target.GetComponent<playerController>();
                if (player != null)
                {

                    player.TakeDamage(meleeDmg);
                    Debug.Log("Player took " + meleeDmg + " meleedamage");
                }
                meleCooldownTimer = shootRate;

            }
        }
    }

    void rangedAttack()
    {
        if (rangeCooldownTimer <= 0 && arrowPrefab != null && shootPOS != null)
        {
            GameObject arrow = Instantiate(arrowPrefab, shootPOS.position, Quaternion.LookRotation(target.position - shootPOS.position));

            Damage arrowDmg = arrow.GetComponent<Damage>();
            if (arrowDmg != null)
            {
                Debug.Log("Arrow delt " + arrowDmg.damageAmount + " damage");
            }

            rangeCooldownTimer = shootRate;
        }


    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }
}