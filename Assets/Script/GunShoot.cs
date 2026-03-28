using UnityEngine;
using UnityEngine.Animations;

public class GunShoot : MonoBehaviour
{
    [Header("References")]
    public Camera Camera;
    public Transform muzzle;
    public GameObject bulletPrefab;

    [Header("Shoot Settings")]
    public float bulletSpeed = 80f;
    public float maxDistance = 200f;
    public float fireRate = 0.12f;
    public AudioClip shootSFX;
    public AudioSource audioSource;
    public Animator animator;

    private float nextFireTime;

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            if (!CanShoot())
                return;
            
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        Vector3 targetPoint;

        Ray ray = new Ray(Camera.transform.position, Camera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = Camera.transform.position + Camera.transform.forward * maxDistance;
        }

        Vector3 shootDirection = (targetPoint - muzzle.position).normalized;

        GameObject bullet = Instantiate(
            bulletPrefab,
            muzzle.position,
            Quaternion.LookRotation(shootDirection)
        );

        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb)
        {
            bulletRb.linearVelocity = shootDirection * bulletSpeed;
        }

        if (shootSFX)
            if(audioSource)
                audioSource.PlayOneShot(shootSFX);
    }

    bool CanShoot()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName("Default");
    }
}
