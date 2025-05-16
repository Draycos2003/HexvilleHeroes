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
            StartCoroutine(resetCooldown());
        }
    }

    public void OnAttack2(InputValue input)
    {
        if (canAttack)
        {
            isAttacking = true;
            canAttack = false;
            StartCoroutine(resetCooldown());
        }
    }

    IEnumerator resetCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
