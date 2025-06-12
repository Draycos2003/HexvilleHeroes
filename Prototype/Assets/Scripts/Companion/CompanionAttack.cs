using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Pool;
using Unity.VisualScripting;
using System.ComponentModel;

public class CompanionAttack : MonoBehaviour
{
    [SerializeField]
    private PoolableObject projectile;

    [SerializeField]
    private GameObject companion;

    [SerializeField]
    [Range(0.1f, 1f)]
    private float attackDelay;

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

            yield return Wait;

            enemyAI closestEnemy = FindClosestEnemy();

            ObjectPool pool = ObjectPool.CreateInstance(projectile, 10);
            PoolableObject pooledObject = pool.GetObject();
            pooledObject.transform.position = transform.position;


            StartCoroutine(MoveAttack(pooledObject, closestEnemy));
        }
    }

    private IEnumerator MoveAttack(PoolableObject projectile, enemyAI enemies)    {

        Vector3 startPosition = projectile.transform.position;

        float dist = Vector3.Distance(projectile.transform.position, enemies.transform.position);

        float startingDist = dist;

        while(dist > 0) {

            projectile.transform.position = Vector3.Lerp(startPosition, enemies.transform.position, 1 - (dist /startingDist));

            dist -= Time.deltaTime * attackDelay;

            yield return null;
        }

        yield return new WaitForSeconds(1f);

        projectile.gameObject.SetActive(false);
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
