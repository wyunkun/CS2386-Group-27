using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleporter : MonoBehaviour
{
    [Header("Enemy Spawn")]
    public GameObject[] enemyPrefabs;
    public int enemySpawnCount = 3;
    public Transform[] spawnPoints;

    private static List<Teleporter> allPortals = new List<Teleporter>();
    private static HashSet<Teleporter> activatedPortals = new HashSet<Teleporter>();

    private bool playerInRange = false;
    private bool hasBeenUsed = false;

    void OnEnable()
    {
        if (allPortals.Count == 0)
            activatedPortals.Clear();
        allPortals.Add(this);
    }

    void OnDisable()
    {
        allPortals.Remove(this);
    }

    void Update()
    {
        if (playerInRange && !hasBeenUsed && Input.GetKeyDown(KeyCode.E))
        {
            UsePortal();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    void UsePortal()
    {
        hasBeenUsed = true;
        activatedPortals.Add(this);

        SpawnEnemies();

        List<Teleporter> others = new List<Teleporter>();
        foreach (Teleporter p in allPortals)
        {
            if (p != this)
                others.Add(p);
        }

        if (others.Count > 0)
        {
            Teleporter destination = others[Random.Range(0, others.Count)];
            TeleportPlayer(destination);
        }

        if (activatedPortals.Count >= allPortals.Count)
        {
            LevelManager levelManager = FindFirstObjectByType<LevelManager>();
            if (levelManager != null)
                levelManager.LevelBeat();
        }
    }

    void TeleportPlayer(Teleporter destination)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = destination.transform.position + Vector3.up * 1.5f;

            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = Vector3.zero;
        }
    }

    void SpawnEnemies()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

        for (int i = 0; i < enemySpawnCount; i++)
        {
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            Vector3 spawnPos;
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                spawnPos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
            }
            else
            {
                Vector2 offset = Random.insideUnitCircle * 3f;
                spawnPos = transform.position + new Vector3(offset.x, 0f, offset.y);
            }

            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }
}