using UnityEngine;

public class DroneEnemy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float minOrbitDistance = 4f;
    public float maxOrbitDistance = 8f;
    public float minOrbitHeight = 3f;
    public float maxOrbitHeight = 6f;
    public float minOrbitSpeed = 20f;
    public float maxOrbitSpeed = 50f;

    [Header("Attack")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float attackRange = 10f;
    public float fireRate = 2f;

    private Transform player;
    private float fireCooldown = 0f;
    private float orbitAngle = 0f;
    private float orbitDistance;
    private float orbitHeight;
    private float orbitSpeed;
    private float orbitDirection;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;

        orbitDistance = Random.Range(minOrbitDistance, maxOrbitDistance);
        orbitHeight = Random.Range(minOrbitHeight, maxOrbitHeight);
        orbitSpeed = Random.Range(minOrbitSpeed, maxOrbitSpeed);
        orbitDirection = Random.value > 0.5f ? 1f : -1f;
        orbitAngle = Random.Range(0f, 360f);
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
            orbitAngle += orbitSpeed * orbitDirection * Time.deltaTime;
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