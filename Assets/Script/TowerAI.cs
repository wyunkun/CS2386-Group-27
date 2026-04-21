using Unity.VisualScripting;
using UnityEngine;

public class TowerAI : MonoBehaviour
{
    public enum TowerState {Patrol, Attact, Die}
    public TowerState currentState = TowerState.Patrol;
    public Transform turret;
    public GameObject explosionEffect;

    [Header("Patrol Settings")]
    public float rotationSpeed = 30f;
    public float maxrotationAngle = 90f;
    public float detectionRange = 10f;

    [Header("Attact Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 2;

    [Header("Die Settings")]
    public int health = 100;
    public GameObject destoryPrefab;

    [Header("Take Damate")]
    public int damage = 10;

    bool isTowerDead;
    float fireCooldown = 0;
    Transform target;

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case TowerState.Patrol:
                Patrol();
                break;
            
            case TowerState.Attact:
                Attack();
                break;
            
            case TowerState.Die:
                Die();
                break;
        }
    }

    void Patrol()
    {
        //turret.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        float angle = Mathf.PingPong(rotationSpeed * Time.time, maxrotationAngle * 2) - maxrotationAngle;
        turret.localRotation = Quaternion.Euler(0, angle, 0);

        LookforPlayer();

    }

    void Attack()
    {
        if(target == null || Vector3.Distance(transform.position, target.position) > detectionRange)
        {
            target = null;
            currentState = TowerState.Patrol;
            return;
        }

        //turret.LookAt(target);
        Vector3 direction = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        turret.rotation = Quaternion.Slerp(turret.rotation, lookRotation, rotationSpeed * Time.deltaTime);

        if(fireCooldown <= 0)
        {
            if(HasLineOfSight(target))
                Shoot();
                
            fireCooldown = 1 / fireRate;
        } 

        fireCooldown -= Time.deltaTime;
    }

    void Die()
    {
        if (isTowerDead)
            return;
        if(destoryPrefab)
            Instantiate(destoryPrefab, transform.position, transform.rotation);
        Destroy(gameObject, 1);

        isTowerDead = true;
    }

    void LookforPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);
        Transform nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;
        foreach(Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                float distanceToEnemy = Vector3.Distance(transform.position, collider.transform.position);
                if(distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = collider.transform;
                }
            }
        }

        if (nearestEnemy)
        {
            target = nearestEnemy;
            currentState = TowerState.Attact;
        }        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

    }

    void Shoot()
    {
        var bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        BulletBehavior bulletBehavior = bullet.GetComponent<BulletBehavior>();

        if(bulletBehavior)
            bulletBehavior.SetTarget(target);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if(health <= 0)
        {
            currentState = TowerState.Die;
        }
    }

    bool HasLineOfSight(Transform target)
    {
        RaycastHit hit;
        Vector3 direction = (target.position - firePoint.position).normalized;
        
        if(Physics.Raycast(firePoint.position, direction, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.CompareTag("Bullet"))
        {
            TakeDamage(damage);
        }

        if (collision.CompareTag("Rocket"))
        {
            if (explosionEffect != null)
            {
                GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
                Destroy(explosion, 1f);
            }
            Destroy(gameObject);
        }
    }

    void OnDrawGizmos()
    {
        Vector3 lineEndpoint = firePoint.position + (firePoint.forward * detectionRange);
        Debug.DrawLine(firePoint.position, lineEndpoint, Color.green);
    }
}
