using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSPlayerController : MonoBehaviour
{
    [Header("move")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpHeight = 0.4f;
    public float gravity = -9.81f;
    public float airControl = 10f;

    [Header("view")]
    public float mouseSensitivity = 100f;
    public float pitchMin = -90f;
    public float pitchMax = 90f;
    public Transform cameraTransform; 

    Vector3 moveDirection;
    CharacterController controller;
    float pitch;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleLook();
        HandleMove();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void HandleLook()
    {
        float moveX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float moveY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;


        transform.Rotate(Vector3.up * moveX);


        pitch -= moveY;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
        if (cameraTransform)
            cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void HandleMove()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");


        Vector3 input = (transform.right * moveHorizontal + transform.forward * moveVertical).normalized;

        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        if (controller.isGrounded)
        {
            moveDirection = input;

            if (Input.GetButtonDown("Jump"))
                moveDirection.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(gravity));
            else
                moveDirection.y = 0f;
        }
        else
        {
            input.y = moveDirection.y;
            moveDirection = Vector3.Lerp(moveDirection, input, airControl * Time.deltaTime);
        }

        moveDirection.y += gravity * Time.deltaTime;
        controller.Move(moveDirection * speed * Time.deltaTime);
    }
}
