using UnityEngine;

public class Blade : MonoBehaviour
{
    public int damageToPlayer = 10;
    public float damageCooldown = 1f;
    public float rotationSpeed = 180f;
    public AudioClip spinSFX;
    public AudioClip hitSFX;

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
        if (!other.CompareTag("enemy")) return;

        DestoryRobot dr = other.GetComponent<DestoryRobot>();
        if (dr != null && dr.explosionEffect != null)
        {
            GameObject explosion = Instantiate(dr.explosionEffect, other.transform.position, Quaternion.identity);
            Destroy(explosion, 2f);
        }

        Destroy(other.gameObject);
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (damageTimer > 0f) return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        playerHealth.TakeDamage(damageToPlayer);
        damageTimer = damageCooldown;

        if (hitSFX != null)
            audioSource.PlayOneShot(hitSFX);
    }
}