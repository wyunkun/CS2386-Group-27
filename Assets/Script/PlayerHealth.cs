using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int startingHealth = 100;
    private int currentHealth;
    public float maxHealth = 100;
    public float invincibleDuration = 1.5f;

    [Header("Speed Boost on Hit")]
    public float speedMultiplier = 1.5f;
    public float speedBoostDuration = 3f;

    [Header("UI")]
    public TMP_Text healthText;

    [Header("Audio")]
    public AudioClip hitSFX;

    public static bool isAlive { get; private set; }

    private bool isInvincible = false;
    private float invincibleTimer = 0f;

    private PlayerController movement;
    private float originalWalkSpeed;
    private float originalRunSpeed;
    private Coroutine speedBoostCoroutine;

    void Start()
    {
        currentHealth = startingHealth;
        isAlive = true;
        UpdateHealthText();

        movement = GetComponent<PlayerController>();
        if (movement != null)
        {
            originalWalkSpeed = movement.walkSpeed;
            originalRunSpeed = movement.runSpeed;
        }
    }

    void Update()
    {
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0f)
                isInvincible = false;
        }
    }

    public void TakeHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = (int)maxHealth;
        UpdateHealthText();
    }

    public void TakeDamage(int damage)
    {
        if (!isAlive || isInvincible)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, startingHealth);

        if (hitSFX != null)
            AudioSource.PlayClipAtPoint(hitSFX, transform.position);

        UpdateHealthText();

        isInvincible = true;
        invincibleTimer = invincibleDuration;

        ApplySpeedBoost();

        if (currentHealth <= 0 && isAlive)
            Die();
    }

    void ApplySpeedBoost()
    {
        if (speedBoostCoroutine != null)
            StopCoroutine(speedBoostCoroutine);

        speedBoostCoroutine = StartCoroutine(SpeedBoostCoroutine());
    }

    IEnumerator SpeedBoostCoroutine()
    {
        if (movement != null)
        {
            movement.walkSpeed = originalWalkSpeed * speedMultiplier;
            movement.runSpeed = originalRunSpeed * speedMultiplier;
        }

        yield return new WaitForSeconds(speedBoostDuration);

        if (movement != null)
        {
            movement.walkSpeed = originalWalkSpeed;
            movement.runSpeed = originalRunSpeed;
        }
    }

    void UpdateHealthText()
    {
        if (healthText != null)
            healthText.text = "HP: " + currentHealth + " / " + startingHealth;
    }

    void Die()
    {
        isAlive = false;
        LevelManager levelManager = FindFirstObjectByType<LevelManager>();
        if (levelManager != null)
            levelManager.LevelLost();
    }
}