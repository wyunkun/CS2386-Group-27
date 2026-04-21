using UnityEngine;

public class Lava : MonoBehaviour
{
    public int damageToPlayer = 10;
    public float damageCooldown = 1f;
    public AudioClip ambientSFX;
    public AudioClip burnSFX;

    private float damageTimer = 0f;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.spatialBlend = 0f;
        audioSource.volume = 30f;

        if (ambientSFX != null)
        {
            audioSource.clip = ambientSFX;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void Update()
    {
        if (damageTimer > 0f)
            damageTimer -= Time.deltaTime;
    }

    void OnCollisionStay(Collision collision)
    {
        if (!collision.collider.CompareTag("Player")) return;
        if (damageTimer > 0f) return;

        PlayerHealth playerHealth = collision.collider.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        playerHealth.TakeDamage(damageToPlayer);
        damageTimer = damageCooldown;

        if (burnSFX != null)
            audioSource.PlayOneShot(burnSFX);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("enemy")) return;

        DestoryRobot dr = collision.collider.GetComponent<DestoryRobot>();
        if (dr != null && dr.explosionEffect != null)
        {
            GameObject explosion = Instantiate(dr.explosionEffect, collision.transform.position, Quaternion.identity);
            Destroy(explosion, 2f);
        }

        Destroy(collision.gameObject);
    }
}