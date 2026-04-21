using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    public float speed = 20f;
    public float rotationSpeed = 5f;
    public float lifeTime = 5f;
    public int damage = 10;
    public GameObject bulletHitPrefab;
    private Rigidbody rb;
    private Transform target;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(target == null)
            return;
        Vector3 direction = (target.position - transform.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

        rb.linearVelocity = transform.forward * speed;
    }

    public void SetTarget(Transform currentTarget)
    {
        target = currentTarget;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (bulletHitPrefab)
        {   
            var pos = collision.contacts[0].point;

            Instantiate(bulletHitPrefab, pos, Quaternion.identity);
        }
        if (collision.collider.CompareTag("Player"))
        {
            PlayerHealth health = collision.collider.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(damage);
        }
    }

    public int GetDamageValue()
    {
        return damage;
    }
}
