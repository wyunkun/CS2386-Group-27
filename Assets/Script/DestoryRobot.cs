using UnityEngine;

public class DestoryRobot : MonoBehaviour
{
    public GameObject explosionEffect;
    private int hitCount = 0;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            if (explosionEffect != null)
            {
                GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
                Destroy(explosion, 1f);
            }
            hitCount += 1;
            if(hitCount >= 5)
                Destroy(gameObject);
        }

        if (other.CompareTag("Rocket"))
        {
            if (explosionEffect != null)
            {
                GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
                Destroy(explosion, 1f);
            }
            Destroy(gameObject);
        }
    }
}