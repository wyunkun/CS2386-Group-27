using UnityEngine;

public class WeaponManeuver : MonoBehaviour
{
    Animator animator;
    public PlayerController playerController;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        animator.SetBool("isRunning", isRunning);
        Debug.Log("isRunning = " + isRunning);

        if (playerController.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                animator.SetTrigger("Jump");
        }
    }
}
