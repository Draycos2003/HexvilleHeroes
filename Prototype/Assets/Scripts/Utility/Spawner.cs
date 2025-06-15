using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject[] spawnObjects;
    [SerializeField] private int spawnAmount;
    [SerializeField] private float spawnRate = 1f;
    [SerializeField] private Transform[] spawnPositions;

    private int spawnCount;
    private float spawnTimer;
    private bool startSpawning;
    private bool spawnTriggered;

    // track who already died
    private HashSet<GameObject> _deadHandled = new HashSet<GameObject>();

    void Update()
    {
        if (!startSpawning || spawnCount >= spawnAmount) return;
        spawnTimer += Time.deltaTime;
        if (spawnTimer < spawnRate) return;
        spawnTimer = 0f;
        Spawn();
    }

    void OnTriggerEnter(Collider other)
    {
        if (spawnTriggered || !other.CompareTag("Player")) return;
        startSpawning = true;
        gamemanager.instance.updateGameGoal(spawnAmount);
        spawnTriggered = true;
    }

    private void Spawn()
    {
        for (int i = 0; i < spawnPositions.Length && spawnCount < spawnAmount; i++)
        {
            var prefab = spawnObjects[Random.Range(0, spawnObjects.Length)];
            var enemy = Instantiate(prefab, spawnPositions[i].position, spawnPositions[i].rotation);
            spawnCount++;

            var ai = enemy.GetComponent<enemyAI>();
            //if (ai != null)
            //    ai.OnDeath += OnEnemyDeath;  // subscribe a named method
            //else
                Debug.LogWarning($"'{enemy.name}' has no enemyAI.OnDeath event.");
        }
    }

    private void OnEnemyDeath(GameObject enemy)
    {
        // only handle each enemy once
        if (!_deadHandled.Add(enemy))
            return;

        gamemanager.instance.updateGameGoal(-1);

        // unsubscribe so no leaky
        var ai = enemy.GetComponent<enemyAI>();
        //if (ai != null)
        //    ai.OnDeath -= OnEnemyDeath;
    }
}
