using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [Header("Trampoline Settings")]
    public float jumpForce = 20f;

    private bool playerOnTrampoline = false;
    private Rigidbody playerRb;

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
        if (playerOnTrampoline && playerRb != null && Input.GetKeyDown(KeyCode.Space))
        {
            playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, 0f, playerRb.linearVelocity.z);
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}