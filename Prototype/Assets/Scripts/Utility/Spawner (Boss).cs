using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class WaveSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] spawnObjects;
    [SerializeField] private int spawnAmount = 5;
    [SerializeField] private float spawnRate = 1f;
    [SerializeField] private Transform[] spawnPositions;

    [Header("Boss Arena Settings")]
    [Tooltip("Check to seal off the arena when spawning starts")]
    [SerializeField] private bool isBossArena = false;
    [Tooltip("Walls to raise/lower during the boss wave")]
    [SerializeField] private List<GameObject> wallObjects;

    [Header("Gameplay")]
    [Tooltip("Notify game manager of total enemies to defeat")]
    [SerializeField] private bool notifyGameGoal = false;

    private bool spawnTriggered;
    private int spawnCount;
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private int remaining;

    private void Awake()
    {
        // Ensure this collider is a trigger
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        // Hide walls at start if in boss mode
        if (isBossArena)
            wallObjects.ForEach(w => w.SetActive(false));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (spawnTriggered || !other.CompareTag("Player"))
            return;

        spawnTriggered = true;

        if (notifyGameGoal)
            gamemanager.instance.updateGameGoal(spawnAmount);

        // Seal arena
        if (isBossArena)
            wallObjects.ForEach(w => w.SetActive(true));

        StartCoroutine(SpawnAndMonitor());
    }

    private IEnumerator SpawnAndMonitor()
    {
        spawnCount = 0;
        spawnedEnemies.Clear();

        // Spawn loop
        while (spawnCount < spawnAmount)
        {
            foreach (Transform pos in spawnPositions)
            {
                if (spawnCount >= spawnAmount) break;
                GameObject prefab = spawnObjects[UnityEngine.Random.Range(0, spawnObjects.Length)];
                GameObject enemy = Instantiate(prefab, pos.position, pos.rotation);
                spawnCount++;
                spawnedEnemies.Add(enemy);

                // Subscribe to enemy death event
                var ai = enemy.GetComponent<enemyAI>();
                if (ai != null)
                    ai.OnDeath += OnEnemyDeath;
                else
                    Debug.LogWarning($"Enemy '{enemy.name}' missing enemyAI.OnDeath event.");
            }
            yield return new WaitForSeconds(spawnRate);
        }

        // Monitor until all enemies are dead
        remaining = spawnedEnemies.Count;
        while (remaining > 0)
            yield return null;

        // Wave complete, lower walls
        if (isBossArena)
            wallObjects.ForEach(w => w.SetActive(false));
    }

    private void OnEnemyDeath(GameObject enemy)
    {
        remaining--;
    }
}
