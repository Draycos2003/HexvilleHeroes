using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class playerAttack : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private float attackCooldown;

    public bool isAttacking = false;
    public bool canAttack = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnAttack1(InputValue input)
    {
        if (canAttack)
        {
            isAttacking = true;
            canAttack = false;
            animator.SetTrigger("attack1");
            StartCoroutine(resetCooldown());
        }
    }

    public void OnAttack2(InputValue input)
    {
        if (canAttack)
        {
            isAttacking = true;
            canAttack = false;
            animator.SetTrigger("attack2");
            StartCoroutine(resetCooldown());
        }
    }

    IEnumerator resetCooldown()
    {
        StartCoroutine(resetAttackBool());
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    IEnumerator resetAttackBool()
    {
        yield return new WaitForSeconds(1.0f);
        isAttacking = false;
    }
}
