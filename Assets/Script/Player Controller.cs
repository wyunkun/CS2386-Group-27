using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    public Transform head;

    [Header("Move")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpSpeed = 5f;

    [Header("Look")]
    public float mouseSensitivity = 2f;
    public float minPitch = -85f;
    public float maxPitch = 85f;

    [Header("Ground Check")]
    public float groundRayLength = 0.02f;
    public float groundNormalMinY = 0.05f;

    private Vector2 moveInput;
    private bool runHeld;
    private bool jumpPressed;
    private bool isGrounded;
    private float pitch;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        ReadInput();
        Look();
    }

    void FixedUpdate()
    {
        GroundCheck();
        Move();
    }

    void ReadInput()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        runHeld = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetKeyDown(KeyCode.Space))
            jumpPressed = true;
    }

    void Move()
    {
        float speed = runHeld ? runSpeed : walkSpeed;

        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        Vector3 worldDirection = transform.TransformDirection(direction);

        float verticalVelocity = rb.linearVelocity.y;

        if (jumpPressed && isGrounded)
        {
            verticalVelocity = jumpSpeed;
            isGrounded = false;
        }
        jumpPressed = false;

        rb.linearVelocity = new Vector3(worldDirection.x * speed, verticalVelocity, worldDirection.z * speed);
    }

    void GroundCheck()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundRayLength))
            isGrounded = hit.normal.y > groundNormalMinY;
        else
            isGrounded = false;
    }

    void Look()
    {
        float sensitivity = PlayerPrefs.GetFloat("Sensitivity", mouseSensitivity);

        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        transform.Rotate(Vector3.up * mouseX);

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        if (head != null)
            head.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}