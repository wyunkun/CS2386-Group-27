using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour
{
    public GameObject activatedEffect;

    private static List<Teleporter> allPortals = new List<Teleporter>();
    private bool playerNearby = false;
    private bool isActivated = false;
    public bool isLevelGate = false;  

    void Awake()
    {
        foreach (Teleporter t in allPortals)
        {
            if (t != this && t.gameObject.name == gameObject.name)
            {
                Destroy(gameObject);
                return;
            }
        }
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        allPortals.Add(this);
    }

    void OnDisable()
    {
        allPortals.Remove(this);
    }

    void Update()
    {
        if (!playerNearby) return;
        if (!Input.GetKeyDown(KeyCode.E)) return;

        if (!isActivated)
            ActivatePortal();
        else if (isLevelGate)
            StartCoroutine(DelayedLevelBeat());
        else
            TeleportPlayer();
    }

    void ActivatePortal()
    {
        isActivated = true;

        if (activatedEffect != null)
            Instantiate(activatedEffect, transform.position, Quaternion.identity);

        Bunker bunker = FindFirstObjectByType<Bunker>();
        if (bunker != null)
            bunker.SpawnEnemies();
    }

    IEnumerator DelayedLevelBeat()
    {
        yield return new WaitForSeconds(1f);
        LevelManager lm = FindFirstObjectByType<LevelManager>();
        if (lm != null)
            lm.LevelBeat();
    }

    /*void CheckWinCondition()
    {
        foreach (Teleporter portal in allPortals)
        {
            if (!portal.isActivated)
                return;
        }

        LevelManager lm = FindFirstObjectByType<LevelManager>();
        if (lm != null)
            lm.LevelBeat();
    }*/

    void TeleportPlayer()
    {
        foreach (Teleporter other in allPortals)
        {
            if (other != this && other.isActivated)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player == null) return;

                player.transform.position = other.transform.position + Vector3.up * 1.5f;

                Rigidbody rb = player.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.linearVelocity = Vector3.zero;

                return;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNearby = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNearby = false;
    }
}