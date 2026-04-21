using UnityEngine;

public class Trampoline : MonoBehaviour
{
    public float jumpForce = 50f;
    public GameObject jumpEffect;

    private bool playerOnTrampoline = false;
    private Rigidbody playerRb;

    void Start()
    {
        if (jumpEffect != null)
            Instantiate(jumpEffect, transform.position + Vector3.up * 0.1f, Quaternion.identity, transform);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnTrampoline = true;
            playerRb = other.GetComponent<Rigidbody>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnTrampoline = false;
            playerRb = null;
        }
    }

    void Update()
    {
        if (!playerOnTrampoline) return;
        if (playerRb == null) return;
        if (!Input.GetKeyDown(KeyCode.Space)) return;

        playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, 0f, playerRb.linearVelocity.z);
        playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}