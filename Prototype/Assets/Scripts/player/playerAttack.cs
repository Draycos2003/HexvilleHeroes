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

    private void Update()
    {
        Attack();
    }

    public void Attack()
    {

        if (canAttack)
        {
            if (Input.GetButton("Fire1"))
            {
                isAttacking = true;
                canAttack = false;
                StartCoroutine(resetCooldown());
            }
        }
           
    }

    IEnumerator resetCooldown()
    {
        yield return new WaitForSeconds(0.1f);
        isAttacking = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
