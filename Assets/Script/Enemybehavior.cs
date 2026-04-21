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
    private Animator animator;
    private Rigidbody rb;
    private float attackTimer = 0f;
    private int hitCount = 0;
    private bool isAttacking = false;

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

        animator = GetComponentInChildren<Animator>();

        rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        if (attackTimer > 0f)
            attackTimer -= Time.fixedDeltaTime;

        float distance = Vector3.Distance(transform.position, player.position);
        bool inRange = distance <= detectionDistance && PlayerHealth.isAlive;
        bool canMove = inRange && distance > minDistance && !isAttacking;

        if (animator != null)
        {
            animator.SetBool("isWalking", canMove);
            animator.SetBool("isAttacking", isAttacking);
        }

        if (canMove)
            MoveTowardsPlayer();

        if (inRange && distance <= minDistance && attackTimer <= 0f && !isAttacking)
            Attack();
    }

    void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position);
        direction.y = 0f;
        direction.Normalize();

        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);

        if (direction != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 10f * Time.fixedDeltaTime);
    }

    void Attack()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        playerHealth.TakeDamage(damageToPlayer);
        hitCount++;
        attackTimer = attackCooldown;
        isAttacking = true;

        if (hitSound != null)
            audioSource.PlayOneShot(hitSound, soundVolume);

        Invoke(nameof(StopAttack), 1f);

        if (hitCount >= maxHits)
        {
            CancelInvoke();
            Destroy(gameObject);
        }
    }

    void StopAttack()
    {
        isAttacking = false;
    }
}