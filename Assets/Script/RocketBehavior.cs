using UnityEngine;

public class RocketBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject explosionPrefab;
    public float lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("enemy"))
        {
           if (explosionPrefab)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            Destroy(gameObject); 
        }
        
    }
}
