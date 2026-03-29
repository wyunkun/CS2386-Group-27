using UnityEngine;

public class Blade : MonoBehaviour
{
    [Header("Blade Settings")]
    public int damageToPlayer = 10;
    public float damageCooldown = 1f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 180f;

    [Header("Audio")]
    public AudioClip spinSFX;
    public AudioClip hitSFX;
    public float volume = 1f;

    private float damageTimer = 0f;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (spinSFX != null)
        {
            audioSource.clip = spinSFX;
            audioSource.loop = true;
            audioSource.volume = volume;
            audioSource.Play();
        }
    }

    void Update()
    {
        if (damageTimer > 0f)
            damageTimer -= Time.deltaTime;

        transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && damageTimer <= 0f)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageToPlayer);
                damageTimer = damageCooldown;

                if (hitSFX != null)
                    audioSource.PlayOneShot(hitSFX, volume);
            }
        }
    }
}