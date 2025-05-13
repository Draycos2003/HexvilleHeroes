using System;
using System.Collections;
using UnityEngine;

public class playerAttack : MonoBehaviour
{
    [SerializeField] GameObject attackArea = default;
    [SerializeField] float attackRate;

    [SerializeField] int stabDamageAmount;
    [SerializeField] int slashDamageAmount;

    private Animator animator;
    private attackArea area;

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
            attack("attack");
            StartCoroutine(hit(false));
        }
        if (Input.GetButton("Fire2") && attackRate <= attackTimer)
        {
            attack("attack1");
            StartCoroutine(hit(true));
        }
    }

    private void attack(System.String triggerName)
    {
        Debug.Log("Attack!");
        animator.SetTrigger(triggerName);
        if (!attacking)
        {
            attacking = true;
            attackArea.SetActive(attacking);
        }
    }

    private IEnumerator hit(bool stab)
    {
        yield return new WaitForSeconds(attackRate);
        {
            foreach (IDamage dmg in area.Damagables)
            {
                dmg.TakeDamage(stab ? stabDamageAmount : slashDamageAmount);
            }
        }
    }
}
