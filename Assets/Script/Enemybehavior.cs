using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float detectionDistance = 10f;
    public float stopDistance = 1.5f;

    [Header("Attack Settings")]
    public int damageToPlayer = 20;
    public float attackDistance = 2f;
    public float attackCooldown = 1.5f;
    public int maxHits = 5;
    public float attackAnimTime = 1f;

    [Header("Audio")]
    public AudioClip hitSound;
    public float soundVolume = 1f;

    private Transform player;
    private AudioSource audioSource;
    private Animator animator;
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
    }

    void Update()
    {
        if (player == null || !PlayerHealth.isAlive) return;

        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;
        float distance = toPlayer.magnitude;

        bool inRange = distance <= detectionDistance;
        bool canMove = inRange && distance > stopDistance && !isAttacking;
        bool canAttack = inRange && distance <= attackDistance && attackTimer <= 0f && !isAttacking;

        if (animator != null)
        {
            animator.SetBool("isWalking", canMove);
            animator.SetBool("isAttacking", isAttacking);
        }

        if (canMove)
        {
            Vector3 direction = toPlayer.normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(direction),
                    10f * Time.deltaTime
                );
            }
        }

        if (canAttack)
            Attack();
    }

    void Attack()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        isAttacking = true;
        attackTimer = attackCooldown;

        if (animator != null)
            animator.SetBool("isAttacking", true);

        playerHealth.TakeDamage(damageToPlayer);
        hitCount++;

        if (hitSound != null)
            audioSource.PlayOneShot(hitSound, soundVolume);

        Invoke(nameof(StopAttack), attackAnimTime);

        if (hitCount >= maxHits)
        {
            CancelInvoke();
            Destroy(gameObject);
        }
    }

    void StopAttack()
    {
        isAttacking = false;

        if (animator != null)
            animator.SetBool("isAttacking", false);
    }
}