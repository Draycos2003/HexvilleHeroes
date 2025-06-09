using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Pool;
using Unity.VisualScripting;
using System.ComponentModel;

public class CompanionAttack : MonoBehaviour
{
    [SerializeField]
    private GameObject projectile;

    [SerializeField]
    [Range(0.1f, 1f)]
    private float attackDelay = 0.33f;

    [SerializeField]
    private float attackMoveSpeed = 3f;

    [SerializeField] 
    private Companion Companion;

    private Coroutine attackCo;

    private List<enemyAI> enemies = new List<enemyAI>();

    private void OnTriggerEnter(Collider other) {
        
        if (other.TryGetComponent<enemyAI>(out enemyAI currentEnemy)) {
            
            enemies.Add(currentEnemy);

            if (attackCo != null) {

                StopCoroutine(attackCo);
            }
            attackCo = StartCoroutine(Attack());
        }
    }

    private void OnTriggerExit(Collider other) {
        
        if (other.TryGetComponent<enemyAI>(out enemyAI currentEnemy)) {

            enemies.Remove(currentEnemy);
            
            if (enemies.Count == 0) {
                
               StopCoroutine(attackCo);
            }
        }
    }

    private IEnumerator Attack() {

        WaitForSeconds Wait = new WaitForSeconds(attackDelay);
        while (enemies.Count > 0) {

            yield return null;

            enemyAI closestEnemy = FindClosestEnemy();

            Instantiate(projectile, transform.position, transform.rotation);
            StartCoroutine(MoveAttack(projectile, closestEnemy));
        }
    }

    private IEnumerator MoveAttack(GameObject projectile, enemyAI enemies)    {

        Vector3 startPosition = projectile.transform.position;

        float dist = Vector3.Distance(projectile.transform.position, enemies.transform.position);
        float startingDistance = dist;

        while(dist > 0) {

            projectile.transform.position = Vector3.Lerp(startPosition, enemies.transform.position, 1);

            dist -= Time.deltaTime * attackMoveSpeed;
        }
        yield return new WaitForSeconds(1f);
        projectile.gameObject.SetActive(true);
    }

    private enemyAI FindClosestEnemy() {
        float closestDist = float.MaxValue;
        int closestIndex = 0;

        for (int i = 0; i < enemies.Count; i++) {

            float distance = Vector3.Distance(transform.position, enemies[i].transform.position);
            if (distance < closestDist) { 
                closestDist = distance;
                closestIndex = i;
            }
        }
        return enemies[closestIndex];
    }
}
