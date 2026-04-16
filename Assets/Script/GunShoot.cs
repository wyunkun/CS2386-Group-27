using TMPro;
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

    [Header("Ammo")]
    public int totalAmmoAmonut = 100;
    public int magazineAmmoAmout = 30;
    public TMP_Text ammo;

    public bool canReload;
    private float nextFireTime;
    private int currentAmmoAmount;
    void Start()
    {
        currentAmmoAmount = magazineAmmoAmout;
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            if (!CanShoot())
                return;
            if(currentAmmoAmount > 0)
            {
                nextFireTime = Time.time + fireRate;
                Shoot();
                currentAmmoAmount -= 1;
            }
            
        }
        Reload();
        UpdataAmmoText();
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

    void Reload()
    {
        if (totalAmmoAmonut <= 0)
            return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            if(currentAmmoAmount < 30)
            {
                canReload = true;
                Invoke(nameof(FinishReload), 2f);
            }
        }
    }

    void FinishReload()
    {
        int needAmmoAmount = magazineAmmoAmout - currentAmmoAmount;
        int loadAmmoAmount = Mathf.Min(totalAmmoAmonut, needAmmoAmount);

        currentAmmoAmount += loadAmmoAmount;
        totalAmmoAmonut -= loadAmmoAmount;
        canReload = false;
    }

    void UpdataAmmoText()
    {
        ammo.text = currentAmmoAmount.ToString() + "/" + totalAmmoAmonut.ToString();
    }
}
