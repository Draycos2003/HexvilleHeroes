using UnityEngine;

public class playerAttack : MonoBehaviour
{
    [SerializeField] GameObject attackArea = default;
    [SerializeField] int attackRate;

    private Animator animator;

    private float attackTimer;
    private bool attacking;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = gamemanager.instance.Player.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer += Time.deltaTime;
        attacking = false;

        if (Input.GetButton("Fire1") && attackRate <= attackTimer)
        {
            attack();
        }
    }

    private void attack()
    {
        Debug.Log("Attack!");
        animator.SetTrigger("attack");
        if (!attacking)
        {
            attacking = true;
            attackArea.SetActive(attacking);
        }
    }
}
