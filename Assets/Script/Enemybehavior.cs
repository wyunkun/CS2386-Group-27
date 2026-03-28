using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float detectionDistance = 10f;
    public float minDistance = 0.5f;

    [Header("Attack Settings")]
    public int damageToPlayer = 20;
    public float attackCooldown = 1.5f;
    public int maxHits = 5;

    [Header("Audio")]
    public AudioClip hitSound;
    public float soundVolume = 1f;

    private Transform player;
    private AudioSource audioSource;
    private float attackTimer = 0f;
    private int hitCount = 0;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;
        else
            Debug.LogWarning("Enemy: No GameObject with tag 'Player' found!");

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (player == null) return;

        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionDistance && distance > minDistance && PlayerHealth.isAlive)
        {
            Vector3 direction = player.position - transform.position;
            direction.y = 0f;
            direction.Normalize();

            Vector3 newPosition = transform.position + direction * moveSpeed * Time.deltaTime;
            newPosition.y = transform.position.y;
            transform.position = newPosition;

            if (direction != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player")) return;
        if (attackTimer > 0f) return;
        if (!PlayerHealth.isAlive) return;

        PlayerHealth playerHealth = collision.collider.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageToPlayer);
            hitCount++;
            attackTimer = attackCooldown;

            if (hitSound != null)
                audioSource.PlayOneShot(hitSound, soundVolume);

            if (hitCount >= maxHits)
                Destroy(gameObject);
        }
        else
        {
            Debug.LogError("MCA3PlayerHealth component not found on Player!");
        }
    }
}