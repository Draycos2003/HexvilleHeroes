using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.ProBuilder.MeshOperations;

public class CompanionAttack : MonoBehaviour
{
    [SerializeField]
    private PoolableObject projectile;
    [SerializeField]
    private GameObject companion;


    [SerializeField]
    [Range(0.1f, 1f)]
    private float attackDelay;

    [SerializeField]
    [Range(1,20)]
    private int attackDamage;

    [SerializeField]
    [Range(3, 5f)]
    private float projectileSpeed = 3;

    private Coroutine attackCo;
    private List<enemyAI> enemies = new List<enemyAI>();
    private float dist;
    private ObjectPool pool;

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

            StartCoroutine(MoveAttack(pooledObject, closestEnemy, pool));
        }
    }

    private IEnumerator MoveAttack(PoolableObject projectile, enemyAI currentEnemy, ObjectPool pool)    {

        if(currentEnemy != null)
        {
            Vector3 startPosition = projectile.transform.position;
        
            dist = Vector3.Distance(projectile.transform.position, currentEnemy.transform.position);

            float startingDist = dist;

            while(dist > 0) {
            
                if(currentEnemy != null)
                {
                    projectile.transform.position = Vector3.Lerp(startPosition, currentEnemy.transform.position, 1 - (dist /startingDist));
                }
            

                dist -= Time.deltaTime * projectileSpeed;

                yield return null;

            
            }
        
            if (dist == 0)
            {
                IDamage damage = GetComponentInChildren<IDamage>();
                if (damage != null)
                {
                    damage.TakeDamage(attackDamage);
                    projectile.gameObject.SetActive(false);
                }
            }

            yield return new WaitForSeconds(1f);

            projectile.gameObject.SetActive(false);

        }

    }

    private enemyAI FindClosestEnemy() {
        
        float closestDist = float.MaxValue;
        
        int closestIndex = 0;
        
        for (int i = 0; i < enemies.Count; i++) {

            if (enemies[i] == null) continue;

            float distance = Vector3.Distance(transform.position, enemies[i].transform.position);

                    
            if (distance < closestDist) {

                closestDist = distance;
                closestIndex = i;
                    
            }
            
        }
        return enemies[closestIndex];
    }
}
