using UnityEngine;

public class DroneEnemy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float orbitDistance = 8f;
    public float orbitHeight = 5f;
    public float orbitSpeed = 30f;

    [Header("Attack")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float attackRange = 12f;
    public float fireRate = 2f;

    private Transform player;
    private float fireCooldown = 0f;
    private float orbitAngle = 0f;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;
    }

    void Update()
    {
        if (player == null) return;

        if (fireCooldown > 0f)
            fireCooldown -= Time.deltaTime;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > orbitDistance)
        {
            Vector3 target = player.position + Vector3.up * orbitHeight;
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        }
        else
        {
            orbitAngle += orbitSpeed * Time.deltaTime;
            float rad = orbitAngle * Mathf.Deg2Rad;
            Vector3 orbitPos = player.position + new Vector3(
                Mathf.Cos(rad) * orbitDistance,
                orbitHeight,
                Mathf.Sin(rad) * orbitDistance
            );
            transform.position = Vector3.MoveTowards(transform.position, orbitPos, moveSpeed * Time.deltaTime);
        }

        transform.LookAt(player.position);

        if (distance <= attackRange && fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = 1f / fireRate;
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = (player.position - firePoint.position).normalized;
            rb.linearVelocity = direction * 15f;
        }
    }
}