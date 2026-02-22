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
    public float groundRayLength = 1.1f;
    public float groundNormalMinY = 0.5f;

    // runtime
    private Vector2 moveInput;     // x=Horizontal, y=Vertical
    private bool runHeld;
    private bool jumpStatus;
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
            jumpStatus = true;
    }

    void Move()
    {
        float speed = runHeld ? runSpeed : walkSpeed;

        Vector3 input = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        Vector3 world = transform.TransformDirection(input);

        float y = rb.linearVelocity.y;

        if (jumpStatus && isGrounded)
        {
            y = jumpSpeed;
            isGrounded = false; 
        }
        jumpStatus = false;

        rb.linearVelocity = new Vector3(world.x * speed, y, world.z * speed);
    }

    void GroundCheck()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundRayLength))
        {
            isGrounded = hit.normal.y > groundNormalMinY;
        }
        else
        {
            isGrounded = false;
        }
    }

    void Look()
    {
        float mx = Input.GetAxis("Mouse X") * mouseSensitivity;
        float my = Input.GetAxis("Mouse Y") * mouseSensitivity;


        transform.Rotate(Vector3.up * mx);

        pitch -= my;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        if (head)
            head.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}