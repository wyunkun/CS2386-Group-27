using UnityEngine;

public class DestoryRobot : MonoBehaviour
{
    public GameObject explosionEffect;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            if (explosionEffect != null)
            {
                GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
                Destroy(explosion, 2f);
            }
            Destroy(gameObject);
        }
    }
}