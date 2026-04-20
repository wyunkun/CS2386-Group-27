using UnityEngine;

public class Bunker : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int spawnCount = 2;

    public void SpawnEnemies()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * 3f;
            Vector3 spawnPos = transform.position + new Vector3(offset.x, 0f, offset.y);
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        }
    }
}