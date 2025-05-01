using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float jumpPower = 7f;
    [SerializeField]
    private float gravityMultiplier = 2f;
    [SerializeField]
    private float coyoteTime = 0.2f;
    [SerializeField]
    private Transform standartParent;
    [SerializeField]
    private Player player;
    [SerializeField]
    private Grounder grounder;
    [SerializeField]
    private Transform cameraPivot; // Точка фиксации камеры
    [SerializeField]
    private Transform mainCamera;
    [SerializeField]
    private Vector3 thirdPersonOffset;
    [SerializeField]
    private float cameraSpeed = 5f;
    [SerializeField]
    private float mouseSensitivity = 100f;
    [SerializeField]
    private float minZoom = 0f;
    [SerializeField]
    private float maxZoom = 3f;
    [SerializeField]
    private float zoomSpeed = 2f;
    [SerializeField]
    private float rotationSpeed = 5f;

    private Rigidbody rb;
    private Vector3 moveDirection = Vector3.zero;
    private bool isFalling = false;
    private float fallStartHeight = 0f;
    private float lastGroundedTime;
    private float currentZoom = 1f;
    private float xRotation = 0f;
    private float yRotation = 0f;

    private bool canJump => Time.time - lastGroundedTime < coyoteTime && grounder.IsGrounded != GroundedType.Dirt;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        grounder.OnGroundChanged += OnGroundChanged;

        if (mainCamera != null && cameraPivot != null)
        {
            currentZoom = maxZoom; // Начинаем с третьего лица
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDestroy()
    {
        grounder.OnGroundChanged -= OnGroundChanged;
    }

    private void OnGroundChanged(GameObject newGround)
    {
        if (newGround != null)
        {
            if (isFalling)
            {
                float fallDistance = fallStartHeight - transform.position.y;
                if (fallDistance > 0.5f)
                {
                    Debug.Log($"Высота падения: {fallDistance} метров");
                    if (grounder.IsGrounded != GroundedType.Jumper)
                    {
                        Debug.Log($"урон у нас: {(int)fallDistance}");
                        player.TakeDamage((int)fallDistance);
                    }
                }
                isFalling = false;
            }
            transform.parent = newGround.transform;
        }
        else
        {
            transform.parent = standartParent;
        }
    }

    private void Update()
    {
        Move();
        Jump();
        RotateCamera();
        UpdateCameraPosition();
    }

    private void Move()
    {
        Transform cam = Camera.main.transform;
        Vector3 forward = cam.forward;
        Vector3 right = cam.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        moveDirection = Vector3.zero;
        if (Input.GetKey(ButtonsPresets.Instance.KeyCodeW)) moveDirection += forward;
        if (Input.GetKey(ButtonsPresets.Instance.KeyCodeS)) moveDirection -= forward;
        if (Input.GetKey(ButtonsPresets.Instance.KeyCodeD)) moveDirection += right;
        if (Input.GetKey(ButtonsPresets.Instance.KeyCodeA)) moveDirection -= right;

        moveDirection.Normalize();
        float calculatedSpeed = speed;
        if (grounder.IsGrounded == GroundedType.Dirt)
            calculatedSpeed *= 0.5f;
        else if (grounder.IsGrounded == GroundedType.Jumper)
            calculatedSpeed *= 0.75f;

        rb.linearVelocity = new Vector3(moveDirection.x * calculatedSpeed, rb.linearVelocity.y, moveDirection.z * calculatedSpeed);
    }

    private void Jump()
    {
        if (grounder.IsGrounded != GroundedType.Null)
        {
            lastGroundedTime = Time.time;
            isFalling = false;
        }
        else if (!isFalling)
        {
            isFalling = true;
            fallStartHeight = transform.position.y;
        }

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            float jumpMultiplier = (grounder.IsGrounded == GroundedType.Jumper) ? 2.5f : 1f;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpPower * jumpMultiplier, rb.linearVelocity.z);
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (gravityMultiplier - 1) * Time.deltaTime;
        }
    }

    private void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yRotation += mouseX * rotationSpeed;
        xRotation -= mouseY * rotationSpeed;

        xRotation = Mathf.Clamp(xRotation, -30f, 60f);

        cameraPivot.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    private void UpdateCameraPosition()
    {
        if (mainCamera == null || cameraPivot == null) return;

        // Управление зумом через колесо мыши
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scroll * zoomSpeed; // Удалён Time.deltaTime для мгновенного изменения
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        if (currentZoom <= minZoom)
        {
            // Режим от первого лица: камера фиксируется в точке cameraPivot
            mainCamera.position = cameraPivot.position;
            mainCamera.rotation = cameraPivot.rotation;
        }
        else
        {
            // Режим от третьего лица
            Vector3 targetPosition = cameraPivot.position - cameraPivot.forward * currentZoom * thirdPersonOffset.magnitude;
            mainCamera.position = Vector3.Lerp(mainCamera.position, targetPosition, Time.deltaTime * cameraSpeed);
            mainCamera.LookAt(cameraPivot.position);
        }
    }
}