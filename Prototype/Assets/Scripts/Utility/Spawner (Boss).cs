using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class WaveSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private int bossCheckpointOffset = 0;
    [SerializeField] private GameObject[] spawnObjects;
    [SerializeField] private int spawnAmount = 5;
    [SerializeField] private float spawnRate = 1f;
    [SerializeField] private Transform[] spawnPositions;

    [Header("Boss Arena Settings")]
    [SerializeField] private bool isBossArena = false;
    [SerializeField] private List<GameObject> wallObjects;

    [Header("Gameplay")]
    [SerializeField] private bool notifyGameGoal = false;

    // runtime
    private bool spawnTriggered;
    private int spawnCount;
    private int remaining;
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private PlayerProgression progression;

    private void Awake()
    {
        // make sure this collider is a trigger
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        // hide walls until wave starts
        if (isBossArena)
            wallObjects.ForEach(w => w.SetActive(false));

        // cache progression component
        progression = Object.FindFirstObjectByType<PlayerProgression>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (spawnTriggered || !other.CompareTag("Player"))
            return;

        spawnTriggered = true;

        // tell game manager how many to expect
        if (notifyGameGoal)
            gamemanager.instance.updateGameGoal(spawnAmount);

        // seal the arena
        if (isBossArena)
            wallObjects.ForEach(w => w.SetActive(true));

        StartCoroutine(SpawnAndMonitor());
    }

    private IEnumerator SpawnAndMonitor()
    {
        spawnCount = 0;
        spawnedEnemies.Clear();

        // spawn loop
        while (spawnCount < spawnAmount)
        {
            foreach (var pos in spawnPositions)
            {
                if (spawnCount >= spawnAmount) break;

                var prefab = spawnObjects[Random.Range(0, spawnObjects.Length)];
                var enemy = Instantiate(prefab, pos.position, pos.rotation);

                spawnCount++;
                spawnedEnemies.Add(enemy);

                var ai = enemy.GetComponent<enemyAI>();
                //if (ai != null)
                //    ai.OnDeath += OnEnemyDeath;
                //else
                    Debug.LogWarning($"Enemy '{enemy.name}' missing enemyAI.OnDeath event.");
            }

            yield return new WaitForSeconds(spawnRate);
        }

        // wait until they're all dead
        remaining = spawnedEnemies.Count;
        while (remaining > 0)
            yield return null;

        // lower walls
        if (isBossArena)
            wallObjects.ForEach(w => w.SetActive(false));

        // mark the progression flag
        if (progression != null)
        {
            int idx = SceneManager.GetActiveScene().buildIndex - 1 + bossCheckpointOffset;
            progression.CollectCheckpoint(idx);
        }

        // re-call win-check now that both conditions are met
        gamemanager.instance.updateGameGoal(0);
    }

    private void OnEnemyDeath(GameObject enemy)
    {
        remaining--;
        gamemanager.instance.updateGameGoal(-1);
    }
}
