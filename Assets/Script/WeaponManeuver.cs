using UnityEngine;

public class WeaponManeuver : MonoBehaviour
{
    Animator animator;
    public PlayerController playerController;
    public GunShoot gunShoot;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Reload();
        Run();
        Jump();
    }

    void Reload()
    {
        if (gunShoot.canReload)
        {
            animator.SetTrigger("isReloading");
            gunShoot.canReload = false;
        }
        
    }

    void Run()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        animator.SetBool("isRunning", isRunning);
        Debug.Log("isRunning = " + isRunning);
    }
    void Jump()
    {
        if (playerController.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                animator.SetTrigger("Jump");
        }
    }
}
