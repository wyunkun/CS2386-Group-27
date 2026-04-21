using UnityEngine;

public class Bunker : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int spawnCount = 2;
    public float spawnDistance = 3f;
    public float spawnInterval = 10f;

    public float timer = 5f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnEnemies();
        }
    }

    public void SpawnEnemies()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPos = transform.position + transform.forward * spawnDistance;
            spawnPos.x += Random.Range(-1.5f, 1.5f);
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        }
    }
}