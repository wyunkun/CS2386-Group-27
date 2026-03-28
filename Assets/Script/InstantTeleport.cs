using UnityEngine;

public class InstantTeleport : MonoBehaviour
{
    public enum TeleportMode
    {
        MoveToTargetPosition,  
        MoveToSpawnPoint,      
        KeepOffset          
    }

    [Header("Target")]
    public InstantTeleport targetTeleporter;
    public Transform spawnPoint;

    [Header("Settings")]
    public string playerTag = "Player";
    public TeleportMode teleportMode = TeleportMode.MoveToSpawnPoint;

    public float cooldown = 0.2f;

    private bool canTeleport = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!canTeleport)
        {
            return;
        }

        if (!other.CompareTag(playerTag))
        {
            return;
        }

        if (targetTeleporter == null)
        {
            return;
        }

        Transform player = other.transform;
        Vector3 destination = GetDestination(player);

        CharacterController cc = player.GetComponent<CharacterController>();
        Rigidbody rb = player.GetComponent<Rigidbody>();

        if (cc != null)
        {
            cc.enabled = false;
            player.position = destination;
            cc.enabled = true;
        }
        else if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = destination;
        }
        else
        {
            player.position = destination;
        }

        StartCoroutine(DisableTeleportTemporarily());
        targetTeleporter.StartCoroutine(targetTeleporter.DisableTeleportTemporarily());
    }

    private Vector3 GetDestination(Transform player)
    {
        switch (teleportMode)
        {
            case TeleportMode.MoveToTargetPosition:
                return targetTeleporter.transform.position;

            case TeleportMode.MoveToSpawnPoint:
                if (targetTeleporter.spawnPoint != null)
                {
                    return targetTeleporter.spawnPoint.position;
                }
                return targetTeleporter.transform.position;

            case TeleportMode.KeepOffset:
                Vector3 offset = player.position - transform.position;
                return targetTeleporter.transform.position + offset;

            default:
                return targetTeleporter.transform.position;
        }
    }

    private System.Collections.IEnumerator DisableTeleportTemporarily()
    {
        canTeleport = false;
        yield return new WaitForSeconds(cooldown);
        canTeleport = true;
    }
}
