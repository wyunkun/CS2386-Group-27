using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public GameObject activatedEffect;

    private static List<Teleporter> allPortals = new List<Teleporter>();
    private bool playerNearby = false;
    private bool isActivated = false;

    void OnEnable()
    {
        allPortals.Add(this);
        if (activatedEffect != null)
            activatedEffect.SetActive(false);
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
        {
            ActivatePortal();
        }
        else
        {
            TeleportPlayer();
        }
    }

    void ActivatePortal()
    {
        isActivated = true;

        if (activatedEffect != null)
            activatedEffect.SetActive(true);

        Bunker bunker = FindFirstObjectByType<Bunker>();
        if (bunker != null)
            bunker.SpawnEnemies();

        CheckWinCondition();
    }

    void CheckWinCondition()
    {
        foreach (Teleporter portal in allPortals)
        {
            if (!portal.isActivated)
                return;
        }

        LevelManager lm = FindFirstObjectByType<LevelManager>();
        if (lm != null)
            lm.LevelBeat();
    }

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