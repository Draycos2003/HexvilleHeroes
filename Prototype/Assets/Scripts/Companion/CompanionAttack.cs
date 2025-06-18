using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CompanionAttack : MonoBehaviour
{
    [SerializeField]
    private PoolableObject projectile;

    [SerializeField]
    private GameObject companion;

    [SerializeField]
    [Min(1)]
    private int poolSize;

    [SerializeField]
    [Range(0.1f, 1f)]
    private float attackDelay;

    [SerializeField]
    [Range(1, 20)]
    private int attackDamage;

    [SerializeField]
    [Range(3, 5f)]
    private float projectileSpeed = 3;


    [SerializeField] AudioClip[] attackSound;
    
    [SerializeField]
    [Range(1, 10f)] private int attackSFXVolume;

    private Coroutine attackCo;
    private List<enemyAI> enemies = new List<enemyAI>();

    private ObjectPool pool;

    private void Awake()
    {
        pool = ObjectPool.CreateInstance(projectile, poolSize);
    }


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
                attackCo = null;
            }
        }
    }

    private IEnumerator Attack() {

        WaitForSeconds Wait = new WaitForSeconds(attackDelay);
        
        while (enemies.Count > 0) {

            yield return Wait;

            enemyAI closestEnemy = FindClosestEnemy();

            if (closestEnemy == null) continue; 

            PoolableObject pooledObject = pool.GetObject();
            pooledObject.transform.position = transform.position;

            StartCoroutine(MoveAttack(pooledObject, closestEnemy, pool));
        }
    }

    private IEnumerator MoveAttack(PoolableObject projectile, enemyAI currentEnemy, ObjectPool pool)    {

       while(currentEnemy != null)
       {
            Vector3 target = currentEnemy.transform.position;
            target.y = projectile.transform.position.y;
            projectile.transform.position = Vector3.MoveTowards(
                projectile.transform.position,
                target,
                projectileSpeed * Time.deltaTime );
            projectile.gameObject.SetActive(true);

            if (Vector3.Distance(projectile.transform.position, target) < 0.1)
                break;
                yield return null;
       }

        if (currentEnemy != null)
            currentEnemy.TakeDamage(attackDamage);
            
        projectile.gameObject.SetActive(false);
       

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
