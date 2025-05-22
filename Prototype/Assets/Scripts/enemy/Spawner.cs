using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject spawnObject;
    [SerializeField] int spawnAmount;
    [SerializeField] int spawnRate;
    [SerializeField] Transform[] spawnPos;

    int spawnCount;
    float spawnTimer;
    bool startSpawning;
    bool spawnTriggered = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (startSpawning)
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= spawnRate && spawnCount < spawnAmount)
            {
                Spawn();
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (spawnTriggered == false)
        {
            if (other.CompareTag("Player"))
            {
                startSpawning = true;
                gamemanager.instance.updateGameGoal(spawnAmount);
                spawnTriggered = true;
            }
        }
    }

    void Spawn()
    {
        for (int arrayPos = 0; arrayPos < spawnPos.Length && arrayPos < spawnAmount; arrayPos++)
        {
            Instantiate(spawnObject, spawnPos[arrayPos].position, spawnPos[arrayPos].rotation);
            spawnCount++;
            spawnTimer = 0;
        }
    }
}
