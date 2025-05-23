using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject[] spawnObject;
    [SerializeField] int spawnAmount;
    [SerializeField] int spawnRate;
    [SerializeField] Transform[] spawnPos;

    int spawnCount;
    float spawnTimer;
    bool startSpawning;
    bool spawnTriggered;

    void Update()
    {
        if (startSpawning)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnRate && spawnCount < spawnAmount)
            {
                Spawn();
                spawnTimer = 0f;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!spawnTriggered && other.CompareTag("Player"))
        {
            startSpawning = true;
            gamemanager.instance.updateGameGoal(spawnAmount);
            spawnTriggered = true;
        }
    }

    void Spawn()
    {
        for (int i = 0; i < spawnPos.Length && spawnCount < spawnAmount; i++)
        {
            // Pick a random prefab from the array
            GameObject prefab = spawnObject[Random.Range(0, spawnObject.Length)];
            Instantiate(prefab, spawnPos[i].position, spawnPos[i].rotation);
            spawnCount++;
        }
    }
}
